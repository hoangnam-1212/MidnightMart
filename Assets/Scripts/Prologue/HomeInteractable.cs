
using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Interactions;

namespace MidnightMart.Prologue
{
    /// <summary>
    /// Các vật thể tương tác trong căn hộ của người chơi trước khi đi làm:
    /// - Bàn làm việc: Giấy báo nợ tiền nhà và tin nhắn tuyển dụng thay thế David mất tích.
    /// - Cửa phòng: Lên xe đi làm trong đêm mưa bão.
    /// </summary>
    public class HomeInteractable : MonoBehaviour, IInteractable
    {
        public enum HomeType
        {
            DeskNotice,
            ApartmentDoor
        }

        [Header("Loại Tương tác")]
        [SerializeField] private HomeType _type = HomeType.DeskNotice;
        [SerializeField] private PlayerHomeManager _homeManager;

        private InteractionHighlighter _highlighter;
        private bool _isHighlighted;

        private void Awake()
        {
            _highlighter = GetComponent<InteractionHighlighter>();
            if (_highlighter == null)
            {
                _highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }
            if (_homeManager == null)
            {
                _homeManager = Object.FindAnyObjectByType<PlayerHomeManager>();
            }
        }

        public string GetInteractionPrompt()
        {
            if (_type == HomeType.DeskNotice)
            {
                return "[E] Đọc Giấy báo nợ & Tin nhắn tuyển dụng gấp";
            }
            else if (_type == HomeType.ApartmentDoor)
            {
                if (_homeManager != null && !_homeManager.HasReadJobNotice)
                {
                    return "[E] Cửa khóa (Hãy kiểm tra bàn làm việc trước)";
                }

                return _homeManager != null && _homeManager.IsDoorOpen ? 
                    "[E] Đóng cửa phòng lại" : 
                    "[E] Mở cửa bước ra ngoài sân";
            }

            return string.Empty;
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
            SetHighlight(false);

            if (_homeManager == null)
            {
                _homeManager = Object.FindAnyObjectByType<PlayerHomeManager>();
            }

            if (_homeManager == null) return;

            if (_type == HomeType.DeskNotice)
            {
                _homeManager.InteractWithNotice();
            }
            else if (_type == HomeType.ApartmentDoor)
            {
                _homeManager.InteractWithDoor();
            }
        }
    }
}

