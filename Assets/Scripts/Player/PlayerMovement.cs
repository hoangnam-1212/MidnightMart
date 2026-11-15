using UnityEngine;
using UnityEngine.InputSystem;

namespace MidnightMart.Player
{
    /// <summary>
    /// Điều khiển di chuyển nhân vật góc nhìn thứ nhất (First-Person Movement).
    /// Hỗ trợ: Đi bộ, Chạy nhanh (Sprint), Ngồi cúi (Crouch) và Hiệu ứng nhún chân (Head Bobbing).
    /// Tối ưu hóa hiệu năng: Cache toàn bộ biến Vector, không cấp phát rác (Zero GC) trong Update.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Thông số Tốc độ")]
        [SerializeField] private float _walkSpeed = 3.5f;
        [SerializeField] private float _sprintSpeed = 5.5f;
        [SerializeField] private float _crouchSpeed = 2.0f;
        [SerializeField] private float _gravity = -15f;

        [Header("Thông số Nhảy (Jump)")]
        [SerializeField] private float _jumpHeight = 1.25f;

        [Header("Cấu hình Ngồi (Crouch)")]
        [SerializeField] private float _standingHeight = 1.8f;
        [SerializeField] private float _crouchingHeight = 1.0f;
        [SerializeField] private float _crouchTransitionSpeed = 10f;
        [SerializeField] private Transform _cameraContainer;

        [Header("Hiệu ứng Nhún chân (Head Bobbing)")]
        [SerializeField] private bool _enableHeadBob = true;
        [SerializeField] private float _bobFrequency = 7.5f;
        [SerializeField] private float _bobAmount = 0.04f;

        // Components & Trạng thái
        private CharacterController _characterController;
        private Vector2 _moveInput;
        private Vector3 _velocity;
        private bool _isSprinting;
        private bool _isCrouching;
        private float _targetHeight;
        private float _defaultCameraY;
        private float _bobTimer;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _characterController.height = _standingHeight;
            _targetHeight = _standingHeight;

            if (_cameraContainer != null)
            {
                _defaultCameraY = _cameraContainer.localPosition.y;
            }
        }

        /// <summary>
        /// Callback từ New Input System cho phím W, A, S, D.
        /// </summary>
        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }

        /// <summary>
        /// Callback từ New Input System cho phím Shift (Chạy nhanh).
        /// </summary>
        public void OnSprint(InputValue value)
        {
            _isSprinting = value.isPressed;
        }

        /// <summary>
        /// Callback từ New Input System cho phím C hoặc Ctrl (Ngồi).
        /// </summary>
        public void OnCrouch(InputValue value)
        {
            if (value.isPressed)
            {
                _isCrouching = !_isCrouching;
                _targetHeight = _isCrouching ? _crouchingHeight : _standingHeight;
            }
        }

        /// <summary>
        /// Callback từ New Input System cho phím Space (Nhảy).
        /// </summary>
        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                Jump();
            }
        }

        /// <summary>
        /// Thực hiện hành động nhảy khi nhân vật đang tiếp xúc mặt đất.
        /// </summary>
        public void Jump()
        {
            if (_characterController != null && _characterController.isGrounded && !_isCrouching)
            {
                _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
        }

        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                _moveInput = Vector2.zero;
                return;
            }

            PollKeyboardFallback();
            HandleMovement();
            HandleCrouchHeight();
            HandleHeadBob();
        }

        /// <summary>
        /// Đọc phím trực tiếp từ New Input System Keyboard làm fallback bảo đảm 100% nhân vật luôn di chuyển được.
        /// Tự động reset về Vector2.zero khi nhả phím hoặc khi cửa sổ mất tiêu điểm (Lost Focus).
        /// </summary>
        private void PollKeyboardFallback()
        {
            // Tránh nhân vật tự động trôi khi mất tiêu điểm cửa sổ game
            if (!Application.isFocused)
            {
                _moveInput = Vector2.zero;
                return;
            }

            if (Keyboard.current != null)
            {
                Vector2 keyInput = Vector2.zero;
                if (Keyboard.current.wKey.isPressed) keyInput.y += 1f;
                if (Keyboard.current.sKey.isPressed) keyInput.y -= 1f;
                if (Keyboard.current.aKey.isPressed) keyInput.x -= 1f;
                if (Keyboard.current.dKey.isPressed) keyInput.x += 1f;

                // Cập nhật hướng đi từ bàn phím. Khi nhả phím (keyInput == 0) thì dừng lại ngay lập tức
                _moveInput = keyInput.sqrMagnitude > 0.01f ? keyInput.normalized : Vector2.zero;

                _isSprinting = Keyboard.current.leftShiftKey.isPressed;

                if (Keyboard.current.cKey.wasPressedThisFrame || Keyboard.current.leftCtrlKey.wasPressedThisFrame)
                {
                    _isCrouching = !_isCrouching;
                    _targetHeight = _isCrouching ? _crouchingHeight : _standingHeight;
                }

                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    Jump();
                }
            }
        }

        /// <summary>
        /// Tính toán hướng và tốc độ di chuyển theo hướng nhìn của nhân vật.
        /// </summary>
        private void HandleMovement()
        {
            // 1. Kiểm tra tiếp đất
            if (_characterController.isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f; // Giữ áp lực nhẹ để bám dính mặt đất
            }

            // 2. Xác định tốc độ hiện tại dựa trên trạng thái
            float targetSpeed = _walkSpeed;
            if (_isCrouching)
            {
                targetSpeed = _crouchSpeed;
            }
            else if (_isSprinting && _moveInput.y > 0.1f) // Chỉ chạy nhanh khi tiến tới
            {
                targetSpeed = _sprintSpeed;
            }

            // 3. Tính toán Vector di chuyển cục bộ sang thế giới (Zero allocation)
            // Chỉ thực hiện di chuyển ngang khi có tín hiệu điều khiển (_moveInput != Vector2.zero)
            if (_moveInput.sqrMagnitude > 0.001f)
            {
                Vector3 forward = transform.forward;
                Vector3 right = transform.right;

                Vector3 moveDirection = (right * _moveInput.x + forward * _moveInput.y).normalized;
                _characterController.Move(moveDirection * (targetSpeed * Time.deltaTime));
            }

            // 4. Áp dụng trọng lực
            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        /// <summary>
        /// Chuyển đổi chiều cao CharacterController khi ngồi xuống / đứng lên một cách mượt mà.
        /// </summary>
        private void HandleCrouchHeight()
        {
            if (Mathf.Abs(_characterController.height - _targetHeight) > 0.01f)
            {
                _characterController.height = Mathf.Lerp(_characterController.height, _targetHeight, Time.deltaTime * _crouchTransitionSpeed);
                _characterController.center = new Vector3(0f, _characterController.height * 0.5f, 0f);
            }
        }

        /// <summary>
        /// Tạo cảm giác bước chân lắc nhẹ tự nhiên khi di chuyển (Head Bobbing).
        /// </summary>
        private void HandleHeadBob()
        {
            if (!_enableHeadBob || _cameraContainer == null) return;

            // Kiểm tra nhân vật có đang thực sự di chuyển trên mặt đất không
            if (_characterController.isGrounded && _moveInput.sqrMagnitude > 0.01f)
            {
                _bobTimer += Time.deltaTime * _bobFrequency * (_isSprinting ? 1.4f : 1.0f);
                float newY = _defaultCameraY + Mathf.Sin(_bobTimer) * _bobAmount;
                Vector3 currentPos = _cameraContainer.localPosition;
                currentPos.y = newY;
                _cameraContainer.localPosition = currentPos;
            }
            else
            {
                // Trả camera về vị trí cân bằng
                _bobTimer = 0f;
                Vector3 currentPos = _cameraContainer.localPosition;
                currentPos.y = Mathf.Lerp(currentPos.y, _defaultCameraY, Time.deltaTime * 6f);
                _cameraContainer.localPosition = currentPos;
            }
        }
    }
}

