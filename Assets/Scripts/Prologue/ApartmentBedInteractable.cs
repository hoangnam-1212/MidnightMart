using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Interactions;

namespace MidnightMart.Prologue
{
    /// <summary>
    /// Đối tượng Tương tác Chiếc Giường Ngủ tại Căn Hộ (Apartment Bed Interactable).
    /// Cho phép người chơi leo lên giường ngủ sau khi lái xe máy về nhà,
    /// kích hoạt chuỗi chuyển cảnh ác mộng và bắt đầu Ngày 2 (Day 2).
    /// </summary>
    public class ApartmentBedInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _promptVietnamese = "[E] Lên giường đi ngủ (Chuyển sang Ngày 2)";
        [SerializeField] private string _promptEnglish = "[E] Sleep on bed (Advance to Day 2)";

        public string GetInteractionPrompt()
        {
            return LocalizationManager.T(_promptVietnamese, _promptEnglish);
        }

        public bool CanInteract()
        {
            if (PlayerHomeManager.Instance != null)
            {
                return PlayerHomeManager.Instance.CanSleepForDay2;
            }
            return false;
        }

        public void OnInteract(Transform playerTransform)
        {
            if (!CanInteract()) return;

            if (PlayerHomeManager.Instance != null)
            {
                PlayerHomeManager.Instance.GoToSleepForDay2();
            }
        }

        public void SetHighlight(bool isHighlighted)
        {
            // Tùy chọn hiệu ứng viền sáng khi lia tâm ngắm vào giường
        }
    }
}
