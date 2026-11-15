using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Interactions;

namespace MidnightMart.Customer
{
    /// <summary>
    /// Thẻ Căn cước công dân (ID Card) của khách hàng đặt lên quầy thanh toán.
    /// Là công cụ cốt lõi trong Giai đoạn A để người chơi phân biệt Khách hàng thật và Kẻ giả dạng:
    /// - Khách thật: Tên hợp lệ, năm sinh bình thường (ví dụ: 1952, 1978, 1994).
    /// - Quái vật giả dạng: Năm sinh phi lý (Ví dụ: 1820 - đã hơn 200 tuổi!), khuôn mặt vô hồn hoặc địa chỉ không tồn tại.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class CustomerIDCard : MonoBehaviour, IInteractable
    {
        [Header("Thông tin Căn cước")]
        [SerializeField] private string _holderName = "Robert Cole";
        [SerializeField] private int _birthYear = 1968;
        [SerializeField] private string _idNumber = "ID-9024-BKWD";
        [SerializeField] private string _address = "Thị trấn Blackwood County";
        [SerializeField] private bool _isDoppelganger = false;

        private InteractionHighlighter _highlighter;

        public string HolderName => _holderName;
        public int BirthYear => _birthYear;
        public string IdNumber => _idNumber;
        public string Address => _address;
        public bool IsDoppelganger => _isDoppelganger;

        private void Awake()
        {
            _highlighter = GetComponent<InteractionHighlighter>();
            if (_highlighter == null)
            {
                _highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }

            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null)
            {
                col.isTrigger = false;
                // Kích thước chuẩn thẻ căn cước dễ trỏ tâm ngắm
                col.size = new Vector3(0.24f, 0.08f, 0.32f);
            }
        }

        public void SetupIDCard(string holderName, int birthYear, string idNumber, string address, bool isDoppelganger)
        {
            _holderName = holderName;
            _birthYear = birthYear;
            _idNumber = idNumber;
            _address = address;
            _isDoppelganger = isDoppelganger;
        }

        public string GetInteractionPrompt()
        {
            return "[E] Soi Thẻ Căn Cước (ID Card)";
        }

        public bool CanInteract()
        {
            return gameObject.activeInHierarchy;
        }

        public void SetHighlight(bool isHighlighted)
        {
            _highlighter?.SetHighlight(isHighlighted);
        }

        public void OnInteract(Transform playerTransform)
        {
            SetHighlight(false);

            // Hiển thị nguyên bản thông tin in trên thẻ để người chơi tự quan sát, suy luận và đối chiếu
            // Tuyệt đối không phán quyết sẵn kết quả (không spoil "hợp lệ" hay "quái vật")
            string idCardText = $"<b>THẺ CĂN CƯỚC THỊ TRẤN BLACKWOOD</b>\n" +
                                $"───────────────────────────────\n" +
                                $"• <b>HỌ VÀ TÊN:</b> {_holderName.ToUpper()}\n" +
                                $"• <b>NĂM SINH:</b> {_birthYear}\n" +
                                $"• <b>MÃ ĐỊNH DANH (ID):</b> {_idNumber}\n" +
                                $"• <b>NƠI CƯ TRÚ:</b> {_address}\n" +
                                $"• <b>HẠN SỬ DỤNG:</b> 10 NĂM KỂ TỪ NGÀY CẤP";

            GameEvents.TriggerSubtitle("CĂN CƯỚC CÔNG DÂN (ID CARD)", idCardText, 6.0f);
        }
    }
}

