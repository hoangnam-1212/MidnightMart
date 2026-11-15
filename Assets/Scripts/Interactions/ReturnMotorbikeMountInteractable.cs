using UnityEngine;
using MidnightMart.Core;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Đối tượng Tương tác Chiếc Xe Máy Trở Về Nhà Sau Ca Trực (Return Motorbike Mount Interactable).
    /// Dựng bên ngoài cửa tiệm Midnight Mart sau khi hoàn thành ca trực 06:00 AM.
    /// Cho phép người chơi nhấn [E] để leo lên xe và bắt đầu hành trình lái xe về phòng trọ.
    /// </summary>
    public class ReturnMotorbikeMountInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _promptVietnamese = "[E] Lên xe máy lái về nhà nghỉ ngơi";
        [SerializeField] private string _promptEnglish = "[E] Mount motorbike & ride home";

        public string GetInteractionPrompt()
        {
            return LocalizationManager.T(_promptVietnamese, _promptEnglish);
        }

        public bool CanInteract()
        {
            if (Night.NightShiftManager.Instance != null)
            {
                return Night.NightShiftManager.Instance.IsReturnRideAvailable;
            }
            return true;
        }

        public void OnInteract(Transform playerTransform)
        {
            if (!CanInteract()) return;

            if (Night.NightShiftManager.Instance != null)
            {
                Night.NightShiftManager.Instance.StartReturnHomeRide();
            }
        }

        public void SetHighlight(bool isHighlighted)
        {
            // Tùy chọn hiệu ứng viền sáng khi lia tâm ngắm vào xe máy
        }
    }
}
