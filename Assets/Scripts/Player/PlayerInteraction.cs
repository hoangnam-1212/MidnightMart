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

        // Bộ đệm Raycast không tạo rác (Non-Alloc Buffer)
        private readonly RaycastHit[] _hitBuffer = new RaycastHit[8];
        private IInteractable _currentHoveredInteractable;
        private Transform _heldItemTransform;
        private Rigidbody _heldItemRigidbody;
        private bool _isHoldingItem;
        private bool _isDialogueOpen;
        private float _lastInteractionTimestamp = -1f;
        private const float INTERACTION_DEBOUNCE = 0.35f;

        public bool IsHoldingItem => _isHoldingItem;
        public Transform HeldItemTransform => _heldItemTransform;

        private void Awake()
        {
            _inventory = GetComponent<PlayerInventory>();
            if (_cameraTransform == null)
            {
                if (Camera.main != null) _cameraTransform = Camera.main.transform;
                else _cameraTransform = GetComponentInChildren<Camera>()?.transform;
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

            // Phím E tương tác trực tiếp
            if (!_isDialogueOpen && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
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

            // Bước 1: Tia Raycast chuẩn từ tâm camera
            int hitCount = Physics.RaycastNonAlloc(ray, _hitBuffer, actualMaxDist, mask, QueryTriggerInteraction.Collide);
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
                    float dist = hit.distance > 0.001f ? hit.distance : Vector3.Distance(rayOrigin, hit.collider.bounds.center);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestInteractable = interactable;
                    }
                }
            }

            // Bước 2: SphereCast quét diện rộng nếu Raycast chưa bắt trúng
            if (closestInteractable == null)
            {
                float sphereRadius = Mathf.Max(_sphereCastRadius, 0.35f);
                int sphereHitCount = Physics.SphereCastNonAlloc(ray, sphereRadius, _hitBuffer, actualMaxDist, mask, QueryTriggerInteraction.Collide);
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
                        float dist = hit.distance > 0.001f ? hit.distance : Vector3.Distance(rayOrigin, hit.collider.bounds.center);
                        if (dist < closestDistance)
                        {
                            closestDistance = dist;
                            closestInteractable = interactable;
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

            if (_heldItemRigidbody != null)
            {
                _heldItemRigidbody.isKinematic = true;
                _heldItemRigidbody.useGravity = false;
                _heldItemRigidbody.linearVelocity = Vector3.zero;
                _heldItemRigidbody.angularVelocity = Vector3.zero;
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

            if (_heldItemRigidbody != null)
            {
                _heldItemRigidbody.isKinematic = false;
                _heldItemRigidbody.useGravity = true;
                // Đẩy nhẹ về phía trước
                _heldItemRigidbody.linearVelocity = _cameraTransform.forward * 1.5f;
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
