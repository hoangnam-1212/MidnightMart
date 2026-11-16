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
        [SerializeField] private GameObject _maleCustomerModel;
        [SerializeField] private GameObject _doppelgangerModel;
        [SerializeField] private Transform _doorEntrancePoint;
        [SerializeField] private Transform _counterWaitingPoint;
        [SerializeField] private MidnightMart.Player.MotorbikeRideController _motorbikeRideController;
        [SerializeField] private GameObject _returnMotorbikeProp;

        // Thống kê ca trực & Tiến trình ngày
        private int _currentDay = 1;
        private bool _isNightActive;
        private bool _isReturnRideAvailable = false;
        private int _currentHour = 0; // 0 = 00:00 AM (Bắt đầu từ nửa đêm)
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
        /// Kích hoạt bắt đầu Ca trực Đêm lúc 00:00 AM (sau khi hoàn thành Tutorial).
        /// </summary>
        public void StartNightShift()
        {
            if (_isNightActive) return;
            _isNightActive = true;
            _currentDay = SaveManager.CurrentNight;
            _currentMoney = SaveManager.PlayerMoney;
            _currentHour = 0; // 00:00 AM
            _currentMinute = 0;
            _blackoutTriggered = false;
            _bossTriggered = false;

            if (_bossPrefab != null)
            {
                _bossPrefab.SetupMonsterForNight(_currentDay);
            }

            GameEvents.TriggerTimeUpdated("00:00 AM");
            GameEvents.TriggerMoneyChanged(_currentMoney, 0f);
            GameEvents.TriggerObjectiveUpdated($"CA TRỰC ĐÊM {_currentDay}: Phục vụ 5 khách hàng & Cảnh giác quái vật giả dạng (00:00 - 06:00)");

            StartCoroutine(NightClockRoutine());
            StartCoroutine(NightCustomerSpawningRoutine());
        }

        /// <summary>
        /// Chế độ chơi nhanh (Skip Tutorial): Mở khóa Shotgun ngay lập tức và vào thẳng ca trực 00:00 AM.
        /// </summary>
        public void QuickStartNightShiftImmediate()
        {
            GameEvents.TriggerShotgunUnlocked();
            StartNightShift();
        }

        /// <summary>
        /// Đồng hồ chạy từ 00:00 AM đến 06:00 AM.
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

                // Sự kiện Đỉnh điểm đúng 03:00 AM: Mất điện toàn tiệm & Quái vật đêm tấn công!
                if (_currentHour == 3 && _currentMinute >= 0 && !_blackoutTriggered)
                {
                    _blackoutTriggered = true;
                    TriggerBlackoutEvent();
                    TriggerBossEncounter();
                }

                yield return null;
            }

            // 06:00 AM: Ca trực kết thúc!
            EndNightShift();
        }

        /// <summary>
        /// Sinh 5 đợt khách hàng ghé tiệm từ 00:00 đến 03:00 AM:
        /// - 3 Khách hàng người thật bình thường.
        /// - 2 Quái vật giả dạng (Doppelgänger) trà trộn mua đồ mà người chơi cần kiểm tra ID/CSDL để phát hiện.
        /// - Áp dụng cơ chế Chờ đợi Tuyệt đối: Khách đứng đợi người chơi, chỉ rời đi sau khi thanh toán hoặc bị bắn hạ.
        /// </summary>
        private IEnumerator NightCustomerSpawningRoutine()
        {
            for (int customerIdx = 0; customerIdx < 5; customerIdx++)
            {
                if (!_isNightActive || _currentHour >= 3) yield break;

                // Khoảng chờ giữa các lượt khách
                yield return new WaitForSeconds(3.5f);
                if (!_isNightActive || _currentHour >= 3) yield break;

                SpawnSpecificNightCustomer(customerIdx);

                // Chờ người chơi phục vụ xong (hoặc bắn hạ)
                while (_isNightActive && _currentHour < 3)
                {
                    if (_npcPrefab == null || !_npcPrefab.gameObject.activeInHierarchy || _npcPrefab.IsDead || _npcPrefab.IsLeaving)
                    {
                        yield return new WaitForSeconds(3.0f);
                        break;
                    }
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        private void SpawnSpecificNightCustomer(int index)
        {
            if (_npcPrefab == null) return;

            _npcPrefab.gameObject.SetActive(true);
            _npcPrefab.transform.position = _doorEntrancePoint != null ? _doorEntrancePoint.position : new Vector3(0f, 0f, -7.5f);

            // Dọn dẹp giỏ hàng cũ nếu có (cả giỏ hướng dẫn và ca đêm)
            Transform oldCart = _npcPrefab.transform.Find("Night_Customer_Cart");
            if (oldCart != null)
            {
                Destroy(oldCart.gameObject);
            }
            Transform oldTutorialCart = _npcPrefab.transform.Find("Tutorial_Customer_Cart");
            if (oldTutorialCart != null)
            {
                Destroy(oldTutorialCart.gameObject);
            }

            GameObject cartObj = new GameObject("Night_Customer_Cart");
            cartObj.transform.SetParent(_npcPrefab.transform);
            cartObj.transform.localPosition = new Vector3(0f, -50f, 0f); // Tuyệt đối không xuất hiện dưới chân khách hàng

            List<MidnightMart.Interactions.ScannableItem> items = new List<MidnightMart.Interactions.ScannableItem>();

            switch (index)
            {
                case 0:
                    // Khách 1 (00:15 AM): Sarah Jenkins (Y tá bệnh viện - Người thật, 3 món hàng)
                    if (_femaleCustomerModel != null) _npcPrefab.ApplyVisualMesh(_femaleCustomerModel);
                    items.Add(CreateCartItem(cartObj.transform, "Lon Nước Tăng Lực", 3.50f, new Color(0.1f, 0.7f, 0.2f), new Vector3(0.12f, 0.24f, 0.12f)));
                    items.Add(CreateCartItem(cartObj.transform, "Gói Bim Bim Bắp", 2.50f, new Color(0.95f, 0.8f, 0.1f), new Vector3(0.2f, 0.25f, 0.08f)));
                    items.Add(CreateCartItem(cartObj.transform, "Kẹo Cao Su Bạc Hà", 1.50f, new Color(0.2f, 0.5f, 0.9f), new Vector3(0.1f, 0.05f, 0.15f)));
                    CustomerIDCard id1 = CreateIDCard(cartObj.transform, "Sarah Jenkins", 1994, "BK-8842-US", "Số 42 Đại lộ Pine, Blackwood", false);
                    _npcPrefab.SetupCustomer("Sarah (Y tá ca đêm)", false, items, id1, 10.0f);
                    break;

                case 1:
                    // Khách 2 (00:50 AM): Mike Davis (Tài xế giao hàng đêm - Người thật, 3 món hàng)
                    if (_maleCustomerModel != null) _npcPrefab.ApplyVisualMesh(_maleCustomerModel);
                    items.Add(CreateCartItem(cartObj.transform, "Lon Cà Phê Đen", 2.80f, new Color(0.25f, 0.15f, 0.1f), new Vector3(0.12f, 0.22f, 0.12f)));
                    items.Add(CreateCartItem(cartObj.transform, "Bao Thuốc Lá", 7.50f, new Color(0.85f, 0.2f, 0.2f), new Vector3(0.14f, 0.20f, 0.08f)));
                    items.Add(CreateCartItem(cartObj.transform, "Bánh Mì Kẹp", 4.00f, new Color(0.85f, 0.65f, 0.35f), new Vector3(0.22f, 0.12f, 0.15f)));
                    CustomerIDCard id2 = CreateIDCard(cartObj.transform, "Mike Davis", 1988, "TRK-5519-BW", "Bến xe ngoại ô Blackwood", false);
                    _npcPrefab.SetupCustomer("Mike (Tài xế giao hàng)", false, items, id2, 20.0f);
                    break;

                case 2:
                    // Khách 3 (01:30 AM): Kẻ giả mạo thay đổi theo từng Đêm (Độ khó tăng tiến)
                    if (_doppelgangerModel != null) _npcPrefab.ApplyVisualMesh(_doppelgangerModel);

                    if (_currentDay == 1)
                    {
                        // ĐÊM 1: Dấu hiệu lộ liễu - Năm sinh 1820 (>200 tuổi), mã ANOMALY-001, không có bóng đổ
                        items.Add(CreateCartItem(cartObj.transform, "Hộp Thịt Đóng Hộp", 5.50f, new Color(0.4f, 0.25f, 0.2f), new Vector3(0.18f, 0.16f, 0.18f)));
                        items.Add(CreateCartItem(cartObj.transform, "Chai Nước Tẩy Trắng", 6.00f, new Color(0.2f, 0.4f, 0.8f), new Vector3(0.14f, 0.28f, 0.14f)));
                        items.Add(CreateCartItem(cartObj.transform, "Bao Diêm Gỗ", 1.50f, new Color(0.7f, 0.1f, 0.1f), new Vector3(0.1f, 0.04f, 0.14f)));
                        CustomerIDCard id3 = CreateIDCard(cartObj.transform, "Thomas Vance", 1820, "ANOMALY-001", "Khu rừng cấm Blackwood County", true);
                        _npcPrefab.SetupCustomer("Kẻ lạ mặt trùm đầu", true, items, id3, 30.0f);
                    }
                    else if (_currentDay == 2)
                    {
                        // ĐÊM 2: Dấu hiệu tinh vi - Năm sinh 1985 bình thường, nhưng mã ID giả mạo không khớp máy tính
                        items.Add(CreateCartItem(cartObj.transform, "Can Axit Tẩy Rửa", 8.50f, new Color(0.3f, 0.7f, 0.4f), new Vector3(0.18f, 0.28f, 0.18f)));
                        items.Add(CreateCartItem(cartObj.transform, "Cuộn Túi Đen Dày", 4.20f, new Color(0.1f, 0.1f, 0.1f), new Vector3(0.10f, 0.10f, 0.25f)));
                        items.Add(CreateCartItem(cartObj.transform, "Băng Keo Dính Lớn", 3.00f, new Color(0.6f, 0.5f, 0.3f), new Vector3(0.12f, 0.06f, 0.12f)));
                        CustomerIDCard id3 = CreateIDCard(cartObj.transform, "Arthur Pendelton", 1985, "FAKE-BW-882", "Ngõ Cụt Không Số, Blackwood", true);
                        _npcPrefab.SetupCustomer("Người đàn ông đeo kính đen", true, items, id3, 40.0f);
                    }
                    else
                    {
                        // ĐÊM 3+: Kẻ ngụy trang cấp cao - Giả dạng tinh xảo, mắt đỏ dõi theo người chơi, mua đồ giải phẫu rùng rợn
                        items.Add(CreateCartItem(cartObj.transform, "Dao Phay Sắc Nhọn", 16.50f, new Color(0.8f, 0.82f, 0.85f), new Vector3(0.08f, 0.04f, 0.38f)));
                        items.Add(CreateCartItem(cartObj.transform, "Bình Clo Nồng Độ Cao", 14.00f, new Color(0.7f, 0.2f, 0.2f), new Vector3(0.16f, 0.30f, 0.16f)));
                        items.Add(CreateCartItem(cartObj.transform, "Bao Tải Đay Cỡ Đại", 6.50f, new Color(0.5f, 0.45f, 0.35f), new Vector3(0.22f, 0.10f, 0.25f)));
                        CustomerIDCard id3 = CreateIDCard(cartObj.transform, "Bác Sĩ Robert Hall", 1978, "CORRUPTED-DNA", "Bệnh Viện Bỏ Hoang Blackwood", true);
                        _npcPrefab.SetupCustomer("Bác sĩ Robert (Mắt đỏ ngầu)", true, items, id3, 60.0f);
                    }
                    break;

                case 3:
                    // Khách 4 (02:10 AM): Emma Watson (Nữ sinh viên - Người thật, 3 món hàng)
                    if (_femaleCustomerModel != null) _npcPrefab.ApplyVisualMesh(_femaleCustomerModel);
                    items.Add(CreateCartItem(cartObj.transform, "Tô Mì Ăn Liền", 2.20f, new Color(0.9f, 0.5f, 0.1f), new Vector3(0.20f, 0.16f, 0.20f)));
                    items.Add(CreateCartItem(cartObj.transform, "Chai Soda Chanh", 2.00f, new Color(0.2f, 0.8f, 0.3f), new Vector3(0.12f, 0.26f, 0.12f)));
                    items.Add(CreateCartItem(cartObj.transform, "Thanh Sô-cô-la", 1.80f, new Color(0.35f, 0.18f, 0.10f), new Vector3(0.10f, 0.03f, 0.22f)));
                    CustomerIDCard id4 = CreateIDCard(cartObj.transform, "Emma Watson", 2002, "STU-9921-BW", "Ký túc xá Đại học Blackwood", false);
                    _npcPrefab.SetupCustomer("Emma (Sinh viên)", false, items, id4, 10.0f);
                    break;

                default:
                    // Khách 5 (02:45 AM): Kẻ kỳ quái trước giờ bão (Quái vật giả dạng tinh vi trước khi cúp điện)
                    if (_doppelgangerModel != null) _npcPrefab.ApplyVisualMesh(_doppelgangerModel);
                    items.Add(CreateCartItem(cartObj.transform, "Dao Bếp Cán Đen", 12.00f, new Color(0.15f, 0.15f, 0.18f), new Vector3(0.08f, 0.05f, 0.35f)));
                    items.Add(CreateCartItem(cartObj.transform, "Tảng Thịt Tươi", 9.50f, new Color(0.85f, 0.15f, 0.15f), new Vector3(0.24f, 0.10f, 0.20f)));
                    items.Add(CreateCartItem(cartObj.transform, "Đèn Pin Cầm Tay", 15.00f, new Color(0.2f, 0.2f, 0.25f), new Vector3(0.10f, 0.10f, 0.28f)));
                    CustomerIDCard id5 = CreateIDCard(cartObj.transform, "Đối Tượng Vô Danh", 0, "ANOMALY-CORRUPT", "Ranh giới hư không Blackwood", true);
                    _npcPrefab.SetupCustomer("Kẻ kỳ quái trùm đầu", true, items, id5, 50.0f);
                    break;
            }

            _npcPrefab.StartCustomerArrival();
        }

        private MidnightMart.Interactions.ScannableItem CreateCartItem(Transform parent, string name, float price, Color color, Vector3 scale)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.localPosition = new Vector3(0f, -50f, 0f);
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
            cardObj.transform.localPosition = new Vector3(0f, -50f, 0f);
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
            GameEvents.TriggerSubtitle("Hệ thống khẩn cấp", "[!] CẢNH BÁO: QUÁI TRÙM ĐANG ĐẾN TRONG 30 GIÂY! HÃY MUA ĐẠN VÀ CHUẨN BỊ PHÒNG THỦ!", 6.0f);

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
            GameEvents.TriggerSubtitle("Hệ thống khẩn cấp", "[!] 03:30 AM: MẤT NGUỒN ĐIỆN CHÍNH! HÃY BẬT ĐÈN PIN [F] VÀ RÚT SHOTGUN [2]!", 5.5f);
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

            // Tự động lưu tiến độ sang Đêm tiếp theo với số tiền tích lũy
            SaveManager.SaveProgress(_currentDay + 1, _currentMoney);

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

