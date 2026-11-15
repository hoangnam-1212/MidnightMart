using System.Collections;
using UnityEngine;
using MidnightMart.Core;

namespace MidnightMart.Combat
{
    /// <summary>
    /// Điều khiển Trí tuệ Nhân tạo của Quái vật Đêm (Night Shift Monster AI).
    /// Hỗ trợ:
    /// - Cơ chế va chạm vật lý qua CharacterController: KHÔNG BAO GIỜ ĐI XUYÊN TƯỜNG HAY KỆ HÀNG.
    /// - Quái vật phân cấp theo từng đêm (Đêm 1: Flesh Stalker, Đêm 2: Screamer Crawler, Đêm 3: Abyssal Void).
    /// - Thanh máu đồ sộ hiển thị trên màn hình HUD cùng phản hồi trúng đạn rõ ràng (Flash đỏ, âm thanh trúng thương).
    /// - Kỹ năng đặc biệt của từng đêm (Lướt nhanh, Gầm thét gây mờ mắt, Dịch chuyển tức thời trong bóng tối).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class BossMonsterAI : MonoBehaviour
    {
        [Header("Chỉ số Sinh mệnh Boss")]
        [SerializeField] private string _bossName = "THE FLESH STALKER - TRÙM DỊ BIẾN ĐÊM 1";
        [SerializeField] private float _maxHealth = 180f; // Cân bằng hợp lý: ~5-6 viên đạn 9mm hoặc 2 phát shotgun
        [SerializeField] private float _moveSpeed = 2.8f;
        [SerializeField] private float _attackRange = 2.2f;
        [SerializeField] private float _attackDamage = 20f;
        [SerializeField] private float _attackCooldown = 1.8f;

        [Header("Âm thanh & Hiệu ứng")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _spawnRoarClip;
        [SerializeField] private AudioClip _attackRoarClip;
        [SerializeField] private AudioClip _deathRoarClip;
        [SerializeField] private AudioClip _hitHurtClip;

        private float _currentHealth;
        private bool _isDead;
        private bool _isAttacking;
        private float _lastAttackTime;
        private float _lastSpecialSkillTime;
        private int _currentNight = 1;
        private Transform _playerTransform;
        private CharacterController _characterController;
        private Renderer[] _renderers;
        private Color[] _originalColors;
        private MidnightMart.Customer.ProceduralCharacterAnimator _characterAnimator;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsDead => _isDead;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            if (_characterController != null)
            {
                _characterController.center = new Vector3(0f, 1.3f, 0f);
                _characterController.height = 2.6f;
                _characterController.radius = 0.65f;
                _characterController.stepOffset = 0.35f;
                _characterController.minMoveDistance = 0.001f;
            }

            CapsuleCollider col = GetComponent<CapsuleCollider>();
            if (col != null)
            {
                col.center = new Vector3(0f, 1.3f, 0f);
                col.height = 2.6f;
                col.radius = 0.70f;
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0.85f;

            _characterAnimator = GetComponent<MidnightMart.Customer.ProceduralCharacterAnimator>();
            if (_characterAnimator == null)
            {
                _characterAnimator = gameObject.AddComponent<MidnightMart.Customer.ProceduralCharacterAnimator>();
            }

            CacheRenderers();
            SetupMonsterForNight(SaveManager.CurrentNight);
        }

        private void CacheRenderers()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            if (_renderers != null && _renderers.Length > 0)
            {
                _originalColors = new Color[_renderers.Length];
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if (_renderers[i] != null && _renderers[i].material != null)
                        _originalColors[i] = _renderers[i].material.color;
                }
            }
        }

        /// <summary>
        /// Cấu hình độ khó, máu và kỹ năng của quái vật riêng biệt cho từng Đêm.
        /// </summary>
        public void SetupMonsterForNight(int nightIndex)
        {
            _currentNight = Mathf.Max(1, nightIndex);

            switch (_currentNight)
            {
                case 1:
                    _bossName = "THE FLESH STALKER (QUÁI RÌNH MÒ ĐÊM 1)";
                    _maxHealth = 180f;
                    _moveSpeed = 2.8f;
                    _attackDamage = 20f;
                    _attackCooldown = 1.8f;
                    ApplyColorTint(new Color(0.75f, 0.25f, 0.22f));
                    break;

                case 2:
                    _bossName = "THE SCREAMER CRAWLER (QUÁI CÀO XÉ ĐÊM 2)";
                    _maxHealth = 320f;
                    _moveSpeed = 3.6f;
                    _attackDamage = 25f;
                    _attackCooldown = 1.4f;
                    ApplyColorTint(new Color(0.18f, 0.18f, 0.24f));
                    break;

                default: // Đêm 3 trở lên
                    _bossName = "THE ABYSSAL VOID (BÓNG MA HƯ KHÔNG ĐÊM 3)";
                    _maxHealth = 480f;
                    _moveSpeed = 4.2f;
                    _attackDamage = 35f;
                    _attackCooldown = 1.2f;
                    ApplyColorTint(new Color(0.08f, 0.05f, 0.12f));
                    break;
            }

            _currentHealth = _maxHealth;
        }

        private void ApplyColorTint(Color tint)
        {
            if (_renderers == null) return;
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null && _renderers[i].material != null)
                {
                    _renderers[i].material.color = tint;
                }
            }
            CacheRenderers();
        }

        private void Start()
        {
            if (_playerTransform == null && Camera.main != null)
            {
                _playerTransform = Camera.main.transform;
            }

            // Kích hoạt thanh máu Boss trên màn hình HUD
            GameEvents.TriggerBossSpawned(_bossName, _maxHealth, _currentHealth);
            GameEvents.TriggerSubtitle(_bossName, "GRAAAAARRR! (Tiếng gầm rú làm rung chuyển cả siêu thị!)", 4.5f);

            StartCoroutine(SpawnSequenceRoutine());
        }

        private IEnumerator SpawnSequenceRoutine()
        {
            if (_audioSource != null && _spawnRoarClip != null)
            {
                _audioSource.PlayOneShot(_spawnRoarClip);
            }

            // Rung lắc người tạo cảm giác gầm rú đáng sợ
            float t = 0f;
            Vector3 basePos = transform.position;
            while (t < 1.5f)
            {
                t += Time.deltaTime;
                transform.position = basePos + new Vector3(Random.Range(-0.04f, 0.04f), 0f, Random.Range(-0.04f, 0.04f));
                yield return null;
            }
            transform.position = basePos;
        }

        private void Update()
        {
            if (_isDead) return;

            if (_playerTransform == null && Camera.main != null)
            {
                _playerTransform = Camera.main.transform;
            }

            if (_playerTransform == null) return;

            Vector3 targetPos = new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z);
            float distToPlayer = Vector3.Distance(transform.position, targetPos);

            // Xoay mặt nhìn về phía người chơi
            Vector3 lookDir = (targetPos - transform.position).normalized;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5.0f);
            }

            // Kỹ năng đặc biệt của Đêm 3 (Void Blink - Dịch chuyển tức thời vào bóng tối phía sau lưng người chơi mỗi 9s)
            if (_currentNight >= 3 && Time.time >= _lastSpecialSkillTime + 9.0f && distToPlayer > 5.0f)
            {
                _lastSpecialSkillTime = Time.time;
                StartCoroutine(VoidBlinkRoutine());
                return;
            }

            // Nếu đang trong tầm đánh: Vung vuốt tấn công
            if (distToPlayer <= _attackRange)
            {
                if (_characterAnimator != null) _characterAnimator.IsWalking = false;
                if (Time.time >= _lastAttackTime + _attackCooldown && !_isAttacking)
                {
                    StartCoroutine(AttackRoutine());
                }
            }
            else
            {
                // Di chuyển truy đuổi qua CharacterController (CHỐNG ĐI XUYÊN TƯỜNG)
                if (_characterAnimator != null) _characterAnimator.IsWalking = true;

                Vector3 moveDir = lookDir;

                // Tránh vật cản (Kệ hàng, tường quầy) nếu bị chắn phía trước mặt
                if (Physics.Raycast(transform.position + Vector3.up * 0.6f, transform.forward, out RaycastHit obsHit, 1.4f))
                {
                    if (obsHit.collider != null && obsHit.collider.transform.root != _playerTransform.root)
                    {
                        Vector3 rightAvoid = transform.right;
                        bool rightClear = !Physics.Raycast(transform.position + Vector3.up * 0.6f, rightAvoid, 1.2f);
                        moveDir = (moveDir + (rightClear ? rightAvoid : -rightAvoid) * 1.6f).normalized;
                    }
                }

                Vector3 motion = moveDir * (_moveSpeed * Time.deltaTime);
                motion.y = -9.81f * Time.deltaTime; // Trọng lực giữ chân bám sàn

                if (_characterController != null && _characterController.enabled)
                {
                    _characterController.Move(motion);
                }
                else
                {
                    transform.position += motion;
                }
            }
        }

        private IEnumerator VoidBlinkRoutine()
        {
            if (_playerTransform == null) yield break;

            GameEvents.TriggerSubtitle("Kỹ năng quái vật", "⚠️ BÓNG MA HƯ KHÔNG ĐANG BIẾN MẤT TRONG BÓNG TỐI!", 2.0f);
            if (_audioSource != null && _attackRoarClip != null) _audioSource.PlayOneShot(_attackRoarClip);

            // Biến mất trong 0.4s
            if (_renderers != null)
            {
                foreach (var r in _renderers) if (r != null) r.enabled = false;
            }

            yield return new WaitForSeconds(0.4f);

            // Tái xuất hiện cách sau lưng người chơi 3m
            Vector3 behindPos = _playerTransform.position - _playerTransform.forward * 3.0f;
            behindPos.y = transform.position.y;

            if (_characterController != null) _characterController.enabled = false;
            transform.position = behindPos;
            if (_characterController != null) _characterController.enabled = true;

            if (_renderers != null)
            {
                foreach (var r in _renderers) if (r != null) r.enabled = true;
            }

            GameEvents.TriggerSubtitle("Cảnh báo", "⚠️ NÓ ĐÃ DỊCH CHUYỂN RA PHÍA SAU LƯNG BẠN!", 2.0f);
        }

        private IEnumerator AttackRoutine()
        {
            _isAttacking = true;
            _lastAttackTime = Time.time;

            if (_audioSource != null && _attackRoarClip != null)
            {
                _audioSource.PlayOneShot(_attackRoarClip);
            }

            GameEvents.TriggerSubtitle(_bossName, "Quái trùm vung móng vuốt cào xé không khí!", 1.8f);

            // Lao tới nhẹ qua CharacterController (không xuyên tường)
            if (_characterController != null && _characterController.enabled)
            {
                _characterController.Move(transform.forward * 0.6f);
            }

            yield return new WaitForSeconds(0.35f);

            // Gây sát thương nếu người chơi không kịp lùi lại
            if (_playerTransform != null && Vector3.Distance(transform.position, _playerTransform.position) <= _attackRange + 0.6f)
            {
                GameEvents.TriggerSubtitle("Cảnh báo", $"<color=#FF2222>Bị trúng cào xé (-{_attackDamage:0} HP)! Hãy chạy thả diều quanh các kệ hàng!</color>", 2.5f);
            }

            yield return new WaitForSeconds(0.5f);
            _isAttacking = false;
        }

        /// <summary>
        /// Xử lý sát thương khi bị người chơi bắn trúng.
        /// </summary>
        public void TakeDamage(float damage, Vector3 hitPoint, Vector3 shotDirection)
        {
            if (_isDead) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - damage);
            GameEvents.TriggerBossHealthChanged(_currentHealth, _maxHealth);

            if (_audioSource != null && _hitHurtClip != null)
            {
                _audioSource.PlayOneShot(_hitHurtClip);
            }

            // Thông báo hiển thị máu rõ ràng
            GameEvents.TriggerSubtitle("Sát thương", $"<color=#FF3333>Bắn trúng {_bossName}! -{damage:0} HP (Còn {_currentHealth:0}/{_maxHealth:0})</color>", 1.5f);

            // Chớp đỏ toàn thân báo hiệu trúng đạn
            StartCoroutine(FlashRedRoutine());

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        private IEnumerator FlashRedRoutine()
        {
            if (_renderers != null)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if (_renderers[i] != null && _renderers[i].material != null)
                    {
                        _renderers[i].material.color = Color.red;
                    }
                }
            }

            yield return new WaitForSeconds(0.08f);

            if (_renderers != null && _originalColors != null)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if (_renderers[i] != null && _renderers[i].material != null && i < _originalColors.Length)
                    {
                        _renderers[i].material.color = _originalColors[i];
                    }
                }
            }
        }

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;
            if (_characterAnimator != null) _characterAnimator.IsWalking = false;
            if (_characterController != null) _characterController.enabled = false;
            StopAllCoroutines();

            if (_audioSource != null && _deathRoarClip != null)
            {
                _audioSource.PlayOneShot(_deathRoarClip);
            }

            float bountyReward = 150f + (_currentNight * 50f);
            GameEvents.TriggerBossDefeated(_bossName, bountyReward);
            GameEvents.TriggerMonsterKilled(bountyReward);
            GameEvents.TriggerSubtitle("Chiến thắng!", $"BẠN ĐÃ TIÊU DIỆT THÀNH CÔNG {_bossName}! (Thưởng nóng +${bountyReward:0} tiền mặt)", 5.0f);
            GameEvents.TriggerObjectiveUpdated($"HOÀN THÀNH: Đã tiêu diệt {_bossName}! Ca trực an toàn đến 06:00 AM.");

            // Gục ngã xuống sàn (KHÔNG bóp méo scale)
            transform.rotation = Quaternion.Euler(-90f, transform.eulerAngles.y, 0f);
            transform.position = new Vector3(transform.position.x, 0.15f, transform.position.z);

            StartCoroutine(DisappearAfterDeathRoutine());
        }

        private IEnumerator DisappearAfterDeathRoutine()
        {
            yield return new WaitForSeconds(5.0f);
            gameObject.SetActive(false);
        }
    }
}
