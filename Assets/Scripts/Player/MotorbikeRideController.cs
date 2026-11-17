using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MidnightMart.Core;

namespace MidnightMart.Player
{
    /// <summary>
    /// Điều khiển Chuyến xe máy đêm mưa bão xuyên qua rừng Blackwood (Motorbike Night Ride Controller).
    /// Đảm bảo trải nghiệm điện ảnh và dễ chơi (Arcade / On-Rails Assist):
    /// - Giữ [W] để tăng tốc chạy về phía siêu thị Midnight Mart.
    /// - [A][D] để lượn xe nhẹ nhàng, giới hạn an toàn trong lòng đường nhựa (không bao giờ lật xe hay kẹt cây).
    /// - Tự do lia chuột nhìn ngắm khung cảnh rừng cây âm u, biển báo rỉ sét và xe cảnh sát bỏ hoang.
    /// - Khi đến gần siêu thị (Z ~ -8m), xe tự động tấp vào lề đường, tắt máy và đưa người chơi sang trạng thái đi bộ.
    /// </summary>
    public class MotorbikeRideController : MonoBehaviour
    {
        [Header("Thông số Tốc độ")]
        [SerializeField] private float _maxSpeed = 11.0f;
        [SerializeField] private float _acceleration = 6.0f;
        [SerializeField] private float _brakeSpeed = 9.0f;
        [SerializeField] private float _steeringSpeed = 4.0f;
        [SerializeField] private float _roadWidthLimit = 2.4f;

        [Header("Vị trí & Điểm đến")]
        [SerializeField] private float _arrivalZ = -8.5f;
        [SerializeField] private Vector3 _parkedPosition = new Vector3(-2.2f, 0.1f, -7.5f);
        [SerializeField] private float _returnArrivalZ = -276.0f;
        [SerializeField] private Vector3 _homeParkedPosition = new Vector3(0.5f, 0.08f, -282.0f);

        [Header("Thành phần Thị giác & Âm thanh")]
        [SerializeField] private GameObject _bikeHandlebarsFPS;
        [SerializeField] private Light _bikeHeadlight;
        [SerializeField] private GameObject _parkedBikeWorldModel;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _engineIdleClip;
        [SerializeField] private AudioClip _engineRevClip;

        private bool _isRiding = false;
        private bool _isReturnTrip = false;
        private float _currentSpeed = 0f;
        private float _steerOffsetFromCenter = 0f;
        private CharacterController _characterController;
        private PlayerMovement _playerMovement;
        private PlayerInteraction _playerInteraction;

        public bool IsRiding => _isRiding;
        public bool IsReturnTrip => _isReturnTrip;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerMovement = GetComponent<PlayerMovement>();
            _playerInteraction = GetComponent<PlayerInteraction>();

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
            _audioSource.loop = true;

            if (_bikeHandlebarsFPS != null) _bikeHandlebarsFPS.SetActive(false);
            if (_bikeHeadlight != null) _bikeHeadlight.enabled = false;

#if UNITY_EDITOR
            if (_engineRevClip == null)
            {
                _engineRevClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Motorbike_Engine_Loop.wav");
            }
#endif
            if (_engineRevClip == null)
            {
                _engineRevClip = ProceduralAudioGenerator.CreateMotorbikeEngineClip();
            }
        }

        /// <summary>
        /// Bắt đầu phân đoạn lái xe máy dọc con đường âm u tới siêu thị Midnight Mart.
        /// </summary>
        public void StartMotorbikeRide(Vector3 startPosition)
        {
            _isRiding = true;
            _isReturnTrip = false;
            _currentSpeed = 0f;
            _steerOffsetFromCenter = 0f;

            // Tạm thời vô hiệu hóa bộ điều khiển đi bộ thông thường
            if (_characterController != null) _characterController.enabled = false;
            if (_playerMovement != null) _playerMovement.enabled = false;

            // Căn chỉnh góc nhìn camera thẳng về phía trước theo hướng đường đi và khóa góc quay ngang
            PlayerLook look = GetComponent<PlayerLook>();
            if (look != null)
            {
                look.ResetOrientation(0f, 0f);
                look.SetYawClamp(true, -75f, 75f);
            }

            transform.position = startPosition;
            float initialAngle = RoadCurveUtility.GetRoadTangentAngle(startPosition.z);
            transform.rotation = Quaternion.Euler(0f, initialAngle, 0f);

            if (_bikeHandlebarsFPS != null) _bikeHandlebarsFPS.SetActive(true);
            if (_bikeHeadlight != null)
            {
                _bikeHeadlight.enabled = true;
                _bikeHeadlight.intensity = 600f;
                _bikeHeadlight.spotAngle = 36f;
                _bikeHeadlight.innerSpotAngle = 18f;
                _bikeHeadlight.range = 32f;
            }

            if (_parkedBikeWorldModel != null)
            {
                _parkedBikeWorldModel.SetActive(false);
            }

            GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Lái xe máy (Giữ [W]) qua rừng Blackwood đến siêu thị");
            GameEvents.TriggerSubtitle("Độc thoại", "Đoạn đường đêm vắng ngắt... Rừng Blackwood đêm nay lạnh buốt bất thường.", 4.5f);

            // Bắt đầu âm thanh máy nổ
            PlayEngineSound();
        }

        /// <summary>
        /// Bắt đầu phân đoạn lái xe máy sau khi kết thúc ca trực, trở về căn phòng trọ an toàn.
        /// </summary>
        public void StartMotorbikeRideReturnHome(Vector3 startPosition)
        {
            _isRiding = true;
            _isReturnTrip = true;
            _currentSpeed = 0f;
            _steerOffsetFromCenter = 0f;

            if (_characterController != null) _characterController.enabled = false;
            if (_playerMovement != null) _playerMovement.enabled = false;

            PlayerLook look = GetComponent<PlayerLook>();
            if (look != null)
            {
                look.ResetOrientation(180f, 0f);
                look.SetYawClamp(true, 105f, 255f);
            }

            transform.position = startPosition;
            float initialAngle = 180f - RoadCurveUtility.GetRoadTangentAngle(startPosition.z);
            transform.rotation = Quaternion.Euler(0f, initialAngle, 0f);

            if (_bikeHandlebarsFPS != null) _bikeHandlebarsFPS.SetActive(true);
            if (_bikeHeadlight != null)
            {
                _bikeHeadlight.enabled = true;
                _bikeHeadlight.intensity = 600f;
                _bikeHeadlight.spotAngle = 36f;
                _bikeHeadlight.innerSpotAngle = 18f;
                _bikeHeadlight.range = 32f;
            }

            if (_parkedBikeWorldModel != null)
            {
                _parkedBikeWorldModel.SetActive(false);
            }

            GameEvents.TriggerReturnHomeRideStarted();
            GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Lái xe máy (Giữ [W]) trở về phòng trọ an toàn");
            GameEvents.TriggerSubtitle("Độc thoại", "Ca trực dài đằng đẵng cuối cùng cũng xong... Phải mau lái xe về phòng trọ nghỉ ngơi.", 4.5f);

            PlayEngineSound();
        }

        private void Update()
        {
            if (!_isRiding) return;

            HandleDriving();
            CheckArrival();
        }

        private void HandleDriving()
        {
            bool throttle = false;
            float steerInput = 0f;

            if (Keyboard.current != null)
            {
                throttle = Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) steerInput -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) steerInput += 1f;
            }

            // Tăng tốc hoặc giảm tốc
            if (throttle)
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, _maxSpeed, _acceleration * Time.deltaTime);
            }
            else
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, 1.5f, _brakeSpeed * Time.deltaTime);
            }

            Vector3 pos = transform.position;

            if (!_isReturnTrip)
            {
                // Chuyến đi: Di chuyển tiến dọc theo trục Z (tiến về 0m)
                pos.z += _currentSpeed * Time.deltaTime;

                float roadCenterX = RoadCurveUtility.GetRoadCenterX(pos.z);
                float roadTangentAngle = RoadCurveUtility.GetRoadTangentAngle(pos.z);

                _steerOffsetFromCenter += steerInput * _steeringSpeed * Time.deltaTime;
                _steerOffsetFromCenter = Mathf.Clamp(_steerOffsetFromCenter, -_roadWidthLimit, _roadWidthLimit);

                pos.x = roadCenterX + _steerOffsetFromCenter;
                pos.y = 1.0f;
                transform.position = pos;

                float targetYaw = roadTangentAngle + steerInput * 5.0f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, targetYaw, 0f), Time.deltaTime * 6f);

                if (_bikeHandlebarsFPS != null)
                {
                    float targetRoll = -steerInput * 10f - (roadTangentAngle * 0.45f);
                    _bikeHandlebarsFPS.transform.localRotation = Quaternion.Lerp(_bikeHandlebarsFPS.transform.localRotation, Quaternion.Euler(0f, 0f, targetRoll), Time.deltaTime * 6f);
                }
            }
            else
            {
                // Chuyến về: Di chuyển lùi dọc theo trục Z (tiến về -280m)
                pos.z -= _currentSpeed * Time.deltaTime;

                float roadCenterX = RoadCurveUtility.GetRoadCenterX(pos.z);
                float roadTangentAngle = RoadCurveUtility.GetRoadTangentAngle(pos.z);

                // Khi chạy ngược chiều, đánh lái A/D điều khiển offset ngược lại
                _steerOffsetFromCenter -= steerInput * _steeringSpeed * Time.deltaTime;
                _steerOffsetFromCenter = Mathf.Clamp(_steerOffsetFromCenter, -_roadWidthLimit, _roadWidthLimit);

                pos.x = roadCenterX + _steerOffsetFromCenter;
                pos.y = 1.0f;
                transform.position = pos;

                float targetYaw = 180f - roadTangentAngle - steerInput * 5.0f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, targetYaw, 0f), Time.deltaTime * 6f);

                if (_bikeHandlebarsFPS != null)
                {
                    float targetRoll = -steerInput * 10f + (roadTangentAngle * 0.45f);
                    _bikeHandlebarsFPS.transform.localRotation = Quaternion.Lerp(_bikeHandlebarsFPS.transform.localRotation, Quaternion.Euler(0f, 0f, targetRoll), Time.deltaTime * 6f);
                }
            }

            // Điều chỉnh cao độ âm thanh động cơ theo tốc độ
            if (_audioSource != null && _audioSource.isPlaying)
            {
                _audioSource.pitch = Mathf.Lerp(0.85f, 1.45f, _currentSpeed / _maxSpeed);
            }
        }

        private void CheckArrival()
        {
            if (!_isReturnTrip)
            {
                // Khi xe đến gần khu vực phía trước siêu thị
                if (transform.position.z >= _arrivalZ)
                {
                    _isRiding = false;
                    StartCoroutine(DismountSequenceRoutine());
                }
            }
            else
            {
                // Khi xe về đến gần phòng trọ (Z <= _returnArrivalZ)
                if (transform.position.z <= _returnArrivalZ)
                {
                    _isRiding = false;
                    StartCoroutine(ReturnHomeDismountSequenceRoutine());
                }
            }
        }

        private IEnumerator DismountSequenceRoutine()
        {
            float startZ = transform.position.z;
            float startX = transform.position.x;
            float elapsed = 0f;

            while (elapsed < 1.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 1.2f;
                float currentZ = Mathf.Lerp(startZ, _parkedPosition.z, t);
                float currentX = Mathf.Lerp(startX, _parkedPosition.x, t);
                transform.position = new Vector3(currentX, 1.0f, currentZ);
                yield return null;
            }

            transform.position = _parkedPosition;

            if (_audioSource != null) _audioSource.Stop();
            if (_bikeHandlebarsFPS != null) _bikeHandlebarsFPS.SetActive(false);
            if (_bikeHeadlight != null) _bikeHeadlight.enabled = false;

            if (_parkedBikeWorldModel != null)
            {
                _parkedBikeWorldModel.SetActive(true);
                _parkedBikeWorldModel.transform.position = new Vector3(_parkedPosition.x, 0.05f, _parkedPosition.z);
                _parkedBikeWorldModel.transform.rotation = Quaternion.Euler(0f, 25f, 0f);
            }

            if (_characterController != null) _characterController.enabled = true;
            if (_playerMovement != null) _playerMovement.enabled = true;

            PlayerLook look = GetComponent<PlayerLook>();
            if (look != null)
            {
                look.SetYawClamp(false);
            }

            GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Đọc Bản Ghi Nhớ Ca Đêm của Quản Lý dán trên tường cạnh cửa");
            GameEvents.TriggerSubtitle("Độc thoại", "Đã đến siêu thị Midnight Mart... Có tờ giấy note của Quản lý dán ngay trên tường.", 4.0f);
        }

        private IEnumerator ReturnHomeDismountSequenceRoutine()
        {
            float startZ = transform.position.z;
            float startX = transform.position.x;
            float elapsed = 0f;

            while (elapsed < 1.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 1.2f;
                float currentZ = Mathf.Lerp(startZ, _homeParkedPosition.z, t);
                float currentX = Mathf.Lerp(startX, _homeParkedPosition.x, t);
                transform.position = new Vector3(currentX, 1.0f, currentZ);
                yield return null;
            }

            transform.position = _homeParkedPosition;

            if (_audioSource != null) _audioSource.Stop();
            if (_bikeHandlebarsFPS != null) _bikeHandlebarsFPS.SetActive(false);
            if (_bikeHeadlight != null) _bikeHeadlight.enabled = false;

            if (_characterController != null) _characterController.enabled = true;
            if (_playerMovement != null) _playerMovement.enabled = true;

            PlayerLook look = GetComponent<PlayerLook>();
            if (look != null)
            {
                look.SetYawClamp(false);
                look.ResetOrientation(0f, 0f);
            }

            // Kích hoạt thông báo trở về nhà cho PlayerHomeManager
            if (MidnightMart.Prologue.PlayerHomeManager.Instance != null)
            {
                MidnightMart.Prologue.PlayerHomeManager.Instance.HandleBikeArrivalAtHome();
            }

            GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Bước vào phòng trọ và [E] Lên giường đi ngủ để qua Ngày 2");
            GameEvents.TriggerSubtitle("Độc thoại", "Cuối cùng cũng về tới phòng trọ an toàn... Mau lên giường ngủ một giấc.", 4.0f);
        }

        private void PlayEngineSound()
        {
            if (_audioSource == null) return;
            if (_engineRevClip != null)
            {
                _audioSource.clip = _engineRevClip;
                _audioSource.Play();
            }
        }
    }
}
