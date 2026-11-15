using System.Collections;
using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Tutorial;
using MidnightMart.Player;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Vết bẩn / Vũng chất lỏng sẫm màu bí ẩn dưới sàn gạch siêu thị (Mysterious Floor Dirt Stain).
    /// Người chơi tiến lại gần nhấn [E] để lau sạch (hoặc dùng cây lau nhà).
    /// Khi lau xong 3 vệt bẩn và xếp xong 3 món đồ, ca trực mới được mở khóa đón khách.
    /// </summary>
    public class StoreFloorStain : MonoBehaviour, IInteractable
    {
        [Header("Trạng thái")]
        [SerializeField] private bool _isCleaned = false;

        [Header("Thành phần & Hiệu ứng")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _mopClip;

        private InteractionHighlighter _highlighter;
        private Collider _collider;
        private Renderer _renderer;

        public bool IsCleaned => _isCleaned;

        private void Awake()
        {
            _highlighter = GetComponent<InteractionHighlighter>();
            if (_highlighter == null)
            {
                _highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }

            _collider = GetComponent<Collider>();
            _renderer = GetComponent<Renderer>();

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0.75f;
        }

        public string GetInteractionPrompt()
        {
            if (_isCleaned) return string.Empty;
            bool hasMop = (PlayerInventory.Instance != null && PlayerInventory.Instance.HasItemType(InventoryItemType.Mop)) ||
                          (TutorialManager.Instance != null && TutorialManager.Instance.HasCleaningMop);
            if (!hasMop) return "[!] Cần nhặt Cây Lau Nhà trong tiệm để lau sàn";

            bool isHoldingMop = PlayerInventory.Instance != null && PlayerInventory.Instance.ActiveSlotItem != null && PlayerInventory.Instance.ActiveSlotItem.ItemType == InventoryItemType.Mop;
            if (isHoldingMop)
            {
                return "[E] Dùng Cây Lau Nhà chà sạch vết bẩn";
            }
            else
            {
                int mopSlot = PlayerInventory.Instance != null ? PlayerInventory.Instance.FindSlotIndex(InventoryItemType.Mop) : -1;
                string slotKey = mopSlot >= 0 ? $"[{mopSlot + 1}]" : "[E]";
                return $"[E] Rút Cây Lau Nhà {slotKey} chà sạch vết bẩn";
            }
        }

        public bool CanInteract()
        {
            if (_isCleaned) return false;
            return (PlayerInventory.Instance != null && PlayerInventory.Instance.HasItemType(InventoryItemType.Mop)) ||
                   (TutorialManager.Instance != null && TutorialManager.Instance.HasCleaningMop);
        }

        public void SetHighlight(bool isHighlighted)
        {
            if (_isCleaned) return;
            _highlighter?.SetHighlight(isHighlighted);
        }

        public void OnInteract(Transform playerTransform)
        {
            if (_isCleaned) return;
            bool hasMop = (PlayerInventory.Instance != null && PlayerInventory.Instance.HasItemType(InventoryItemType.Mop)) ||
                          (TutorialManager.Instance != null && TutorialManager.Instance.HasCleaningMop);
            if (!hasMop)
            {
                GameEvents.TriggerSubtitle("Thiếu dụng cụ", "Bạn không thể dùng tay không để chùi vết bẩn này! Hãy nhặt Cây Lau Nhà trong tiệm.", 3.5f);
                return;
            }

            // Nếu người chơi có chổi trong kho đồ nhưng đang chọn ô khác, tự động rút chổi ra lau
            if (PlayerInventory.Instance != null && (PlayerInventory.Instance.ActiveSlotItem == null || PlayerInventory.Instance.ActiveSlotItem.ItemType != InventoryItemType.Mop))
            {
                int mopSlot = PlayerInventory.Instance.FindSlotIndex(InventoryItemType.Mop);
                if (mopSlot >= 0)
                {
                    PlayerInventory.Instance.SelectSlot(mopSlot);
                }
            }

            _isCleaned = true;

            if (_collider != null) _collider.enabled = false;
            if (_highlighter != null) _highlighter.SetHighlight(false);

            StartCoroutine(CleanStainRoutine());
        }

        private IEnumerator CleanStainRoutine()
        {
            // Âm thanh chà lau sàn
            if (_audioSource != null && _mopClip != null)
            {
                _audioSource.PlayOneShot(_mopClip);
            }

            Vector3 startScale = transform.localScale;
            float elapsed = 0f;
            float duration = 0.6f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                yield return null;
            }

            transform.localScale = Vector3.zero;

            // Báo cáo hoàn thành cho TutorialManager
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.ReportStainCleaned();
            }

            gameObject.SetActive(false);
        }
    }
}

