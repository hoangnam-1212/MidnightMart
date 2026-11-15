using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Customer;

namespace MidnightMart.Night
{
    /// <summary>
    /// Quản lý toàn bộ tiến trình Ca trực Đêm (01:00 AM -> 06:00 AM) theo đúng tài liệu GDD Core Mechanics.
    /// Quản lý:
    /// - Đồng hồ thời gian ca trực (01:00 -> 06:00).
    /// - Kinh tế: Tiền bán hàng, Phạt 10% khi bắn nhầm khách thật, Thưởng $50 khi diệt quái vật.
    /// - Quái vật thoát ra ngoài làm tăng độ đông đúc của bầy quái lúc 03:30 AM.
    /// - Sự kiện Mất điện lúc 03:30 AM (Blackout Event): Tắt điện, đèn khẩn cấp đỏ xoay, bầy quái vây tiệm.
    /// - Kết thúc ca 06:00 AM: Tổng kết doanh thu, số quái đã diệt, tiền phạt.
    /// </summary>
    public class NightShiftManager : MonoBehaviour
    {
        public static NightShiftManager Instance { get; private set; }

        [Header("Thông số Kinh tế")]
        [SerializeField] private float _currentMoney = 150f;
        [SerializeField] private float _civilianPenaltyPercent = 0.10f; // Phạt 10% khi bắn nhầm dân thường
        [SerializeField] private float _monsterBounty = 50f; // Thưởng $50 khi hạ quái vật

        [Header("Tiến trình Thời gian (Giây thực tế / 1 giờ game)")]
        [SerializeField] private float _secondsPerHour = 25f; // ~2.5 phút cho toàn bộ đêm ca trực
        [SerializeField] private bool _autoStartOnAwake = false;

        [Header("Tham chiếu Môi trường & Đèn tiệm")]
        [SerializeField] private GameObject _mainLightsRoot;
        [SerializeField] private Light _emergencyRedLight;
        [SerializeField] private CustomerNPC _npcPrefab;
        [SerializeField] private MidnightMart.Combat.BossMonsterAI _bossPrefab;
        [SerializeField] private GameObject _femaleCustomerModel;
        [SerializeField] private GameObject _doppelgangerModel;
        [SerializeField] private Transform _doorEntrancePoint;
        [SerializeField] private Transform _counterWaitingPoint;
        [SerializeField] private MidnightMart.Player.MotorbikeRideController _motorbikeRideController;
        [SerializeField] private GameObject _returnMotorbikeProp;

        // Thống kê ca trực & Tiến trình ngày
        private int _currentDay = 1;
        private bool _isNightActive;
        private bool _isReturnRideAvailable = false;
        private int _currentHour = 1; // 1 = 01:00 AM
        private int _currentMinute = 0;
        private int _civiliansShotCount;
        private int _monstersKilledCount;
        private int _customersServedCount;
        private int _escapedMonstersCount;
        private bool _blackoutTriggered;
        private bool _bossTriggered;

        public int CurrentDay => _currentDay;
        public float CurrentMoney => _currentMoney;
        public int EscapedMonstersCount => _escapedMonstersCount;
        public bool IsReturnRideAvailable => _isReturnRideAvailable;

        /// <summary>
        /// Trừ tiền khi mua sắm tiếp tế từ máy tính quầy thu ngân.
        /// </summary>
        public bool DeductMoney(float amount)
        {
            if (_currentMoney < amount) return false;
            _currentMoney -= amount;
            GameEvents.TriggerMoneyChanged(_currentMoney, -amount);
            return true;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (_emergencyRedLight != null)
            {
                _emergencyRedLight.enabled = false;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnCivilianShot += HandleCivilianShot;
            GameEvents.OnMonsterKilled += HandleMonsterKilled;
            GameEvents.OnCheckoutCompleted += HandleCustomerServed;
        }

        private void OnDisable()
        {
            GameEvents.OnCivilianShot -= HandleCivilianShot;
            GameEvents.OnMonsterKilled -= HandleMonsterKilled;
            GameEvents.OnCheckoutCompleted -= HandleCustomerServed;
        }

        private void Start()
        {
            if (_autoStartOnAwake)
            {
                StartNightShift();
            }
        }

        /// <summary>
        /// Kích hoạt bắt đầu Ca trực Đêm lúc 01:00 AM (sau khi hoàn thành Tutorial).
        /// </summary>
        public void StartNightShift()
        {
            if (_isNightActive) return;
            _isNightActive = true;
            _currentHour = 1;
            _currentMinute = 0;

            GameEvents.TriggerTimeUpdated("01:00 AM");
            GameEvents.TriggerMoneyChanged(_currentMoney, 0f);
            GameEvents.TriggerObjectiveUpdated("CA TRỰC ĐÊM: Phục vụ khách & Cảnh giác kẻ giả dạng (01:00 - 06:00)");

            StartCoroutine(NightClockRoutine());
            StartCoroutine(NightCustomerSpawningRoutine());
        }

        /// <summary>
        /// Chế độ chơi nhanh (Skip Tutorial): Mở khóa Shotgun ngay lập tức và vào thẳng ca trực 01:00 AM.
        /// </summary>
        public void QuickStartNightShiftImmediate()
        {
            GameEvents.TriggerShotgunUnlocked();
            StartNightShift();
        }

        /// <summary>
        /// Đồng hồ chạy từ 01:00 AM đến 06:00 AM.
        /// </summary>
        private IEnumerator NightClockRoutine()
        {
            float timer = 0f;

            while (_isNightActive && _currentHour < 6)
            {
                timer += Time.deltaTime;
                float progressInHour = timer / _secondsPerHour;
                _currentMinute = Mathf.FloorToInt(progressInHour * 60f);

                if (_currentMinute >= 60)
                {
                    _currentHour++;
                    _currentMinute = 0;
                    timer = 0f;
                }

                // Cập nhật hiển thị đồng hồ
                string timeStr = $"{_currentHour:00}:{_currentMinute:00} AM";
                GameEvents.TriggerTimeUpdated(timeStr);

                // Sự kiện Đỉnh điểm 03:30 AM: Cúp điện & Quái vật vây tiệm!
                if (_currentHour == 3 && _currentMinute >= 30 && !_blackoutTriggered)
                {
                    _blackoutTriggered = true;
                    TriggerBlackoutEvent();
                }

                // Sự kiện Boss Trùm xuất hiện lúc gần hết ca trực (04:30 AM) hoặc khi để quái sổng
                if (_currentHour == 4 && _currentMinute >= 30 && !_bossTriggered)
                {
                    _bossTriggered = true;
                    TriggerBossEncounter();
                }

                yield return null;
            }

            // 06:00 AM: Ca trực kết thúc!
            EndNightShift();
        }

        /// <summary>
        /// Sinh các đợt khách hàng ghé tiệm trong đêm: Người thật lẫn Quái vật giả dạng.
        /// Áp dụng cơ chế Chờ đợi Tuyệt đối (Wait Until Finished):
        /// - Khách kiên nhẫn đứng đợi người chơi tại quầy, không bao giờ bị timer ép xóa.
        /// - Đợt khách tiếp theo chỉ xuất hiện sau khi khách cũ đã hoàn tất đi ra khỏi cửa hoặc bị bắn hạ.
        /// </summary>
        private IEnumerator NightCustomerSpawningRoutine()
        {
            // Khách hàng 1 (Lúc 01:20 AM): Sarah Jenkins (Người thật - 3 món hàng)
            yield return new WaitForSeconds(4.0f);
            if (!_isNightActive) yield break;
            SpawnNightCustomer("Chị gái đi làm ca đêm", isDoppelganger: false);

            // Chờ người chơi phục vụ xong khách 1 (hoặc bắn hạ) - Tuyệt đối không xóa bằng timer
            while (_isNightActive)
            {
                if (_npcPrefab == null || !_npcPrefab.gameObject.activeInHierarchy || _npcPrefab.IsDead || _npcPrefab.IsLeaving)
                {
                    // Chờ thêm 3.5s sau khi khách 1 rời tiệm để tạo khoảng lặng hồi hộp
                    yield return new WaitForSeconds(3.5f);
                    break;
                }
                yield return new WaitForSeconds(0.5f);
            }

            // Khách hàng 2: Thomas Vance (Quái vật giả dạng Đêm 1 - 4 món hàng, không bóng đổ)
            if (!_isNightActive) yield break;
            SpawnNightCustomer("Kẻ lạ mặt trùm kín đầu", isDoppelganger: true);

            // Chờ xử lý xong khách 2
            while (_isNightActive)
            {
                if (_npcPrefab == null || !_npcPrefab.gameObject.activeInHierarchy || _npcPrefab.IsDead || _npcPrefab.IsLeaving)
                {
                    break;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void SpawnNightCustomer(string name, bool isDoppelganger)
        {
            if (_npcPrefab == null) return;

            _npcPrefab.gameObject.SetActive(true);
            _npcPrefab.transform.position = _doorEntrancePoint != null ? _doorEntrancePoint.position : new Vector3(0f, 0f, -7.5f);

            // Dọn dẹp giỏ hàng cũ của khách trước nếu có
            Transform oldCart = _npcPrefab.transform.Find("Night_Customer_Cart");
            if (oldCart != null)
            {
                Destroy(oldCart.gameObject);
            }

            GameObject cartObj = new GameObject("Night_Customer_Cart");
            cartObj.transform.SetParent(_npcPrefab.transform);
            cartObj.transform.localPosition = Vector3.zero;

            List<MidnightMart.Interactions.ScannableItem> items = new List<MidnightMart.Interactions.ScannableItem>();

            if (!isDoppelganger)
            {
                if (_femaleCustomerModel != null)
                {
                    _npcPrefab.ApplyVisualMesh(_femaleCustomerModel);
                }

                // Khách 1: Sarah Jenkins (Người thật - 3 món hàng đặc trưng)
                items.Add(CreateCartItem(cartObj.transform, "Lon Nước Tăng Lực", 3.50f, new Color(0.1f, 0.7f, 0.2f), new Vector3(0.12f, 0.24f, 0.12f)));
                items.Add(CreateCartItem(cartObj.transform, "Gói Bim Bim Bắp", 2.50f, new Color(0.95f, 0.8f, 0.1f), new Vector3(0.2f, 0.25f, 0.08f)));
                items.Add(CreateCartItem(cartObj.transform, "Kẹo Cao Su Bạc Hà", 1.50f, new Color(0.2f, 0.5f, 0.9f), new Vector3(0.1f, 0.05f, 0.15f)));

                // Thẻ ID người thật (Năm sinh 1994 - 30 tuổi hợp lệ)
                CustomerIDCard idCard = CreateIDCard(cartObj.transform, "Sarah Jenkins", 1994, "BK-8842-US", "Số 42 Đại lộ Pine, Blackwood", false);
                _npcPrefab.SetupCustomer("Sarah (Khách ca đêm)", false, items, idCard, 10.0f);
            }
            else
            {
                if (_doppelgangerModel != null)
                {
                    _npcPrefab.ApplyVisualMesh(_doppelgangerModel);
                }

                // Khách 2: Thomas Vance (Quái vật giả dạng Đêm 1 - 4 món hàng)
                items.Add(CreateCartItem(cartObj.transform, "Bao Thuốc Lá Đỏ", 8.50f, new Color(0.85f, 0.15f, 0.15f), new Vector3(0.14f, 0.22f, 0.08f)));
                items.Add(CreateCartItem(cartObj.transform, "Đồ Hộp Thịt Nguội", 5.50f, new Color(0.4f, 0.25f, 0.2f), new Vector3(0.18f, 0.16f, 0.18f)));
                items.Add(CreateCartItem(cartObj.transform, "Chai Nước Suối", 2.00f, new Color(0.3f, 0.6f, 0.95f), new Vector3(0.12f, 0.26f, 0.12f)));
                items.Add(CreateCartItem(cartObj.transform, "Bánh Mì Kẹp", 4.00f, new Color(0.85f, 0.65f, 0.35f), new Vector3(0.22f, 0.12f, 0.15f)));

                // Thẻ ID Quái vật Đêm 1: Năm sinh phi lý 1820 (Đã hơn 200 tuổi!)
                CustomerIDCard idCard = CreateIDCard(cartObj.transform, "Thomas Vance", 1820, "ANOMALY-001", "Khu rừng cấm Blackwood County", true);
                _npcPrefab.SetupCustomer("Kẻ lạ mặt trùm đầu", true, items, idCard, 25.0f);
            }

            _npcPrefab.StartCustomerArrival();
        }

        private MidnightMart.Interactions.ScannableItem CreateCartItem(Transform parent, string name, float price, Color color, Vector3 scale)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.localScale = scale;
            Renderer r = obj.GetComponent<Renderer>();
            if (r != null)
            {
                r.material.color = color;
            }
            MidnightMart.Interactions.ScannableItem scannable = obj.AddComponent<MidnightMart.Interactions.ScannableItem>();
            scannable.SetupItem(name, price);
            obj.SetActive(false);
            return scannable;
        }

        private CustomerIDCard CreateIDCard(Transform parent, string holderName, int birthYear, string idCode, string address, bool isDoppelganger)
        {
            GameObject cardObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cardObj.name = $"IDCard_{holderName}";
            cardObj.transform.SetParent(parent);
            cardObj.transform.localScale = new Vector3(0.22f, 0.02f, 0.32f);
            Renderer r = cardObj.GetComponent<Renderer>();
            if (r != null)
            {
                r.material.color = isDoppelganger ? new Color(0.25f, 0.22f, 0.22f) : new Color(0.88f, 0.92f, 0.96f);
            }
            CustomerIDCard card = cardObj.AddComponent<CustomerIDCard>();
            card.SetupIDCard(holderName, birthYear, idCode, address, isDoppelganger);
            cardObj.SetActive(false);
            return card;
        }

        /// <summary>
        /// Khi người chơi bắn nhầm khách hàng thật: Phạt trừ ngay lập tức 10% tổng số tiền.
        /// </summary>
        private void HandleCivilianShot(float penaltyPercent)
        {
            _civiliansShotCount++;
            float penaltyAmount = _currentMoney * _civilianPenaltyPercent;
            _currentMoney = Mathf.Max(0f, _currentMoney - penaltyAmount);

            GameEvents.TriggerMoneyChanged(_currentMoney, -penaltyAmount);
        }

        /// <summary>
        /// Khi người chơi bắn trúng quái vật giả dạng: Thưởng nóng $50 tiền bảo vệ tiệm.
        /// </summary>
        private void HandleMonsterKilled(float reward)
        {
            _monstersKilledCount++;
            _currentMoney += _monsterBounty;

            GameEvents.TriggerMoneyChanged(_currentMoney, _monsterBounty);
        }

        private void HandleCustomerServed()
        {
            _customersServedCount++;
            float tip = 15f;
            _currentMoney += tip;
            GameEvents.TriggerMoneyChanged(_currentMoney, tip);
        }

        /// <summary>
        /// Quái vật mua hàng thoát ra cửa: Tăng độ hung hãn và tích lũy để kích hoạt Quái Trùm!
        /// </summary>
        public void ReportMonsterEscaped()
        {
            _escapedMonstersCount++;

            // Nếu để kẻ giả mạo sổng ra ngoài và ca trực đã qua nửa đêm, kích hoạt Boss sớm
            if (_escapedMonstersCount >= 1 && !_bossTriggered && _currentHour >= 3)
            {
                _bossTriggered = true;
                TriggerBossEncounter();
            }
        }

        /// <summary>
        /// Kích hoạt Cuộc chạm trán Boss Trùm (THE FLESH STALKER).
        /// Có thanh thời gian đếm ngược 30 giây trên đầu màn hình để người chơi mua đạn và tìm vị trí che chắn.
        /// </summary>
        public void TriggerBossEncounter()
        {
            StartCoroutine(BossSequenceRoutine());
        }

        private IEnumerator BossSequenceRoutine()
        {
            // Giai đoạn 1: Thanh thời gian đếm ngược 30 giây chuẩn bị phòng thủ
            GameEvents.TriggerBossCountdownStarted(30f, "THE FLESH STALKER");
            GameEvents.TriggerObjectiveUpdated("CẢNH BÁO (30s): DÙNG MÁY TÍNH MUA THÊM ĐẠN, NẠP SÚNG [R] VÀ NÉP VÀO GÓC!");
            GameEvents.TriggerSubtitle("Hệ thống khẩn cấp", "⚠️ CẢNH BÁO: QUÁI TRÙM ĐANG ĐẾN TRONG 30 GIÂY! HÃY MUA ĐẠN VÀ CHUẨN BỊ PHÒNG THỦ!", 6.0f);

            yield return new WaitForSeconds(30f);

            // Giai đoạn 2: Trùm xuất hiện trong siêu thị và kích hoạt Thanh Máu Boss
            if (_bossPrefab != null)
            {
                _bossPrefab.gameObject.SetActive(true);
                Vector3 spawnPos = _doorEntrancePoint != null ? _doorEntrancePoint.position : new Vector3(0f, 0.5f, -3.5f);
                _bossPrefab.transform.position = spawnPos;
            }
        }

        /// <summary>
        /// Kích hoạt sự kiện Mất điện kinh hoàng lúc 03:30 AM.
        /// </summary>
        private void TriggerBlackoutEvent()
        {
            GameEvents.TriggerBlackout();
            GameEvents.TriggerSubtitle("Hệ thống khẩn cấp", "⚠️ 03:30 AM: MẤT NGUỒN ĐIỆN CHÍNH! HÃY BẬT ĐÈN PIN [F] VÀ RÚT SHOTGUN [2]!", 5.5f);
            GameEvents.TriggerObjectiveUpdated($"03:30 AM: THỦ TIỆM! BẦY QUÁI KÉO ĐẾN ({_escapedMonstersCount + 1} CON)");

            StartCoroutine(BlackoutRoutine());
        }

        private IEnumerator BlackoutRoutine()
        {
            // Đèn huỳnh quang chớp tắt
            if (_mainLightsRoot != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    _mainLightsRoot.SetActive(false);
                    yield return new WaitForSeconds(0.12f);
                    _mainLightsRoot.SetActive(true);
                    yield return new WaitForSeconds(0.15f);
                }
                _mainLightsRoot.SetActive(false); // Tắt điện hoàn toàn
            }

            // Đèn khẩn cấp màu đỏ xoay vòng
            if (_emergencyRedLight != null)
            {
                _emergencyRedLight.enabled = true;
                _emergencyRedLight.color = Color.red;
                _emergencyRedLight.intensity = 800f;
            }

            // Xoay đèn đỏ tạo không khí báo động
            float blackoutTime = 0f;
            while (blackoutTime < _secondsPerHour * 1.5f && _isNightActive)
            {
                if (_emergencyRedLight != null)
                {
                    _emergencyRedLight.transform.Rotate(0f, 180f * Time.deltaTime, 0f);
                }
                blackoutTime += Time.deltaTime;
                yield return null;
            }

            // Phục hồi lại điện lúc 05:00 AM
            if (_mainLightsRoot != null) _mainLightsRoot.SetActive(true);
            if (_emergencyRedLight != null) _emergencyRedLight.enabled = false;
            GameEvents.TriggerSubtitle("Hệ thống", "Nguồn điện đã được khôi phục. Sắp kết thúc ca trực...", 4.0f);
        }

        /// <summary>
        /// Kết thúc ca trực đêm lúc 06:00 AM: Phát nhạc sáng và hiển thị bảng tổng kết.
        /// </summary>
        private void EndNightShift()
        {
            _isNightActive = false;
            GameEvents.TriggerTimeUpdated("06:00 AM");
            GameEvents.TriggerObjectiveUpdated("06:00 AM: HOÀN THÀNH CA TRỰC ĐÊM!");
            GameEvents.TriggerSubtitle("Phát thanh", "Bình minh đã ló rạng. Ca trực đêm kết thúc an toàn. Hãy kiểm tra bảng lương.", 6.0f);

            // Bắn sự kiện hiển thị bảng tổng kết trên HUD
            GameEvents.TriggerShiftEnded(_customersServedCount, _monstersKilledCount, _civiliansShotCount, _currentMoney);
        }

        /// <summary>
        /// Khi người chơi nhấn nút tắt bảng tổng kết hoặc bấm [Space]:
        /// Mở khóa chiếc xe máy đỗ bên ngoài cửa để người chơi lái xe trở về nhà.
        /// </summary>
        public void OnSummaryDismissed()
        {
            _isReturnRideAvailable = true;

            if (_returnMotorbikeProp != null)
            {
                _returnMotorbikeProp.SetActive(true);
            }

            GameEvents.TriggerObjectiveUpdated(LocalizationManager.T(
                "MỤC TIÊU: Bước ra trước cửa tiệm và [E] Lên xe máy lái về nhà nghỉ ngơi",
                "OBJECTIVE: Step outside store and [E] Mount motorbike to ride home"
            ));

            GameEvents.TriggerSubtitle(
                LocalizationManager.T("Độc thoại", "Monologue"),
                LocalizationManager.T("Ca trực kết thúc an toàn... Mau ra xe máy lái về phòng trọ nghỉ ngơi.", "Shift ended safely... Mount your motorbike and ride back home."),
                4.5f
            );
        }

        /// <summary>
        /// Bắt đầu hành trình lái xe máy ngược về nhà.
        /// </summary>
        public void StartReturnHomeRide()
        {
            if (!_isReturnRideAvailable) return;
            _isReturnRideAvailable = false;

            if (_returnMotorbikeProp != null)
            {
                _returnMotorbikeProp.SetActive(false);
            }

            if (_motorbikeRideController == null)
            {
                _motorbikeRideController = Object.FindAnyObjectByType<MidnightMart.Player.MotorbikeRideController>();
            }

            if (_motorbikeRideController != null)
            {
                _motorbikeRideController.StartMotorbikeRideReturnHome(new Vector3(-2.2f, 1.0f, -8.0f));
            }
        }

        /// <summary>
        /// Nâng ca trực sang ngày mới (Day 2).
        /// </summary>
        public void AdvanceToDay(int day)
        {
            _currentDay = day;
            _currentHour = 1;
            _currentMinute = 0;
            _blackoutTriggered = false;
            _bossTriggered = false;
            _isNightActive = false;
            _civiliansShotCount = 0;
            _monstersKilledCount = 0;
            _customersServedCount = 0;
            _escapedMonstersCount = 0;

            if (_returnMotorbikeProp != null)
            {
                _returnMotorbikeProp.SetActive(false);
            }

            Debug.Log($"[NightShiftManager] ĐÃ CHUYỂN TIẾN TRÌNH SANG NGÀY {day}!");
        }
    }
}

