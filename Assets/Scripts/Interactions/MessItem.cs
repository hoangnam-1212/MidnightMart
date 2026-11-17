using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Player;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Đại diện cho các món đồ bị rơi vãi bừa bộn trên sàn nhà (bánh snack, chai nước đổ).
    /// Người chơi nhặt để hoàn thành nhiệm vụ dọn dẹp ban đầu trong kịch bản Tutorial.
    /// Hỗ trợ nhặt lên, vứt xuống đất và nhặt lại thoải mái không bao giờ bị kẹt.
    /// </summary>
    public class MessItem : MonoBehaviour, IInteractable
    {
        [Header("Thông tin")]
        [SerializeField] private string _itemName = "Hộp bánh Snack bị đổ";
        [SerializeField] private Transform _targetShelfPosition; // Vị trí sau khi dọn gọn gàng
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _pickupSound;

        private bool _isPickedUp;
        private bool _isCleaned;
        private InteractionHighlighter _highlighter;
        private Rigidbody _rigidbody;

        public string ItemName => _itemName;
        public bool IsPickedUp => _isPickedUp;
        public bool IsCleaned => _isCleaned;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _highlighter = GetComponent<InteractionHighlighter>();
            if (_highlighter == null)
            {
                _highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;

#if UNITY_EDITOR
            if (_pickupSound == null)
            {
                _pickupSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Item_Pickup_Pop.wav");
            }
#endif
            if (_pickupSound == null)
            {
                _pickupSound = ProceduralAudioGenerator.CreateItemPickupPopClip();
            }
        }

        public string GetInteractionPrompt()
        {
            if (_isCleaned) return string.Empty;

            if (_isPickedUp)
            {
                return $"[E] Đặt {_itemName} lên kệ";
            }

            return $"[E] Nhặt {_itemName}";
        }

        public bool CanInteract()
        {
            return !_isCleaned;
        }

        public void SetHighlight(bool isHighlighted)
        {
            if (_isCleaned || _isPickedUp)
            {
                _highlighter?.SetHighlight(false);
                return;
            }
            _highlighter?.SetHighlight(isHighlighted);
        }

        /// <summary>
        /// Xử lý tương tác: Nhặt lên tay từ sàn, hoặc nếu đang cầm thì đặt lên kệ.
        /// </summary>
        public void OnInteract(Transform playerTransform)
        {
            if (_isCleaned) return;

            PlayerInteraction playerInteraction = playerTransform.GetComponent<PlayerInteraction>();
            if (playerInteraction == null) return;

            // 1. Nếu đang cầm món đồ này trên tay
            if (playerInteraction.IsHoldingItem && playerInteraction.HeldItemTransform == transform)
            {
                if (_targetShelfPosition != null && Vector3.Distance(transform.position, _targetShelfPosition.position) < 3.0f)
                {
                    PlaceOnShelf(_targetShelfPosition.position, _targetShelfPosition.rotation);
                    playerInteraction.ReleaseHeldItemSilently();
                }
                else
                {
                    playerInteraction.DropHeldItem();
                }
                return;
            }

            // 2. Nếu món đồ đang nằm trên sàn: Nhặt lên tay
            _isPickedUp = true;
            SetHighlight(false);

            if (_pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(_pickupSound, transform.position);
            }

            playerInteraction.PickUpItem(transform, _rigidbody);
        }

        /// <summary>
        /// Gọi khi người chơi vứt đồ rơi xuống đất, cho phép nhặt lại bình thường.
        /// </summary>
        public void OnDropped()
        {
            _isPickedUp = false;
            SetHighlight(false);
        }

        /// <summary>
        /// Đặt vật thể cố định lên kệ hàng và hoàn thành tiến trình dọn dẹp.
        /// </summary>
        public void PlaceOnShelf(Vector3 targetPos, Quaternion targetRot)
        {
            if (_isCleaned) return;

            _isCleaned = true;
            _isPickedUp = false;
            SetHighlight(false);

            transform.position = targetPos;
            transform.rotation = targetRot;

            // Đưa mô hình con về trạng thái đứng thẳng ngay ngắn trên mâm kệ (không còn bị nghiêng/lăn lóc)
            Transform visual = transform.Find("Visual_Model");
            if (visual != null)
            {
                visual.localPosition = Vector3.zero;
                visual.localRotation = Quaternion.identity;
            }

            if (_rigidbody != null)
            {
                if (!_rigidbody.isKinematic)
                {
                    _rigidbody.linearVelocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;
                }
                _rigidbody.isKinematic = true;
                _rigidbody.useGravity = false;
            }

            if (_pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(_pickupSound, targetPos);
            }

            // Báo cho TutorialManager biết đã dọn thêm 1 món
            Tutorial.TutorialManager.Instance?.ReportMessCleaned();
        }
    }
}
