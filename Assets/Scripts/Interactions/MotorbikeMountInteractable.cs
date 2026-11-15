using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Prologue;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Vật thể tương tác xe máy dựng ngoài sân nhà người chơi (Motorbike Mount Interactable).
    /// Người chơi sau khi mở cửa bước ra sân sẽ nhìn thấy chiếc xe máy dựng sẵn:
    /// - Nhìn vào xe máy hiển thị: "[E] Lên xe máy đến Midnight Mart".
    /// - Nhấn [E]: Người chơi leo lên xe, nổ máy và bắt đầu chuyến đi xuyên qua con đường rừng âm u.
    /// </summary>
    public class MotorbikeMountInteractable : MonoBehaviour, IInteractable
    {
        private InteractionHighlighter _highlighter;
        private bool _hasMounted = false;

        private void Awake()
        {
            _highlighter = GetComponent<InteractionHighlighter>();
            if (_highlighter == null)
            {
                _highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }
        }

        public string GetInteractionPrompt()
        {
            if (_hasMounted) return string.Empty;
            return "[E] Lên xe máy đến tiệm Midnight Mart";
        }

        public bool CanInteract()
        {
            return !_hasMounted;
        }

        public void SetHighlight(bool isHighlighted)
        {
            _highlighter?.SetHighlight(isHighlighted);
        }

        public void OnInteract(Transform playerTransform)
        {
            if (_hasMounted) return;
            _hasMounted = true;
            SetHighlight(false);

            if (PlayerHomeManager.Instance != null)
            {
                PlayerHomeManager.Instance.MountMotorbikeAndRide();
            }
        }
    }
}

