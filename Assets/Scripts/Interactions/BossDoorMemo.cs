using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Tutorial;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Tờ giấy note hướng dẫn ca đêm của Quản lý dán trên cửa kính siêu thị (Boss Shift Memo Note).
    /// Bắt buộc người chơi phải đọc trước khi bước vào tiệm:
    /// - Giải thích bối cảnh: David ca trước mất tích bí ẩn.
    /// - Giao nhiệm vụ chuẩn bị ca trực: Xếp 3 món đồ rơi vãi lên kệ & Lau sạch 3 vệt bẩn dưới sàn.
    /// - Cảnh báo quái vật dị biến: Không bóng đổ, tra cứu máy tính ở bàn phía sau quầy.
    /// - Mở khóa cửa kính tự động cuốn sang 2 bên để người chơi bước vào.
    /// </summary>
    public class BossDoorMemo : MonoBehaviour, IInteractable
    {
        [Header("Trạng thái")]
        [SerializeField] private bool _hasBeenRead = false;

        [Header("Tham chiếu Cửa & Quản lý")]
        [SerializeField] private StoreAutomaticDoors _storeDoors;
        [SerializeField] private TutorialManager _tutorialManager;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _paperRustleClip;

        private InteractionHighlighter _highlighter;

        public bool HasBeenRead => _hasBeenRead;

        private void Awake()
        {
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
            _audioSource.spatialBlend = 0.5f;

            if (_storeDoors == null)
            {
                _storeDoors = FindAnyObjectByType<StoreAutomaticDoors>();
            }
            if (_tutorialManager == null)
            {
                _tutorialManager = FindAnyObjectByType<TutorialManager>();
            }
        }

        public string GetInteractionPrompt()
        {
            return _hasBeenRead ? "[E] Đọc lại Bản Hướng Dẫn Ca Đêm của Quản Lý" : "[E] Đọc Bản Hướng Dẫn Ca Đêm Của Quản Lý (Dán trên cửa)";
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
            if (_audioSource != null && _paperRustleClip != null)
            {
                _audioSource.PlayOneShot(_paperRustleClip);
            }

            _hasBeenRead = true;

            // Hiển thị nội dung bản ghi nhớ gọn gàng, súc tích của Quản lý siêu thị (tránh tràn/đè chữ)
            string memoContent = "1. Cậu trực thay David (nó đã mất tích đêm qua). Đừng tò mò.\n" +
                                 "2. Trước 01:00 AM: Nhặt chổi lau 3 vết bẩn & Xếp 3 món đồ rơi lên kệ.\n" +
                                 "3. Kẻ không có bóng đổ là quái vật giả mạo! Tra cứu máy tính ở bàn sau quầy.\n" +
                                 "4. Súng 9mm và đạn cất trong ngăn kéo quầy khi gặp nguy hiểm khẩn cấp.";

            GameEvents.TriggerSubtitle("Ghi Nhớ Quản Lý", memoContent, 10.0f);

            // Mở khóa cửa kính cảm ứng trượt tự động
            if (_storeDoors != null)
            {
                _storeDoors.UnlockDoors();
            }

            // Báo cho TutorialManager chuyển sang nhiệm vụ dọn dẹp & lau sàn
            if (_tutorialManager != null)
            {
                _tutorialManager.ReportBossMemoRead();
            }
            else if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.ReportBossMemoRead();
            }
        }
    }
}

