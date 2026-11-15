using System.Collections;
using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Tutorial;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Cánh cửa kính mặt tiền của siêu thị Midnight Mart (Storefront Entrance Door).
    /// Yêu cầu người chơi sau khi đi xe máy tới nơi phải đi bộ lại gần và nhấn [E] để đẩy cửa bước vào.
    /// Kích hoạt tiếng chuông đồng "LENG KENG!" cơ khí, mở cánh cửa ra và bắt đầu ca trực!
    /// </summary>
    public class StoreEntranceDoor : MonoBehaviour, IInteractable
    {
        [Header("Cấu hình Cánh cửa")]
        [SerializeField] private Transform _doorHinge;
        [SerializeField] private float _openAngle = -75f;
        [SerializeField] private float _openSpeed = 3.5f;

        [Header("Âm thanh Chuông cửa")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _doorBellChime;

        [Header("Tham chiếu Quản lý")]
        [SerializeField] private TutorialManager _tutorialManager;

        private bool _isOpen;
        private Quaternion _closedRotation;
        private Quaternion _targetRotation;
        private InteractionHighlighter _highlighter;

        public bool IsOpen => _isOpen;

        private void Awake()
        {
            if (_doorHinge == null) _doorHinge = transform;
            _closedRotation = _doorHinge.localRotation;
            _targetRotation = _closedRotation;

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
            _audioSource.spatialBlend = 0.8f;

#if UNITY_EDITOR
            if (_doorBellChime == null)
            {
                _doorBellChime = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Door_Chime_DingDong.wav");
            }
#endif
            if (_doorBellChime == null)
            {
                _doorBellChime = ProceduralAudioGenerator.CreateDoorChimeClip();
            }
        }

        public string GetInteractionPrompt()
        {
            if (!_isOpen)
            {
                return "[E] Đẩy cửa bước vào tiệm";
            }
            return string.Empty;
        }

        public bool CanInteract()
        {
            return !_isOpen;
        }

        public void SetHighlight(bool isHighlighted)
        {
            _highlighter?.SetHighlight(isHighlighted);
        }

        public void OnInteract(Transform playerTransform)
        {
            if (_isOpen) return;
            OpenDoorAndEnter();
        }

        public void OpenDoorAndEnter()
        {
            _isOpen = true;
            _targetRotation = _closedRotation * Quaternion.Euler(0f, _openAngle, 0f);

            // Tiếng chuông leng keng vui tai đón khách
            if (_audioSource != null && _doorBellChime != null)
            {
                _audioSource.PlayOneShot(_doorBellChime);
            }

            // Mở cửa mượt mà
            StartCoroutine(AnimateDoorOpen());

            // Thông báo bắt đầu ca trực
            GameEvents.TriggerTimeUpdated("00:00 AM");
            GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Dọn dẹp đồ rơi vãi trên sàn (0/3)");
            GameEvents.TriggerSubtitle("Cửa hàng Midnight Mart", "LENG KENG! Bạn đẩy cửa bước vào tiệm. Cửa hàng hoàn toàn vắng tanh, đồ đạc vương vãi trên sàn.", 5.0f);

            // Kích hoạt Tutorial Manager
            if (_tutorialManager != null)
            {
                _tutorialManager.gameObject.SetActive(true);
            }
            else if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.gameObject.SetActive(true);
            }
        }

        private IEnumerator AnimateDoorOpen()
        {
            float elapsed = 0f;
            Quaternion startRot = _doorHinge.localRotation;

            while (elapsed < 1.0f)
            {
                elapsed += Time.deltaTime * _openSpeed;
                _doorHinge.localRotation = Quaternion.Slerp(startRot, _targetRotation, elapsed);
                yield return null;
            }

            _doorHinge.localRotation = _targetRotation;
        }
    }
}

