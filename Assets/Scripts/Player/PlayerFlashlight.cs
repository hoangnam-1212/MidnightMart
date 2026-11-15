using UnityEngine;
using UnityEngine.InputSystem;
using MidnightMart.Core;

namespace MidnightMart.Player
{
    /// <summary>
    /// Điều khiển đèn pin cá nhân của người chơi (Flashlight Controller).
    /// Bật/Tắt bằng phím F, hỗ trợ hiệu ứng âm thanh và phát sự kiện trạng thái qua GameEvents.
    /// </summary>
    public class PlayerFlashlight : MonoBehaviour
    {
        [Header("Tham chiếu Nguồn sáng")]
        [SerializeField] private Light _flashlightLight;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _clickSound;

        [Header("Trạng thái")]
        [SerializeField] private bool _startsOn = false;

        private bool _isOn;

        public bool IsOn => _isOn;

        private void Awake()
        {
            _isOn = _startsOn;
            if (_flashlightLight != null)
            {
                _flashlightLight.enabled = _isOn;
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;

#if UNITY_EDITOR
            if (_clickSound == null)
            {
                _clickSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Flashlight_Click.wav");
            }
#endif
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                ToggleFlashlight();
            }
        }

        /// <summary>
        /// Callback từ New Input System khi người chơi nhấn phím F (Action: Flashlight).
        /// </summary>
        public void OnFlashlight(InputValue value)
        {
            if (!value.isPressed) return;

            ToggleFlashlight();
        }

        /// <summary>
        /// Đảo trạng thái bật/tắt của đèn pin.
        /// </summary>
        public void ToggleFlashlight()
        {
            _isOn = !_isOn;

            if (_flashlightLight != null)
            {
                _flashlightLight.enabled = _isOn;
            }

            if (_audioSource != null && _clickSound != null)
            {
                _audioSource.PlayOneShot(_clickSound);
            }

            // Phát sự kiện toàn cục để các hệ thống khác biết
            GameEvents.TriggerFlashlightToggled(_isOn);
        }
    }
}

