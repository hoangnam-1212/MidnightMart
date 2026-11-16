using UnityEngine;
using UnityEngine.InputSystem;
using MidnightMart.Core;
using MidnightMart.Interactions;

namespace MidnightMart.Player
{
    /// <summary>
    /// Hệ thống tương tác vật lý và đồ vật của người chơi.
    /// Sử dụng Physics.RaycastNonAlloc để quét vật thể trước mắt mà không gây cấp phát rác (Zero GC).
    /// Hỗ trợ: Nhặt đồ vật, thả đồ, đặt đồ lên kệ, sử dụng máy quét mã vạch và cơ chế trong tiệm.
    /// Tự động loại bỏ vật thể đang cầm khỏi tia quét để người chơi luôn nhìn xuyên qua tay vào kệ/máy móc.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Cấu hình Raycast Tương tác")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _maxInteractionDistance = 3.5f;
        [SerializeField] private float _sphereCastRadius = 0.2f;
        [SerializeField] private LayerMask _interactionLayers = ~0; // Mặc định quét mọi Layer

        [Header("Vị trí Cầm nắm đồ vật (Hold Socket)")]
        [SerializeField] private Transform _itemHoldSocket;
        [SerializeField] private float _pickupSmoothSpeed = 15f;

        private PlayerInventory _inventory;
        private CharacterController _characterController;

        // Bộ đệm Raycast không tạo rác (Non-Alloc Buffer)
        private readonly RaycastHit[] _hitBuffer = new RaycastHit[8];
        private IInteractable _currentHoveredInteractable;
        private Transform _heldItemTransform;
        private Rigidbody _heldItemRigidbody;
        private bool _isHoldingItem;
        private bool _isDialogueOpen;
        private float _lastInteractionTimestamp = -1f;
        private const float INTERACTION_DEBOUNCE = 0.35f;

        // Danh sách Collider của vật thể đang cầm (dùng để vô hiệu hóa khi cầm và phục hồi khi thả)
        private readonly System.Collections.Generic.List<Collider> _heldItemColliders = new System.Collections.Generic.List<Collider>();
        private readonly System.Collections.Generic.Dictionary<Collider, bool> _heldItemColliderStates = new System.Collections.Generic.Dictionary<Collider, bool>();

        public bool IsHoldingItem => _isHoldingItem;
        public Transform HeldItemTransform => _heldItemTransform;

        private void Awake()
        {
            _inventory = GetComponent<PlayerInventory>();
            _characterController = GetComponent<CharacterController>();
            if (_characterController == null) _characterController = GetComponentInParent<CharacterController>();

            if (_cameraTransform == null)
            {
                if (Camera.main != null) _cameraTransform = Camera.main.transform;
                else _cameraTransform = GetComponentInChildren<Camera>()?.transform;
            }

            if (_itemHoldSocket == null && _cameraTransform != null)
            {
                Transform foundSocket = _cameraTransform.Find("Hold_Item_Socket");
                if (foundSocket != null) _itemHoldSocket = foundSocket;
            }

            // Phòng thủ: Đảm bảo LayerMask quét mọi Layer (tránh bị kẹt ở giá trị 0 do serialize)
            if (_interactionLayers.value == 0)
            {
                _interactionLayers = ~0;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnDialogueStateChanged += HandleDialogueStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnDialogueStateChanged -= HandleDialogueStateChanged;
            if (_isHoldingItem)
            {
                RestoreHeldItemColliders();
            }
        }

        private void HandleDialogueStateChanged(bool isOpen)
        {
            _isDialogueOpen = isOpen;
            if (_isDialogueOpen)
            {
                if (_currentHoveredInteractable != null)
                {
                    _currentHoveredInteractable.SetHighlight(false);
                    _currentHoveredInteractable = null;
                }
                GameEvents.TriggerInteractionHover(string.Empty);
            }
        }

        private void Update()
        {
            if (!_isDialogueOpen)
            {
                CheckForInteractables();
            }
            UpdateHeldItemPosition();

            // Phím E hoặc Click Chuột Trái (khi không cầm súng/máy quét) tương tác trực tiếp
            bool ePressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
            bool isHoldingGun = _inventory != null && _inventory.ActiveSlotItem != null && _inventory.ActiveSlotItem.ItemType == InventoryItemType.Gun;
            bool isHoldingScanner = _inventory != null && _inventory.ActiveSlotItem != null && _inventory.ActiveSlotItem.ItemType == InventoryItemType.Scanner;
            bool leftClickPressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

            if (!_isDialogueOpen && (ePressed || (leftClickPressed && !isHoldingGun && !isHoldingScanner)))
            {
                OnInteractDirect();
            }

            // Phím G hoặc Q: Vứt đồ đang cầm xuống đất nếu muốn thả nhanh
            if (!_isDialogueOpen && _isHoldingItem && Keyboard.current != null && (Keyboard.current.gKey.wasPressedThisFrame || Keyboard.current.qKey.wasPressedThisFrame))
            {
                DropHeldItem();
            }
        }

        private void OnInteractDirect()
        {
            if (_isDialogueOpen) return;

            // Chống ấn đúp liên tiếp trong 0.35s (tránh nhấn E giấy note ăn luôn chổi lau nhà)
            if (Time.unscaledTime - _lastInteractionTimestamp < INTERACTION_DEBOUNCE) return;
            _lastInteractionTimestamp = Time.unscaledTime;

            // 1. Không cho phép nhặt đồ nếu đang rút súng
            if (_inventory != null && _inventory.ActiveSlotItem != null && _inventory.ActiveSlotItem.ItemType == InventoryItemType.Gun)
            {
                GameEvents.TriggerSubtitle("Hệ thống", "Đang cầm súng! Nhấn phím số để chuyển sang Bàn tay trước khi nhặt đồ.", 2.0f);
                return;
            }

            // 2. Nếu đang cầm đồ vật và đang nhìn vào một vật thể tương tác (ví dụ: ShelfPlacementSlot để xếp đồ, máy quét):
            if (_isHoldingItem && _currentHoveredInteractable != null)
            {
                _currentHoveredInteractable.OnInteract(transform);
                return;
            }

            // 3. Nếu đang cầm đồ vật và nhìn ra chỗ trống (nhìn xuống sàn): Thả đồ xuống đất
            if (_isHoldingItem && _currentHoveredInteractable == null)
            {
                DropHeldItem();
                return;
            }

            // 4. Nếu tay không và nhìn vào vật thể tương tác: Nhặt đồ lên hoặc kích hoạt
            if (_currentHoveredInteractable != null)
            {
                _currentHoveredInteractable.OnInteract(transform);
            }
        }

        /// <summary>
        /// Bắn tia Raycast & SphereCast từ tâm camera để kiểm tra vật thể tương tác trước mặt.
        /// Đảm bảo loại bỏ hoàn toàn va chạm từ bản thân người chơi và vật thể đang cầm trên tay.
        /// </summary>
        private void CheckForInteractables()
        {
            if (_cameraTransform == null)
            {
                if (Camera.main != null) _cameraTransform = Camera.main.transform;
                else _cameraTransform = GetComponentInChildren<Camera>()?.transform;
                if (_cameraTransform == null) return;
            }

            Vector3 rayOrigin = _cameraTransform.position + _cameraTransform.forward * 0.35f;
            Ray ray = new Ray(rayOrigin, _cameraTransform.forward);
            IInteractable closestInteractable = null;
            float closestDistance = float.MaxValue;
            float actualMaxDist = Mathf.Max(_maxInteractionDistance, 4.5f);
            LayerMask mask = _interactionLayers.value != 0 ? _interactionLayers : (LayerMask)(~0);

            // Bước 1: Tia Raycast chuẩn từ tâm camera (có sắp xếp theo khoảng cách & chặn xuyên tường)
            int hitCount = Physics.RaycastNonAlloc(ray, _hitBuffer, actualMaxDist, mask, QueryTriggerInteraction.Collide);
            if (hitCount > 1)
            {
                System.Array.Sort(_hitBuffer, 0, hitCount, System.Collections.Generic.Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));
            }

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = _hitBuffer[i];
                if (hit.collider == null) continue;
                if (hit.collider.transform.root == transform.root) continue;

                // Bỏ qua vật thể đang cầm trên tay để tia nhìn xuyên qua tới kệ hàng
                if (_isHoldingItem && _heldItemTransform != null)
                {
                    if (hit.collider.transform == _heldItemTransform || hit.collider.transform.IsChildOf(_heldItemTransform))
                    {
                        continue;
                    }
                }

                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable == null)
                {
                    interactable = hit.collider.GetComponentInChildren<IInteractable>();
                }

                if (interactable != null && interactable.CanInteract())
                {
                    // Kiểm tra đường ngắm thẳng (Line of Sight): Đảm bảo không có tường/cửa chắn giữa camera và điểm tương tác
                    Vector3 targetPoint = hit.point != Vector3.zero ? hit.point : hit.collider.bounds.center;
                    Vector3 toTarget = targetPoint - rayOrigin;
                    float targetDist = toTarget.magnitude;

                    if (targetDist > 0.15f)
                    {
                        if (Physics.Raycast(rayOrigin, toTarget.normalized, out RaycastHit occluderHit, targetDist - 0.08f, mask, QueryTriggerInteraction.Ignore))
                        {
                            if (occluderHit.collider != null &&
                                occluderHit.collider != hit.collider &&
                                occluderHit.collider.transform.root != transform.root &&
                                !occluderHit.collider.transform.IsChildOf(hit.collider.transform) &&
                                !hit.collider.transform.IsChildOf(occluderHit.collider.transform))
                            {
                                string occName = occluderHit.collider.name.ToLower();
                                if (occName.Contains("wall") || occName.Contains("glass") || occName.Contains("door_frame") || occName.Contains("ceiling") || occName.Contains("storefront") || occluderHit.collider.CompareTag("Wall"))
                                {
                                    // Bị tường hoặc kính che khuất tầm nhìn -> Ngừng quét, không cho nhặt xuyên tường!
                                    break;
                                }
                            }
                        }
                    }

                    closestInteractable = interactable;
                    closestDistance = hit.distance;
                    break; // Tìm thấy mục tiêu hợp lệ gần nhất
                }

                // Nếu gặp bức tường kiến trúc thực sự (Tường nhà, kính mặt tiền):
                if (!hit.collider.isTrigger)
                {
                    string colName = hit.collider.name.ToLower();
                    if (colName.Contains("wall") || colName.Contains("glass") || colName.Contains("door_frame") || colName.Contains("ceiling") || colName.Contains("storefront") || hit.collider.CompareTag("Wall"))
                    {
                        // Tia nhìn bị chặn bởi tường cứng! Ngăn tương tác với các vật thể phía sau tường
                        break;
                    }
                }
            }

            // Bước 2: SphereCast quét diện rộng nếu Raycast chưa bắt trúng (vẫn kiểm tra chặn xuyên tường)
            if (closestInteractable == null)
            {
                float sphereRadius = Mathf.Max(_sphereCastRadius, 0.30f);
                int sphereHitCount = Physics.SphereCastNonAlloc(ray, sphereRadius, _hitBuffer, actualMaxDist, mask, QueryTriggerInteraction.Collide);
                if (sphereHitCount > 1)
                {
                    System.Array.Sort(_hitBuffer, 0, sphereHitCount, System.Collections.Generic.Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));
                }

                for (int i = 0; i < sphereHitCount; i++)
                {
                    RaycastHit hit = _hitBuffer[i];
                    if (hit.collider == null) continue;
                    if (hit.collider.transform.root == transform.root) continue;

                    if (_isHoldingItem && _heldItemTransform != null)
                    {
                        if (hit.collider.transform == _heldItemTransform || hit.collider.transform.IsChildOf(_heldItemTransform))
                        {
                            continue;
                        }
                    }

                    IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                    if (interactable == null)
                    {
                        interactable = hit.collider.GetComponentInChildren<IInteractable>();
                    }

                    if (interactable != null && interactable.CanInteract())
                    {
                        Vector3 targetPoint = hit.point != Vector3.zero ? hit.point : hit.collider.bounds.center;
                        Vector3 toTarget = targetPoint - rayOrigin;
                        float targetDist = toTarget.magnitude;

                        if (targetDist > 0.15f)
                        {
                            if (Physics.Raycast(rayOrigin, toTarget.normalized, out RaycastHit occluderHit, targetDist - 0.08f, mask, QueryTriggerInteraction.Ignore))
                            {
                                if (occluderHit.collider != null &&
                                    occluderHit.collider != hit.collider &&
                                    occluderHit.collider.transform.root != transform.root &&
                                    !occluderHit.collider.transform.IsChildOf(hit.collider.transform) &&
                                    !hit.collider.transform.IsChildOf(occluderHit.collider.transform))
                                {
                                    string occName = occluderHit.collider.name.ToLower();
                                    if (occName.Contains("wall") || occName.Contains("glass") || occName.Contains("door_frame") || occName.Contains("ceiling") || occName.Contains("storefront") || occluderHit.collider.CompareTag("Wall"))
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        closestInteractable = interactable;
                        closestDistance = hit.distance;
                        break;
                    }

                    if (!hit.collider.isTrigger)
                    {
                        string colName = hit.collider.name.ToLower();
                        if (colName.Contains("wall") || colName.Contains("glass") || colName.Contains("door_frame") || colName.Contains("ceiling") || colName.Contains("storefront") || hit.collider.CompareTag("Wall"))
                        {
                            break;
                        }
                    }
                }
            }

            if (closestInteractable != null)
            {
                if (_currentHoveredInteractable != closestInteractable)
                {
                    _currentHoveredInteractable?.SetHighlight(false);
                    _currentHoveredInteractable = closestInteractable;
                    _currentHoveredInteractable.SetHighlight(true);
                }

                // Cập nhật thông báo tương tác của đối tượng đang nhìn vào
                string promptText = closestInteractable.GetInteractionPrompt();
                GameEvents.TriggerInteractionHover(promptText);
                return;
            }

            // Nếu không nhìn vào vật thể nào:
            if (_currentHoveredInteractable != null)
            {
                _currentHoveredInteractable.SetHighlight(false);
                _currentHoveredInteractable = null;
            }

            // Nếu đang cầm đồ vật và nhìn ra sàn/khoảng trống: Nhắc người chơi có thể bấm E để thả
            if (_isHoldingItem)
            {
                GameEvents.TriggerInteractionHover("[E] Tha mon do xuong dat");
            }
            else
            {
                GameEvents.TriggerInteractionHover(string.Empty);
            }
        }

        /// <summary>
        /// Callback từ New Input System khi nhấn nút Tương tác (phím E).
        /// </summary>
        public void OnInteract(InputValue value)
        {
            if (!value.isPressed) return;
            OnInteractDirect();
        }

        /// <summary>
        /// Cầm một vật thể vật lý lên tay (Hold Item).
        /// Vô hiệu hóa toàn bộ va chạm Collider với Player để loại bỏ triệt để hiện tượng kẹt phím di chuyển (WASD).
        /// </summary>
        public void PickUpItem(Transform item, Rigidbody rb)
        {
            if (_isHoldingItem)
            {
                DropHeldItem();
            }

            _heldItemTransform = item;
            _heldItemRigidbody = rb;
            _isHoldingItem = true;

            // 1. Tắt tạm thời Collider của vật thể và bỏ qua va chạm với CharacterController
            _heldItemColliders.Clear();
            _heldItemColliderStates.Clear();
            if (item != null)
            {
                item.GetComponentsInChildren(true, _heldItemColliders);
                for (int i = 0; i < _heldItemColliders.Count; i++)
                {
                    Collider col = _heldItemColliders[i];
                    if (col != null)
                    {
                        _heldItemColliderStates[col] = col.enabled;
                        col.enabled = false;
                        if (_characterController != null)
                        {
                            Physics.IgnoreCollision(col, _characterController, true);
                        }
                    }
                }
            }

            // 2. Chuyển Rigidbody sang Kinematic để không bị trọng lực kéo tụt
            if (_heldItemRigidbody != null)
            {
                _heldItemRigidbody.isKinematic = true;
                _heldItemRigidbody.useGravity = false;
                _heldItemRigidbody.linearVelocity = Vector3.zero;
                _heldItemRigidbody.angularVelocity = Vector3.zero;
            }

            // 3. Đưa vật thể lập tức tới vị trí socket để không bị giật lag khung hình đầu tiên
            if (_itemHoldSocket != null && _heldItemTransform != null)
            {
                _heldItemTransform.position = _itemHoldSocket.position;
                _heldItemTransform.rotation = _itemHoldSocket.rotation;
            }

            // Đồng bộ tên món hàng lên UI ô vuông kho đồ động
            GameEvents.TriggerHeldItemChanged(item != null ? item.name : null);
            if (_inventory != null && item != null)
            {
                _inventory.TryAcquireItem(InventoryItemType.PhysicalItem, item.name, 0, 0, item, rb);
            }
        }

        /// <summary>
        /// Thả vật thể đang cầm rơi tự do theo vật lý.
        /// </summary>
        public void DropHeldItem()
        {
            if (!_isHoldingItem || _heldItemTransform == null) return;

            // Báo cho vật thể biết đã bị vứt xuống sàn để có thể nhặt lại bình thường
            MessItem mess = _heldItemTransform.GetComponent<MessItem>();
            if (mess != null)
            {
                mess.OnDropped();
            }

            // Bật lại toàn bộ Collider của vật thể
            RestoreHeldItemColliders();

            // Đặt vị trí rơi hơi hướng ra phía trước mặt để tránh va quẹt vào chân người chơi
            if (_cameraTransform != null)
            {
                _heldItemTransform.position = _cameraTransform.position + _cameraTransform.forward * 0.75f - Vector3.up * 0.15f;
            }

            if (_heldItemRigidbody != null)
            {
                _heldItemRigidbody.isKinematic = false;
                _heldItemRigidbody.useGravity = true;
                // Đẩy nhẹ về phía trước
                Vector3 forwardDir = _cameraTransform != null ? _cameraTransform.forward : transform.forward;
                _heldItemRigidbody.linearVelocity = forwardDir * 1.5f + Vector3.up * 0.5f;
            }

            _heldItemTransform = null;
            _heldItemRigidbody = null;
            _isHoldingItem = false;

            // Xóa tên món hàng khỏi UI và giải phóng ô kho đồ
            if (_inventory != null)
            {
                int physSlot = _inventory.FindSlotIndex(InventoryItemType.PhysicalItem);
                if (physSlot >= 0) _inventory.ClearSlot(physSlot);
            }
            GameEvents.TriggerHeldItemChanged(null);
        }

        /// <summary>
        /// Bỏ tham chiếu vật thể đang cầm mà không làm rơi theo vật lý (dùng khi đặt gọn gàng lên kệ).
        /// </summary>
        public void ReleaseHeldItemSilently()
        {
            // Bật lại Collider để vật thể nằm chắc chắn trên giá kệ
            RestoreHeldItemColliders();

            _heldItemTransform = null;
            _heldItemRigidbody = null;
            _isHoldingItem = false;

            // Xóa tên món hàng khỏi UI và giải phóng ô kho đồ
            if (_inventory != null)
            {
                int physSlot = _inventory.FindSlotIndex(InventoryItemType.PhysicalItem);
                if (physSlot >= 0) _inventory.ClearSlot(physSlot);
            }
            GameEvents.TriggerHeldItemChanged(null);
        }

        /// <summary>
        /// Khôi phục trạng thái hoạt động của các Collider trên vật thể sau khi buông tay.
        /// </summary>
        private void RestoreHeldItemColliders()
        {
            for (int i = 0; i < _heldItemColliders.Count; i++)
            {
                Collider col = _heldItemColliders[i];
                if (col != null)
                {
                    if (_heldItemColliderStates.TryGetValue(col, out bool wasEnabled))
                    {
                        col.enabled = wasEnabled;
                    }
                    else
                    {
                        col.enabled = true;
                    }

                    if (_characterController != null)
                    {
                        Physics.IgnoreCollision(col, _characterController, false);
                    }
                }
            }
            _heldItemColliders.Clear();
            _heldItemColliderStates.Clear();
        }

        /// <summary>
        /// Cập nhật vị trí vật thể cầm trên tay đi theo tâm camera mượt mà.
        /// </summary>
        private void UpdateHeldItemPosition()
        {
            if (!_isHoldingItem || _heldItemTransform == null || _itemHoldSocket == null) return;

            _heldItemTransform.position = Vector3.Lerp(_heldItemTransform.position, _itemHoldSocket.position, Time.deltaTime * _pickupSmoothSpeed);
            _heldItemTransform.rotation = Quaternion.Slerp(_heldItemTransform.rotation, _itemHoldSocket.rotation, Time.deltaTime * _pickupSmoothSpeed);
        }
    }
}
