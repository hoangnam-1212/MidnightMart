using System.Collections;
using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Tutorial;
using MidnightMart.Customer;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Cửa kính trượt tự động cảm ứng 2 cánh của siêu thị Midnight Mart (Storefront Automatic Sensor Doors).
    /// Khi Người chơi hoặc Khách hàng lại gần (trong phạm vi ~3m):
    /// - Hai cánh cửa kính tự động trượt/dạt sang hai bên trái & phải ("quấn ra 2 bên").
    /// - Chuông điện tử "LENG KENG!" cơ khí vang lên chào đón.
    /// - Khi người chơi lần đầu bước tới, tự động cập nhật giờ 00:00 AM và kích hoạt ca trực.
    /// - Khi người chơi hoặc khách đi xa khỏi cửa, 2 cánh cửa tự động khép lại giữa.
    /// Hoạt động hoàn toàn tự động, Zero GC, không cần phím bấm hay cửa kéo cơ học.
    /// </summary>
    public class StoreAutomaticDoors : MonoBehaviour
    {
        [Header("Hai Cánh Cửa Kính")]
        [SerializeField] private Transform _leftDoorPanel;
        [SerializeField] private Transform _rightDoorPanel;

        [Header("Thông số Trượt Tự Động")]
        [SerializeField] private float _slideDistance = 1.15f;
        [SerializeField] private float _slideSpeed = 3.5f;
        [SerializeField] private float _sensorDistance = 3.2f;

        [Header("Âm thanh & Quản lý Ca trực")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _doorChimeClip;
        [SerializeField] private TutorialManager _tutorialManager;

        [Header("Khóa Cửa Khi Chưa Đọc Note")]
        [SerializeField] private bool _isLocked = true;

        private Vector3 _leftClosedLocalPos;
        private Vector3 _rightClosedLocalPos;
        private Vector3 _leftOpenLocalPos;
        private Vector3 _rightOpenLocalPos;

        private bool _isOpen = false;
        private bool _hasTriggeredFirstArrival = false;
        private Transform _playerTransform;

        public bool IsOpen => _isOpen;
        public bool IsLocked => _isLocked;

        public void UnlockDoors()
        {
            _isLocked = false;
            OpenDoors();
        }

        public void SetLocked(bool locked)
        {
            _isLocked = locked;
            if (_isLocked && _isOpen)
            {
                CloseDoors();
            }
        }

        private void Awake()
        {
            if (_leftDoorPanel != null)
            {
                _leftClosedLocalPos = _leftDoorPanel.localPosition;
                _leftOpenLocalPos = _leftClosedLocalPos + new Vector3(-_slideDistance, 0f, 0f);
            }

            if (_rightDoorPanel != null)
            {
                _rightClosedLocalPos = _rightDoorPanel.localPosition;
                _rightOpenLocalPos = _rightClosedLocalPos + new Vector3(_slideDistance, 0f, 0f);
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0.85f;

#if UNITY_EDITOR
            if (_doorChimeClip == null)
            {
                _doorChimeClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Door_Chime_DingDong.wav");
            }
#endif
            if (_doorChimeClip == null)
            {
                _doorChimeClip = ProceduralAudioGenerator.CreateDoorChimeClip();
            }
        }

        private void Update()
        {
            bool shouldOpen = CheckProximitySensor();

            if (shouldOpen)
            {
                if (!_isOpen)
                {
                    OpenDoors();
                }
            }
            else
            {
                if (_isOpen)
                {
                    CloseDoors();
                }
            }

            // Di chuyển mượt mà 2 cánh cửa
            Vector3 targetLeft = _isOpen ? _leftOpenLocalPos : _leftClosedLocalPos;
            Vector3 targetRight = _isOpen ? _rightOpenLocalPos : _rightClosedLocalPos;

            if (_leftDoorPanel != null)
            {
                _leftDoorPanel.localPosition = Vector3.MoveTowards(_leftDoorPanel.localPosition, targetLeft, _slideSpeed * Time.deltaTime);
            }
            if (_rightDoorPanel != null)
            {
                _rightDoorPanel.localPosition = Vector3.MoveTowards(_rightDoorPanel.localPosition, targetRight, _slideSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Cảm biến tiệm cận: Kiểm tra Người chơi hoặc Khách hàng có đang đứng gần cửa hay không.
        /// </summary>
        private bool CheckProximitySensor()
        {
            Vector3 doorWorldCenter = transform.position;

            // 1. Kiểm tra Khách hàng NPC (Luôn mở cửa cho khách hàng bước vào/ra tiệm bất kể trạng thái khóa)
            for (int i = 0; i < CustomerNPC.ActiveCustomers.Count; i++)
            {
                CustomerNPC npc = CustomerNPC.ActiveCustomers[i];
                if (npc != null && npc.gameObject.activeInHierarchy && !npc.IsDead)
                {
                    float distSq = (npc.transform.position - doorWorldCenter).sqrMagnitude;
                    if (distSq <= (_sensorDistance + 0.8f) * (_sensorDistance + 0.8f))
                    {
                        if (_isLocked) _isLocked = false;
                        return true;
                    }
                }
            }

            if (_isLocked) return false;

            // 2. Kiểm tra Người chơi
            if (_playerTransform == null)
            {
                if (Camera.main != null) _playerTransform = Camera.main.transform;
            }

            if (_playerTransform != null)
            {
                Vector3 playerPos = _playerTransform.position;
                float distSq = (playerPos - doorWorldCenter).sqrMagnitude;
                if (distSq <= _sensorDistance * _sensorDistance)
                {
                    return true;
                }
            }

            return false;
        }

        private void OpenDoors()
        {
            _isOpen = true;

            // Tiếng chuông leng keng chào đón
            if (_audioSource != null && _doorChimeClip != null)
            {
                _audioSource.PlayOneShot(_doorChimeClip);
            }

            // Kích hoạt ca trực lần đầu khi người chơi đến cửa tiệm
            if (!_hasTriggeredFirstArrival)
            {
                _hasTriggeredFirstArrival = true;

                GameEvents.TriggerTimeUpdated("00:00 AM");
                GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Dọn dẹp đồ rơi vãi trên sàn (0/3)");
                GameEvents.TriggerSubtitle("Cửa hàng Midnight Mart", "LENG KENG! Cửa cảm ứng tự động mở ra hai bên. Bạn bước vào tiệm Midnight Mart vắng vẻ, đồ đạc đang vương vãi trên sàn.", 5.0f);

                if (_tutorialManager != null)
                {
                    _tutorialManager.gameObject.SetActive(true);
                }
                else if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.gameObject.SetActive(true);
                }
            }
        }

        private void CloseDoors()
        {
            _isOpen = false;
        }
    }
}

