using UnityEngine;
using UnityEngine.InputSystem;
using MidnightMart.Core;
using MidnightMart.Player;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Máy quét mã vạch cầm tay tại quầy thu ngân (Tactile Barcode Scanner).
    /// - Khi đặt trên giá ở quầy: Nhấn [E] để nhấc máy quét vào kho đồ hoặc đặt lại lên giá.
    /// - Khi cầm trên tay (FPS Tool): Bắn tia laser đỏ trực diện, ngắm vào hàng hóa và click Chuột Trái (hoặc [E]) để quét mã vạch TÍT nịnh tai.
    /// - Hàng hóa của khách chỉ quét mã để tính tiền, tuyệt đối không cho phép người chơi cầm lên tay làm rơi vãi.
    /// </summary>
    public class BarcodeScanner : MonoBehaviour, IInteractable
    {
        [Header("Chế độ Hoạt động")]
        [Tooltip("True nếu script này gắn trên mô hình cầm tay FPS của nhân vật; False nếu là dock đặt trên quầy")]
        [SerializeField] private bool _isFPSTool = false;
        [SerializeField] private GameObject _dockGunModel; // Mô hình súng trên giá đỡ (nếu là dock)

        [Header("Cấu hình Tia Laser Quét")]
        [SerializeField] private Transform _laserEmitter;
        [SerializeField] private LineRenderer _laserLine;
        [SerializeField] private float _scanRange = 3.5f;
        [SerializeField] private LayerMask _scanLayers = ~0;

        [Header("Âm thanh Phản hồi")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _beepSuccessSound;
        [SerializeField] private AudioClip _errorSound;

        private InteractionHighlighter _highlighter;
        private readonly RaycastHit[] _scanHits = new RaycastHit[1];
        private ScannableItem _currentHoveredItem;

        private void Awake()
        {
            if (_laserLine != null)
            {
                _laserLine.enabled = false;
            }

            _highlighter = GetComponent<InteractionHighlighter>();
            if (_highlighter == null && !_isFPSTool)
            {
                _highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0.5f;

#if UNITY_EDITOR
            if (_beepSuccessSound == null)
            {
                _beepSuccessSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Barcode_Scan_Beep.wav");
            }
#endif
            if (_beepSuccessSound == null)
            {
                _beepSuccessSound = ProceduralAudioGenerator.CreateBarcodeBeepClip();
            }
        }

        public string GetInteractionPrompt()
        {
            if (_isFPSTool) return string.Empty;

            PlayerInventory inv = PlayerInventory.Instance;
            if (inv != null && inv.HasItemType(InventoryItemType.Scanner))
            {
                return "[E] Đặt Máy Quét Lên Giá";
            }

            return "[E] Cầm Máy Quét Mã Vạch";
        }

        public bool CanInteract()
        {
            return !_isFPSTool;
        }

        public void SetHighlight(bool isHighlighted)
        {
            if (_isFPSTool) return;
            _highlighter?.SetHighlight(isHighlighted);
        }

        public void OnInteract(Transform playerTransform)
        {
            if (_isFPSTool) return;

            PlayerInventory inv = playerTransform != null ? playerTransform.GetComponent<PlayerInventory>() : PlayerInventory.Instance;
            if (inv == null) return;

            if (inv.HasItemType(InventoryItemType.Scanner))
            {
                // Cất máy quét lại lên giá quầy thu ngân
                inv.RemoveItemType(InventoryItemType.Scanner);
                if (_dockGunModel != null) _dockGunModel.SetActive(true);
                if (_audioSource != null && _beepSuccessSound != null) _audioSource.PlayOneShot(_beepSuccessSound);
            }
            else
            {
                // Nhấc máy quét vào ô kho đồ động
                bool acquired = inv.TryAcquireItem(InventoryItemType.Scanner, "MÁY QUÉT");
                if (acquired)
                {
                    if (_dockGunModel != null) _dockGunModel.SetActive(false);
                    if (_audioSource != null && _beepSuccessSound != null) _audioSource.PlayOneShot(_beepSuccessSound);
                }
            }
        }

        private void OnEnable()
        {
            if (_laserLine != null && _isFPSTool)
            {
                _laserLine.enabled = true;
            }
        }

        private void OnDisable()
        {
            if (_laserLine != null)
            {
                _laserLine.enabled = false;
            }
            if (_currentHoveredItem != null)
            {
                _currentHoveredItem.SetHighlight(false);
                _currentHoveredItem = null;
            }
        }

        private void Update()
        {
            if (!_isFPSTool) return;

            PlayerInventory inv = PlayerInventory.Instance;
            bool isHeldInHand = inv != null && !inv.IsHolstered && 
                               inv.ActiveSlotItem != null && 
                               inv.ActiveSlotItem.ItemType == InventoryItemType.Scanner;

            if (!isHeldInHand)
            {
                if (_laserLine != null) _laserLine.enabled = false;
                if (_currentHoveredItem != null)
                {
                    _currentHoveredItem.SetHighlight(false);
                    _currentHoveredItem = null;
                }
                return;
            }

            if (_laserLine != null) _laserLine.enabled = true;

            UpdateLaserBeamAndCheckTarget();

            // Nhấn chuột trái hoặc phím E để bắn tia quét mã
            bool scanTriggered = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                                (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame);

            if (scanTriggered)
            {
                PerformScan();
            }
        }

        /// <summary>
        /// Vẽ tia laser đỏ từ máy quét hướng thẳng theo tâm nhìn camera và kiểm tra món hàng trước mặt.
        /// </summary>
        private void UpdateLaserBeamAndCheckTarget()
        {
            Transform camTrans = Camera.main != null ? Camera.main.transform : transform;
            Vector3 startPos = _laserEmitter != null ? _laserEmitter.position : (camTrans.position + camTrans.forward * 0.2f);
            Vector3 rayOrigin = camTrans.position;
            Vector3 rayDir = camTrans.forward;

            Vector3 targetPos = startPos + rayDir * _scanRange;

            Ray ray = new Ray(rayOrigin, rayDir);
            ScannableItem hitItem = null;

            if (Physics.RaycastNonAlloc(ray, _scanHits, _scanRange, _scanLayers) > 0)
            {
                targetPos = _scanHits[0].point;
                hitItem = _scanHits[0].collider.GetComponentInParent<ScannableItem>();
            }

            if (_laserLine != null)
            {
                _laserLine.SetPosition(0, startPos);
                _laserLine.SetPosition(1, targetPos);
            }

            if (hitItem != _currentHoveredItem)
            {
                if (_currentHoveredItem != null) _currentHoveredItem.SetHighlight(false);
                _currentHoveredItem = hitItem;
                if (_currentHoveredItem != null && !_currentHoveredItem.IsScanned)
                {
                    _currentHoveredItem.SetHighlight(true);
                }
            }
        }

        /// <summary>
        /// Thực hiện quét món hàng đang ngắm tới.
        /// </summary>
        public void PerformScan()
        {
            if (_currentHoveredItem != null && !_currentHoveredItem.IsScanned)
            {
                _currentHoveredItem.MarkAsScanned();
                _currentHoveredItem.SetHighlight(false);

                // Âm thanh tít nịnh tai
                if (_audioSource != null && _beepSuccessSound != null)
                {
                    _audioSource.PlayOneShot(_beepSuccessSound);
                }

                // Cập nhật giá tiền vào máy tính tiền
                GameEvents.TriggerItemScanned(_currentHoveredItem.ProductName, _currentHoveredItem.Price);
                return;
            }

            // Quét trượt ra ngoài
            if (_audioSource != null && _errorSound != null)
            {
                _audioSource.PlayOneShot(_errorSound);
            }
        }
    }
}

