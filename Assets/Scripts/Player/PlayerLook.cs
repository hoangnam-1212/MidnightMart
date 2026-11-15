using UnityEngine;
using UnityEngine.InputSystem;
using MidnightMart.Core;

namespace MidnightMart.Player
{
    /// <summary>
    /// Điều khiển góc nhìn camera góc nhìn thứ nhất (First-Person Look) bằng New Input System.
    /// Tích hợp cơ chế triệt tiêu giật camera và hỗ trợ nảy giật khi bắn súng (Procedural Recoil).
    /// Tuân thủ quy chuẩn Zero GC và chú thích đầy đủ theo luật Enterprise.
    /// </summary>
    public class PlayerLook : MonoBehaviour
    {
        [Header("Tham chiếu Component")]
        [Tooltip("Transform của Camera chính gắn trên đầu nhân vật")]
        [SerializeField] private Transform _cameraTransform;

        [Header("Cấu hình Độ nhạy")]
        [Tooltip("Độ nhạy xoay ngang")]
        [SerializeField] private float _mouseSensitivityX = 0.15f;
        [Tooltip("Độ nhạy xoay dọc")]
        [SerializeField] private float _mouseSensitivityY = 0.15f;

        [Header("Giới hạn góc nhìn dọc (Pitch Clamp)")]
        [SerializeField] private float _minPitchAngle = -85f;
        [SerializeField] private float _maxPitchAngle = 85f;

        [Header("Giới hạn góc quay ngang (Yaw Clamp - tùy chọn khi lái xe)")]
        [SerializeField] private bool _clampYaw = false;
        [SerializeField] private float _minYaw = -75f;
        [SerializeField] private float _maxYaw = 75f;

        // Lưu trữ giá trị góc quay hiện tại
        private float _currentPitch; // Xoay quanh trục X (Nhìn lên/xuống)
        private float _currentYaw;   // Xoay quanh trục Y (Xoay người trái/phải)

        // Offset nảy giật từ súng (Recoil)
        private Vector3 _currentRecoilOffset;
        private Vector3 _targetRecoilOffset;
        private const float RECOIL_RETURN_SPEED = 8f;

        // Giá trị đọc từ New Input System
        private Vector2 _lookInput;
        private bool _isDialogueOpen;

        /// <summary>
        /// Khởi tạo trạng thái và khóa con trỏ chuột vào giữa màn hình.
        /// </summary>
        private void Awake()
        {
            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }

            // Khóa chuột để người chơi xoay camera tự nhiên
            SetCursorLock(true);
        }

        private void Start()
        {
            // Khởi tạo góc quay ban đầu theo góc đặt nhân vật trên Scene
            _currentYaw = transform.eulerAngles.y;
            if (_cameraTransform != null)
            {
                _currentPitch = _cameraTransform.localEulerAngles.x;
                if (_currentPitch > 180f) _currentPitch -= 360f;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnShotgunFired += HandleShotgunRecoil;
            GameEvents.OnDialogueStateChanged += HandleDialogueStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnShotgunFired -= HandleShotgunRecoil;
            GameEvents.OnDialogueStateChanged -= HandleDialogueStateChanged;
        }

        private void HandleDialogueStateChanged(bool isOpen)
        {
            _isDialogueOpen = isOpen;
            _lookInput = Vector2.zero;
            // Tuyệt đối không tự ý mở khóa chuột khi có thoại để tránh gây khó chịu và giật lag tầm nhìn FPS.
            // Chuột chỉ được mở khóa bởi các giao diện toàn màn hình có tương tác click (như Bảng Cài Đặt hoặc Máy Tính Quầy).
        }

        private void Update()
        {
            // Chỉ tạm dừng xoay góc nhìn FPS khi con trỏ chuột đang mở khóa (ở trong menu Pause hoặc màn hình máy tính)
            if (Cursor.lockState != CursorLockMode.Locked) return;

            // Tự động khóa chuột lại khi người chơi click vào cửa sổ Game
            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
                {
                    SetCursorLock(true);
                }

                // Đọc delta chuột trực tiếp từ New Input System nếu không qua PlayerInput
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                if (mouseDelta.sqrMagnitude > 0.001f || _lookInput == Vector2.zero)
                {
                    _lookInput = mouseDelta;
                }
            }
        }

        /// <summary>
        /// Callback nhận giá trị từ New Input System (Action: Look).
        /// </summary>
        public void OnLook(InputValue value)
        {
            _lookInput = value.Get<Vector2>();
        }

        /// <summary>
        /// Xử lý xoay góc nhìn trong LateUpdate để đảm bảo mượt mà sau khi nhân vật đã di chuyển xong.
        /// </summary>
        private void LateUpdate()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            // 1. Phục hồi góc giật nảy của súng dần về 0 (Slerp/Lerp mượt mà)
            _targetRecoilOffset = Vector3.Lerp(_targetRecoilOffset, Vector3.zero, Time.deltaTime * RECOIL_RETURN_SPEED);
            _currentRecoilOffset = Vector3.Lerp(_currentRecoilOffset, _targetRecoilOffset, Time.deltaTime * 20f);

            // 2. Tính toán lượng xoay từ chuột
            float mouseX = _lookInput.x * _mouseSensitivityX;
            float mouseY = _lookInput.y * _mouseSensitivityY;

            _currentYaw += mouseX;
            _currentPitch -= mouseY;

            // 3. Khóa góc nhìn dọc để không bị lật ngược đầu
            _currentPitch = Mathf.Clamp(_currentPitch, _minPitchAngle, _maxPitchAngle);

            // Khóa góc quay ngang nếu đang bật chế độ lái xe (Yaw Clamp)
            if (_clampYaw)
            {
                _currentYaw = Mathf.Clamp(_currentYaw, _minYaw, _maxYaw);
            }

            // 4. Áp dụng xoay người nhân vật (trục Y)
            transform.localRotation = Quaternion.Euler(0f, _currentYaw + _currentRecoilOffset.y, 0f);

            // 5. Áp dụng xoay đầu camera lên/xuống (trục X)
            if (_cameraTransform != null)
            {
                _cameraTransform.localRotation = Quaternion.Euler(_currentPitch + _currentRecoilOffset.x, 0f, 0f);
            }
        }

        /// <summary>
        /// Nhận lực giật từ sự kiện bắn súng để nảy camera lên trên.
        /// </summary>
        private void HandleShotgunRecoil(int remainingAmmo, Vector3 recoil)
        {
            _targetRecoilOffset += recoil;
        }

        /// <summary>
        /// Đặt hoặc hủy khóa góc quay ngang (ví dụ khi ngồi trên xe máy).
        /// </summary>
        public void SetYawClamp(bool isClamped, float minYaw = -75f, float maxYaw = 75f)
        {
            _clampYaw = isClamped;
            _minYaw = minYaw;
            _maxYaw = maxYaw;
            if (_clampYaw)
            {
                _currentYaw = Mathf.Clamp(_currentYaw, _minYaw, _maxYaw);
            }
        }

        /// <summary>
        /// Đặt lại hướng nhìn của camera và nhân vật về một góc cụ thể.
        /// </summary>
        public void ResetOrientation(float yaw, float pitch)
        {
            _currentYaw = yaw;
            _currentPitch = pitch;
            transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
            if (_cameraTransform != null)
            {
                _cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }
        }

        /// <summary>
        /// Khóa hoặc mở khóa con trỏ chuột khi cần tương tác menu.
        /// </summary>
        public static void SetCursorLock(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }
    }
}

