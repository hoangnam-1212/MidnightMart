using System.Collections;
using UnityEngine;
using MidnightMart.Core;

namespace MidnightMart.Combat
{
    /// <summary>
    /// Điều khiển Trí tuệ Nhân tạo của Con Trùm Quái vật Đêm 1 (THE FLESH STALKER / TRÙM DỊ BIẾN).
    /// Hỗ trợ:
    /// - Thanh máu Boss đồ sộ hiển thị trên đỉnh màn hình (1000 HP).
    /// - Di chuyển truy đuổi người chơi khắp không gian siêu thị mở rộng (22m x 20m).
    /// - Tấn công vung vuốt cào cấu (Melee Swipes) và phát tiếng gầm rú rung chuyển.
    /// - Khi bị tiêu diệt: Ngã gục, bốc khói và thưởng nóng $200 cho người chơi!
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class BossMonsterAI : MonoBehaviour
    {
        [Header("Chỉ số Sinh mệnh Boss")]
        [SerializeField] private string _bossName = "THE FLESH STALKER - TRÙM DỊ BIẾN ĐÊM 1";
        [SerializeField] private float _maxHealth = 1000f;
        [SerializeField] private float _moveSpeed = 3.0f;
        [SerializeField] private float _attackRange = 2.5f;
        [SerializeField] private float _attackDamage = 25f;
        [SerializeField] private float _attackCooldown = 2.0f;

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
        private Transform _playerTransform;
        private Renderer[] _renderers;
        private Color[] _originalColors;
        private MidnightMart.Customer.ProceduralCharacterAnimator _characterAnimator;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsDead => _isDead;

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _renderers = GetComponentsInChildren<Renderer>();
            if (_renderers != null && _renderers.Length > 0)
            {
                _originalColors = new Color[_renderers.Length];
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if (_renderers[i].material != null)
                        _originalColors[i] = _renderers[i].material.color;
                }
            }

            CapsuleCollider col = GetComponent<CapsuleCollider>();
            if (col != null)
            {
                col.center = new Vector3(0f, 1.4f, 0f);
                col.height = 2.8f;
                col.radius = 0.75f;
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0.8f;

            _characterAnimator = GetComponent<MidnightMart.Customer.ProceduralCharacterAnimator>();
            if (_characterAnimator == null)
            {
                _characterAnimator = gameObject.AddComponent<MidnightMart.Customer.ProceduralCharacterAnimator>();
            }
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

            // Bắt đầu chuỗi xuất hiện
            StartCoroutine(SpawnSequenceRoutine());
        }

        private IEnumerator SpawnSequenceRoutine()
        {
            if (_audioSource != null && _spawnRoarClip != null)
            {
                _audioSource.PlayOneShot(_spawnRoarClip);
            }

            // Rung lắc người tạo cảm giác đáng sợ
            float t = 0f;
            Vector3 basePos = transform.position;
            while (t < 2.0f)
            {
                t += Time.deltaTime;
                transform.position = basePos + new Vector3(Random.Range(-0.05f, 0.05f), 0f, Random.Range(-0.05f, 0.05f));
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
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 4.5f);
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
                // Di chuyển truy đuổi người chơi qua các dãy kệ hàng siêu thị
                if (_characterAnimator != null) _characterAnimator.IsWalking = true;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
            }
        }

        private IEnumerator AttackRoutine()
        {
            _isAttacking = true;
            _lastAttackTime = Time.time;

            if (_audioSource != null && _attackRoarClip != null)
            {
                _audioSource.PlayOneShot(_attackRoarClip);
            }

            GameEvents.TriggerSubtitle(_bossName, "Quái trùm vung móng vuốt cào xé không khí!", 2.0f);

            // Lao nhanh tới trước một chút
            Vector3 forwardLunge = transform.forward * 0.8f;
            transform.position += forwardLunge;

            yield return new WaitForSeconds(0.4f);

            // Kiểm tra xem người chơi còn ở cự ly gần không để gây sát thương
            if (_playerTransform != null && Vector3.Distance(transform.position, _playerTransform.position) <= _attackRange + 0.5f)
            {
                GameEvents.TriggerSubtitle("Cảnh báo", $"Bạn bị trúng móng vuốt quái trùm (-{_attackDamage} HP)! Hãy chạy kiting quanh các kệ hàng!", 2.5f);
            }

            yield return new WaitForSeconds(0.6f);
            _isAttacking = false;
        }

        /// <summary>
        /// Xử lý sát thương khi bị người chơi bắn đạn trúng.
        /// </summary>
        public void TakeDamage(float damage, Vector3 hitPoint, Vector3 shotDirection)
        {
            if (_isDead) return;

            _currentHealth -= damage;
            GameEvents.TriggerBossHealthChanged(_currentHealth, _maxHealth);

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
                    if (_renderers[i] != null && _renderers[i].material != null)
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
            StopAllCoroutines();

            if (_audioSource != null && _deathRoarClip != null)
            {
                _audioSource.PlayOneShot(_deathRoarClip);
            }

            // Thông báo hạ gục Boss, thưởng nóng $200
            GameEvents.TriggerBossDefeated(_bossName, 200f);
            GameEvents.TriggerMonsterKilled(200f);
            GameEvents.TriggerSubtitle("Chiến thắng!", "BẠN ĐÃ TIÊU DIỆT THÀNH CÔNG QUÁI TRÙM! (Thưởng nóng +$200 tiền mặt)", 5.0f);
            GameEvents.TriggerObjectiveUpdated("HOÀN THÀNH: Đã tiêu diệt Quái trùm! Ca trực an toàn đến 06:00 AM.");

            // Gục ngã xuống sàn
            transform.localScale = new Vector3(transform.localScale.x * 1.2f, transform.localScale.y * 0.25f, transform.localScale.z * 1.2f);
            transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);

            StartCoroutine(DisappearAfterDeathRoutine());
        }

        private IEnumerator DisappearAfterDeathRoutine()
        {
            yield return new WaitForSeconds(6.0f);
            gameObject.SetActive(false);
        }
    }
}
