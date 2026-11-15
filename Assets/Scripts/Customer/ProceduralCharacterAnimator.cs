using UnityEngine;

namespace MidnightMart.Customer
{
    /// <summary>
    /// Hệ thống Animation theo thuật toán (Procedural Character Animation).
    /// Tự động tìm kiếm các chi và bộ phận của nhân vật (Arms, Legs, Torso, Head)
    /// và mô phỏng cử động đi bộ, đung đưa tay chân, nhún bước chân (bobbing)
    /// cùng nhịp thở tự nhiên (Idle Breathing) mà không cần file animation clip phức tạp.
    /// </summary>
    public class ProceduralCharacterAnimator : MonoBehaviour
    {
        [Header("Trạng thái Hoạt ảnh")]
        [SerializeField] private bool _isWalking = false;
        [SerializeField] private float _walkSpeedMultiplier = 1.0f;

        [Header("Thông số Đi Bộ (Walk Parameters)")]
        [SerializeField] private float _walkFrequency = 6.0f;       // Tần số bước chân
        [SerializeField] private float _armSwingAngle = 24.0f;      // Góc vung tay
        [SerializeField] private float _legSwingAngle = 28.0f;      // Góc vung chân
        [SerializeField] private float _bodyBobHeight = 0.035f;     // Độ nhún bước chân
        [SerializeField] private float _bodySwayAngle = 3.5f;       // Độ lắc hông

        [Header("Thông số Đứng Chờ (Idle Parameters)")]
        [SerializeField] private float _idleBreathFrequency = 1.8f;
        [SerializeField] private float _idleBreathAmount = 0.015f;

        // Cached Transform Parts
        private Transform _armL;
        private Transform _armR;
        private Transform _legL;
        private Transform _legR;
        private Transform _head;
        private Transform _torso;
        private Transform _hips;

        private Quaternion _armLInitialRot;
        private Quaternion _armRInitialRot;
        private Quaternion _legLInitialRot;
        private Quaternion _legRInitialRot;
        private Vector3 _hipsInitialPos;
        private Quaternion _hipsInitialRot;
        private Vector3 _torsoInitialPos;
        private Quaternion _torsoInitialRot;

        public bool IsWalking
        {
            get => _isWalking;
            set => _isWalking = value;
        }

        private void Awake()
        {
            CacheBodyParts();
        }

        /// <summary>
        /// Tự động tìm kiếm các bộ phận cơ thể dựa theo tên hoặc phân cấp.
        /// </summary>
        public void CacheBodyParts()
        {
            _armL = null;
            _armR = null;
            _legL = null;
            _legR = null;
            _head = null;
            _torso = null;
            _hips = null;

            Transform[] allChildren = GetComponentsInChildren<Transform>(true);

            // Pass 1: Ưu tiên tìm chính xác các khớp gốc UpperArm, UpperLeg, Hips, Torso từ Armature FBX
            foreach (Transform t in allChildren)
            {
                string n = t.name.ToLower();
                bool isLeft = n.Contains(".l") || n.Contains("_l") || n.Contains("left") || n.EndsWith("l");
                bool isRight = n.Contains(".r") || n.Contains("_r") || n.Contains("right") || n.EndsWith("r");

                if (n.Contains("hips") || n.Contains("pelvis") || n.Contains("root"))
                {
                    if (_hips == null && !n.Contains("armature")) _hips = t;
                }
                else if (n.Contains("upperarm") || n.Contains("arm_upper") || n.Contains("shoulder"))
                {
                    if (isLeft && _armL == null) _armL = t;
                    else if (isRight && _armR == null) _armR = t;
                }
                else if (n.Contains("upperleg") || n.Contains("leg_upper") || n.Contains("thigh"))
                {
                    if (isLeft && _legL == null) _legL = t;
                    else if (isRight && _legR == null) _legR = t;
                }
                else if (n.Contains("head") || n.Contains("cranium"))
                {
                    if (_head == null) _head = t;
                }
                else if (n.Contains("chest") || n.Contains("spine") || n.Contains("torso"))
                {
                    if (_torso == null) _torso = t;
                }
            }

            // Pass 2: Fallback tìm rộng hơn nếu pass 1 chưa gán
            foreach (Transform t in allChildren)
            {
                string n = t.name.ToLower();
                bool isLeft = n.Contains(".l") || n.Contains("_l") || n.Contains("left") || n.EndsWith("l");
                bool isRight = n.Contains(".r") || n.Contains("_r") || n.Contains("right") || n.EndsWith("r");

                if (_armL == null && isLeft && (n.Contains("arm") || n.Contains("hand"))) _armL = t;
                if (_armR == null && isRight && (n.Contains("arm") || n.Contains("hand"))) _armR = t;
                if (_legL == null && isLeft && (n.Contains("leg") || n.Contains("foot"))) _legL = t;
                if (_legR == null && isRight && (n.Contains("leg") || n.Contains("foot"))) _legR = t;
                if (_hips == null && (n.Contains("hip") || n.Contains("waist"))) _hips = t;
            }

            if (_armL != null) _armLInitialRot = _armL.localRotation;
            if (_armR != null) _armRInitialRot = _armR.localRotation;
            if (_legL != null) _legLInitialRot = _legL.localRotation;
            if (_legR != null) _legRInitialRot = _legR.localRotation;
            if (_hips != null)
            {
                _hipsInitialPos = _hips.localPosition;
                _hipsInitialRot = _hips.localRotation;
            }
            if (_torso != null)
            {
                _torsoInitialPos = _torso.localPosition;
                _torsoInitialRot = _torso.localRotation;
            }

            // Nếu model có Animator rỗng chưa gán Controller thì tắt để không override cử động xương
            Animator[] animators = GetComponentsInChildren<Animator>(true);
            foreach (var anim in animators)
            {
                if (anim.runtimeAnimatorController == null)
                {
                    anim.enabled = false;
                }
            }
        }

        private void Update()
        {
            if (_isWalking)
            {
                AnimateWalk();
            }
            else
            {
                AnimateIdle();
            }
        }

        private void AnimateWalk()
        {
            float t = Time.time * _walkFrequency * _walkSpeedMultiplier;

            // 1. Nhún hông/thân theo từng bước chân (Bobbing tác động lên xương con, không khóa GameObject gốc)
            float bob = Mathf.Abs(Mathf.Sin(t)) * _bodyBobHeight;
            if (_hips != null)
            {
                _hips.localPosition = _hipsInitialPos + new Vector3(0f, bob, 0f);
            }
            else if (_torso != null)
            {
                _torso.localPosition = _torsoInitialPos + new Vector3(0f, bob, 0f);
            }

            // 2. Nghiêng hông nhẹ theo nhịp đi
            float sway = Mathf.Sin(t * 0.5f) * _bodySwayAngle;
            if (_hips != null)
            {
                _hips.localRotation = _hipsInitialRot * Quaternion.Euler(0f, 0f, sway);
            }
            else if (_torso != null)
            {
                _torso.localRotation = _torsoInitialRot * Quaternion.Euler(0f, 0f, sway);
            }

            // 3. Vung tay so le (Opposite Arm Swings)
            float armAngle = Mathf.Sin(t) * _armSwingAngle;
            if (_armL != null)
            {
                _armL.localRotation = _armLInitialRot * Quaternion.Euler(armAngle, 0f, 0f);
            }
            if (_armR != null)
            {
                _armR.localRotation = _armRInitialRot * Quaternion.Euler(-armAngle, 0f, 0f);
            }

            // 4. Vung chân so le (Opposite Leg Strides)
            float legAngle = Mathf.Sin(t) * _legSwingAngle;
            if (_legL != null)
            {
                _legL.localRotation = _legLInitialRot * Quaternion.Euler(-legAngle, 0f, 0f);
            }
            if (_legR != null)
            {
                _legR.localRotation = _legRInitialRot * Quaternion.Euler(legAngle, 0f, 0f);
            }
        }

        private void AnimateIdle()
        {
            // Trở về vị trí và góc gốc xương mượt mà
            if (_hips != null)
            {
                _hips.localPosition = Vector3.Lerp(_hips.localPosition, _hipsInitialPos, Time.deltaTime * 5f);
                _hips.localRotation = Quaternion.Slerp(_hips.localRotation, _hipsInitialRot, Time.deltaTime * 5f);
            }
            if (_torso != null)
            {
                _torso.localPosition = Vector3.Lerp(_torso.localPosition, _torsoInitialPos, Time.deltaTime * 5f);
                _torso.localRotation = Quaternion.Slerp(_torso.localRotation, _torsoInitialRot, Time.deltaTime * 5f);
            }

            // Nhịp thở phập phồng nhẹ (Breathing)
            float breath = Mathf.Sin(Time.time * _idleBreathFrequency) * _idleBreathAmount;
            if (_torso != null)
            {
                _torso.localScale = Vector3.one + new Vector3(breath * 0.5f, breath * 0.3f, breath);
            }

            // Đưa tay chân về tư thế nghỉ
            if (_armL != null) _armL.localRotation = Quaternion.Slerp(_armL.localRotation, _armLInitialRot, Time.deltaTime * 6f);
            if (_armR != null) _armR.localRotation = Quaternion.Slerp(_armR.localRotation, _armRInitialRot, Time.deltaTime * 6f);
            if (_legL != null) _legL.localRotation = Quaternion.Slerp(_legL.localRotation, _legLInitialRot, Time.deltaTime * 6f);
            if (_legR != null) _legR.localRotation = Quaternion.Slerp(_legR.localRotation, _legRInitialRot, Time.deltaTime * 6f);
        }
    }
}

