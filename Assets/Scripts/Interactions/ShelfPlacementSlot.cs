using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Player;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Vị trí ô trống được chỉ định trên kệ hàng siêu thị để người chơi đặt các món đồ bị rơi vãi lên.
    /// Có khay định vị phát sáng (Visual Tray Indicator) và vùng va chạm (BoxCollider)
    /// để người chơi nhìn từ xa thấy rõ ô trống trên kệ, lại gần bấm phím E để xếp đồ lên.
    /// </summary>
    public class ShelfPlacementSlot : MonoBehaviour, IInteractable
    {
        [Header("Thông tin Vị trí Kệ")]
        [SerializeField] private MessItem _targetMessItem;
        [SerializeField] private string _slotLabel = "Kệ hàng";
        [SerializeField] private GameObject _visualBracket;

        private bool _isItemPlaced;
        private InteractionHighlighter _highlighter;

        public bool IsItemPlaced => _isItemPlaced;

        private void Awake()
        {
            _highlighter = GetComponent<InteractionHighlighter>();
            if (_highlighter == null)
            {
                _highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }
        }

        public void SetupSlot(MessItem messItem, string label, GameObject visualBracket = null)
        {
            _targetMessItem = messItem;
            _slotLabel = label;
            _visualBracket = visualBracket;
        }

        public string GetInteractionPrompt()
        {
            if (_isItemPlaced) return string.Empty;

            if (_targetMessItem != null)
            {
                return $"[E] Đặt {_targetMessItem.ItemName} lên ô này";
            }

            return $"[E] Ô trống: {_slotLabel}";
        }

        public bool CanInteract()
        {
            return !_isItemPlaced;
        }

        public void SetHighlight(bool isHighlighted)
        {
            if (_isItemPlaced)
            {
                _highlighter?.SetHighlight(false);
                return;
            }
            _highlighter?.SetHighlight(isHighlighted);
        }

        public void OnInteract(Transform playerTransform)
        {
            if (_isItemPlaced) return;

            PlayerInteraction playerInteraction = playerTransform.GetComponent<PlayerInteraction>();
            if (playerInteraction == null) return;

            // Nếu người chơi đang cầm đồ vật trên tay
            if (playerInteraction.IsHoldingItem && playerInteraction.HeldItemTransform != null)
            {
                MessItem heldMess = playerInteraction.HeldItemTransform.GetComponent<MessItem>();
                if (heldMess != null)
                {
                    _isItemPlaced = true;
                    SetHighlight(false);
                    if (_visualBracket != null) _visualBracket.SetActive(false);

                    heldMess.PlaceOnShelf(transform.position, transform.rotation);
                    playerInteraction.ReleaseHeldItemSilently();
                    return;
                }
            }

            // Nếu người chơi nhìn vào ô trống khi tay không hoặc chưa cầm đồ
            GameEvents.TriggerSubtitle("Kệ hàng", $"Ô trống này dành cho {_slotLabel}! Hãy nhặt món đồ rơi dưới sàn mang lại đây.", 2.5f);
        }

        /// <summary>
        /// Đánh dấu món đồ đã được đặt xong lên kệ này.
        /// </summary>
        public void MarkPlaced()
        {
            _isItemPlaced = true;
            SetHighlight(false);
            if (_visualBracket != null) _visualBracket.SetActive(false);
        }
    }
}
