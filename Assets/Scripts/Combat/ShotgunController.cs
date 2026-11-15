using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MidnightMart.Core;

namespace MidnightMart.Combat
{
    /// <summary>
    /// Điều khiển hoạt động của khẩu súng săn 2 nòng (Double-Barrel Shotgun).
    /// Hỗ trợ: Rút/cất súng (phím 1), ngắm bắn (Chuột phải), bắn đạn tỏa (Pellet spread),
    /// nảy giật procedural recoil và nạp đạn (phím R).
    /// </summary>
    public class ShotgunController : MonoBehaviour
    {
        [Header("Thông số Vũ khí")]
        [SerializeField] private string _weaponName = "Súng Lục 9mm";
        [SerializeField] private int _maxAmmo = 5;        // Băng đạn 5 viên
        [SerializeField] private int _reserveAmmo = 15;   // Đạn dự trữ 15 viên (Tổng: 5 / 15)
        [SerializeField] private int _pelletCount = 1;    // 1 viên đạn súng lục chuẩn xác
        [SerializeField] private float _damagePerPellet = 35f;
        [SerializeField] private float _spreadAngle = 0.5f;
        [SerializeField] private float _range = 25f;
        [SerializeField] private float _reloadDuration = 1.2f;
        [SerializeField] private Vector3 _recoilKickback = new Vector3(-4.5f, 0.4f, 0f);

        [Header("Vị trí & Ngắm bắn (ADS)")]
        [SerializeField] private Transform _weaponModel;
        [SerializeField] private Vector3 _hipPosition = new Vector3(0.2f, -0.2f, 0.45f);
        [SerializeField] private Vector3 _aimPosition = new Vector3(0f, -0.15f, 0.4f);
        [SerializeField] private float _aimSpeed = 12f;

        [Header("Hiệu ứng Thị giác & Âm thanh")]
        [SerializeField] private Light _muzzleFlashLight;
        [SerializeField] private GameObject _muzzleFlashVisual;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _fireSound;
        [SerializeField] private AudioClip _reloadSound;
        [SerializeField] private AudioClip _emptyClickSound;

        [Header("Raycast & Va chạm")]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private LayerMask _hitLayers = ~0;

        // Trạng thái vũ khí
        private int _currentAmmo;
        private bool _isEquipped;
        private bool _isAiming;
        private bool _isReloading;
        private bool _isDialogueOpen;
        private readonly RaycastHit[] _pelletHits = new RaycastHit[1];

        public string WeaponName => _weaponName;
        public int CurrentAmmo => _currentAmmo;
        public int ReserveAmmo => _reserveAmmo;
        public int MaxAmmo => _maxAmmo;
        public bool IsEquipped => _isEquipped;
        public float DamagePerPellet => _damagePerPellet;

        /// <summary>
        /// Tiếp tế thêm đạn dự trữ (mua từ Máy tính Cửa hàng).
        /// </summary>
        public void AddReserveAmmo(int amount)
        {
            _reserveAmmo += amount;
            GameEvents.TriggerWeaponAmmoChanged(_currentAmmo, _reserveAmmo);
        }

        private void Awake()
        {
            _currentAmmo = _maxAmmo;
            if (_weaponModel != null)
            {
                _weaponModel.gameObject.SetActive(false);
            }
            if (_muzzleFlashLight != null)
            {
                _muzzleFlashLight.enabled = false;
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f; // Âm thanh FPS từ góc nhìn người chơi

#if UNITY_EDITOR
            if (_fireSound == null)
            {
                _fireSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Pistol_Fire.wav");
                if (_fireSound == null) _fireSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Shotgun_Blast.wav");
            }
            if (_reloadSound == null)
            {
                _reloadSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Pistol_Reload.wav");
            }
            if (_emptyClickSound == null)
            {
                _emptyClickSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Gun_Empty_Click.wav");
            }
#endif
            if (_fireSound == null)
            {
                _fireSound = ProceduralAudioGenerator.CreatePistolFireClip();
            }
            if (_reloadSound == null)
            {
                _reloadSound = ProceduralAudioGenerator.CreatePistolReloadClip();
            }
            if (_emptyClickSound == null)
            {
                _emptyClickSound = ProceduralAudioGenerator.CreateGunEmptyClickClip();
            }
        }

        private void OnEnable()
        {
            GameEvents.OnWeaponEquipped += HandleWeaponEquipEvent;
            GameEvents.OnDialogueStateChanged += HandleDialogueStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnWeaponEquipped -= HandleWeaponEquipEvent;
            GameEvents.OnDialogueStateChanged -= HandleDialogueStateChanged;
        }

        private void HandleDialogueStateChanged(bool isOpen)
        {
            _isDialogueOpen = isOpen;
        }

        private void HandleWeaponEquipEvent(bool equipped)
        {
            _isEquipped = equipped;
            if (_weaponModel != null)
            {
                _weaponModel.gameObject.SetActive(_isEquipped);
            }
        }

        /// <summary>
        /// Bật hoặc tắt trạng thái cầm súng từ hệ thống PlayerInventory.
        /// </summary>
        public void SetWeaponEquipped(bool equipped)
        {
            _isEquipped = equipped;
            if (_weaponModel != null)
            {
                _weaponModel.gameObject.SetActive(_isEquipped);
            }
        }

        private void Update()
        {
            if (!_isEquipped || _isDialogueOpen) return;

            HandleAiming();
            HandleManualInputs();
        }

        /// <summary>
        /// Xử lý phím tắt chuột trái (bắn), chuột phải (ngắm), phím R (nạp đạn).
        /// </summary>
        private void HandleManualInputs()
        {
            // Chuột trái: Bắn súng
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                TryFire();
            }

            // Phím R: Nạp đạn từ đạn dự trữ vào băng đạn
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                if (_currentAmmo < _maxAmmo && _reserveAmmo > 0 && !_isReloading)
                {
                    StartCoroutine(ReloadRoutine());
                }
                else if (_currentAmmo >= _maxAmmo)
                {
                    // Băng đạn đã đầy
                }
                else if (_reserveAmmo <= 0)
                {
                    if (_audioSource != null && _emptyClickSound != null)
                    {
                        _audioSource.PlayOneShot(_emptyClickSound);
                    }
                    GameEvents.TriggerSubtitle("Vũ khí", "<color=#FF3333>ĐÃ HẾT ĐẠN DỰ TRỮ! Không còn đạn để nạp!</color>", 2.0f);
                }
            }
        }

        /// <summary>
        /// Di chuyển model súng giữa vị trí cầm ngang hông và vị trí ngắm bắn thẳng tâm màn hình.
        /// </summary>
        private void HandleAiming()
        {
            if (_weaponModel == null) return;

            _isAiming = Mouse.current != null && Mouse.current.rightButton.isPressed;
            Vector3 targetPos = _isAiming ? _aimPosition : _hipPosition;
            _weaponModel.localPosition = Vector3.Lerp(_weaponModel.localPosition, targetPos, Time.deltaTime * _aimSpeed);
        }

        /// <summary>
        /// Thực hiện khai hỏa phát đạn súng lục 9mm.
        /// </summary>
        public void TryFire()
        {
            if (_isReloading) return;

            // Kiểm tra tình trạng đạn
            if (_currentAmmo <= 0)
            {
                if (_audioSource != null && _emptyClickSound != null)
                {
                    _audioSource.PlayOneShot(_emptyClickSound);
                }

                if (_reserveAmmo > 0)
                {
                    GameEvents.TriggerSubtitle("Vũ khí", "Hết đạn trong băng! Nhấn [R] để nạp đạn.", 1.5f);
                }
                else
                {
                    GameEvents.TriggerSubtitle("Vũ khí", "<color=#FF3333>HẾT SẠCH ĐẠN (0/0)! Không thể bắn!</color>", 2.0f);
                }
                return;
            }

            // 1. Giảm đạn trong băng nòng
            _currentAmmo--;

            // 2. Phát âm thanh nổ súng lục 9mm
            if (_audioSource != null && _fireSound != null)
            {
                _audioSource.PlayOneShot(_fireSound);
            }

            // 3. Chớp sáng nòng súng
            StartCoroutine(MuzzleFlashRoutine());

            // 4. Kích hoạt sự kiện giật camera & cập nhật HUD
            GameEvents.TriggerWeaponFired(_currentAmmo, _reserveAmmo, _recoilKickback);

            // 5. Bắn phát đạn chính xác từ tâm mắt camera người chơi
            Camera fpsCam = Camera.main;
            Vector3 camPos = fpsCam != null ? fpsCam.transform.position : transform.position;
            Vector3 camForward = fpsCam != null ? fpsCam.transform.forward : transform.forward;
            Vector3 shootOrigin = _firePoint != null ? _firePoint.position : (camPos + camForward * 0.35f);
            Vector3 baseDir = camForward;

            LayerMask effectiveMask = _hitLayers.value != 0 ? _hitLayers : (LayerMask)(~0);
            effectiveMask &= ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

            RaycastHit[] multiHits = new RaycastHit[8];

            for (int i = 0; i < _pelletCount; i++)
            {
                Vector3 spreadDir = baseDir;
                spreadDir += (fpsCam != null ? fpsCam.transform.right : transform.right) * Random.Range(-_spreadAngle, _spreadAngle) * 0.012f;
                spreadDir += (fpsCam != null ? fpsCam.transform.up : transform.up) * Random.Range(-_spreadAngle, _spreadAngle) * 0.012f;
                spreadDir.Normalize();

                Ray ray = new Ray(shootOrigin, spreadDir);
                int hitCount = Physics.RaycastNonAlloc(ray, multiHits, _range, effectiveMask, QueryTriggerInteraction.Ignore);
                if (hitCount > 1)
                {
                    System.Array.Sort(multiHits, 0, hitCount, System.Collections.Generic.Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));
                }

                for (int h = 0; h < hitCount; h++)
                {
                    RaycastHit hit = multiHits[h];
                    if (hit.collider == null) continue;
                    if (hit.collider.transform.root == transform.root) continue; // Bỏ qua bản thân người chơi

                    // Tác động sát thương lên khách hàng hoặc quái vật giả dạng (Cơ chế Headshot vs Bodyshot)
                    Customer.CustomerNPC customer = hit.collider.GetComponentInParent<Customer.CustomerNPC>();
                    if (customer == null)
                    {
                        customer = hit.collider.GetComponentInChildren<Customer.CustomerNPC>();
                    }

                    if (customer != null)
                    {
                        float relativeY = hit.point.y - customer.transform.position.y;
                        bool isHeadshot = relativeY >= 1.35f || hit.collider.name.ToLower().Contains("head");
                        customer.TakeDamage(_damagePerPellet, hit.point, spreadDir, isHeadshot);
                        break;
                    }

                    // Tác động sát thương lên Boss Quái vật (The Flesh Stalker / Crawler / Void Phantom)
                    BossMonsterAI boss = hit.collider.GetComponentInParent<BossMonsterAI>();
                    if (boss == null) boss = hit.collider.GetComponentInChildren<BossMonsterAI>();
                    if (boss != null)
                    {
                        boss.TakeDamage(_damagePerPellet, hit.point, spreadDir);
                        break;
                    }

                    // Tác động lực vật lý nếu trúng Rigidbody
                    Rigidbody hitRb = hit.collider.attachedRigidbody;
                    if (hitRb != null && !hitRb.isKinematic)
                    {
                        hitRb.AddForceAtPosition(spreadDir * 12f, hit.point, ForceMode.Impulse);
                    }

                    // Viên đạn dừng lại khi trúng vật cản cứng (Tường, quầy, kệ hàng)
                    if (!hit.collider.isTrigger)
                    {
                        break;
                    }
                }
            }
        }

        private IEnumerator MuzzleFlashRoutine()
        {
            if (_muzzleFlashLight != null) _muzzleFlashLight.enabled = true;
            if (_muzzleFlashVisual != null)
            {
                _muzzleFlashVisual.SetActive(true);
                _muzzleFlashVisual.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            }

            yield return new WaitForSeconds(0.06f);

            if (_muzzleFlashLight != null) _muzzleFlashLight.enabled = false;
            if (_muzzleFlashVisual != null) _muzzleFlashVisual.SetActive(false);
        }

        private IEnumerator ReloadRoutine()
        {
            if (_reserveAmmo <= 0 || _currentAmmo >= _maxAmmo) yield break;

            _isReloading = true;

            if (_audioSource != null && _reloadSound != null)
            {
                _audioSource.PlayOneShot(_reloadSound);
            }

            yield return new WaitForSeconds(_reloadDuration);

            int needed = _maxAmmo - _currentAmmo;
            int toLoad = Mathf.Min(needed, _reserveAmmo);
            _currentAmmo += toLoad;
            _reserveAmmo -= toLoad;

            _isReloading = false;
            GameEvents.TriggerWeaponReloaded(_currentAmmo, _reserveAmmo);
        }
    }
}
