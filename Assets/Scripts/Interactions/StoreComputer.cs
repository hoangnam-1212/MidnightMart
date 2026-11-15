using UnityEngine;
using MidnightMart.Core;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Máy tính đặt tại quầy thu ngân (Store Desktop Computer).
    /// Hỗ trợ người chơi:
    /// 1. Tra cứu CSDL Công dân / Hồ sơ Cảnh sát để phát hiện kẻ giả mạo (Doppelgänger).
    /// 2. Đặt mua hàng tiếp tế khẩn cấp: Đạn súng lục, đạn shotgun, nước tăng lực, ván gia cố.
    /// </summary>
    public class StoreComputer : MonoBehaviour, IInteractable
    {
        [Header("Thành phần Âm thanh")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _bootClip;
        [SerializeField] private AudioClip _clickClip;

        private InteractionHighlighter _highlighter;

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
            _audioSource.spatialBlend = 0.6f;

#if UNITY_EDITOR
            if (_clickClip == null)
            {
                _clickClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Cash_Register_Ding.wav");
            }
#endif
            if (_clickClip == null)
            {
                _clickClip = ProceduralAudioGenerator.CreateCashRegisterDingClip();
            }
        }

        public string GetInteractionPrompt()
        {
            return "[E] Dùng Máy tính Cửa hàng (CSDL Công dân & Tiếp tế)";
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
            if (_audioSource != null && _clickClip != null)
            {
                _audioSource.PlayOneShot(_clickClip);
            }

            // Kích hoạt mở giao diện Máy tính Cửa hàng
            GameEvents.TriggerStoreComputerToggled(true);
        }
    }
}

