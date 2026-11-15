using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Tutorial;
using MidnightMart.Player;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Cây lau nhà đặt trong xô nước ở góc trước siêu thị (Store Cleaning Mop).
    /// Bắt buộc người chơi phải lại gần nhấn [E] để cầm chổi lau trước khi có thể lau được các vệt bẩn dưới sàn.
    /// </summary>
    public class StoreCleaningMop : MonoBehaviour, IInteractable
    {
        [Header("Trạng thái")]
        [SerializeField] private bool _isPickedUp = false;
        [SerializeField] private GameObject _mopHandleVisual;

        [Header("Âm thanh")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _pickupClip;

        private InteractionHighlighter _highlighter;

        public bool IsPickedUp => _isPickedUp;

        private void Awake()
        {
            if (_mopHandleVisual == null)
            {
                Transform handleTrans = transform.Find("Mop_Handle");
                if (handleTrans != null) _mopHandleVisual = handleTrans.gameObject;
            }

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
            _audioSource.spatialBlend = 0.6f;
        }

        public string GetInteractionPrompt()
        {
            return _isPickedUp ? "[E] Cất Cây Lau Nhà vào xô" : "[E] Cầm Cây Lau Nhà";
        }

        public bool CanInteract()
        {
            return true;
        }

        public void SetHighlight(bool isHighlighted)
        {
            _highlighter?.SetHighlight(isHighlighted);
        }

        public void OnInteract(Transform playerTransform)
        {
            PlayerInventory inv = playerTransform != null ? playerTransform.GetComponent<PlayerInventory>() : null;
            if (inv == null) inv = PlayerInventory.Instance;

            if (!_isPickedUp)
            {
                bool acquired = false;
                if (inv != null)
                {
                    acquired = inv.TryAcquireItem(InventoryItemType.Mop, "CHỔI LAU");
                }

                if (!acquired)
                {
                    GameEvents.TriggerSubtitle("Kho đồ", "Kho đồ đã đầy! Không thể cầm thêm Cây Lau Nhà.", 2.5f);
                    return;
                }

                _isPickedUp = true;

                if (_audioSource != null && _pickupClip != null)
                {
                    _audioSource.PlayOneShot(_pickupClip);
                }

                if (_highlighter != null) _highlighter.SetHighlight(false);

                // Báo cho TutorialManager biết đã có chổi lau nhà
                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.ReportCleaningMopPickedUp();
                }

                // Ẩn cán cây lau nhà trong xô nước (người chơi đã nhấc ra cầm)
                if (_mopHandleVisual != null)
                {
                    _mopHandleVisual.SetActive(false);
                }
            }
            else
            {
                // Cất cây lau nhà trở lại vào xô sau khi lau xong
                if (inv != null && inv.HasItemType(InventoryItemType.Mop))
                {
                    inv.RemoveItemType(InventoryItemType.Mop);
                    _isPickedUp = false;

                    if (_mopHandleVisual != null)
                    {
                        _mopHandleVisual.SetActive(true);
                    }

                    if (_audioSource != null && _pickupClip != null)
                    {
                        _audioSource.PlayOneShot(_pickupClip);
                    }

                    if (_highlighter != null) _highlighter.SetHighlight(false);
                }
            }
        }
    }
}

