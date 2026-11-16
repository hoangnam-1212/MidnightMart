using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MidnightMart.Core;
using MidnightMart.Customer;
using MidnightMart.Night;
using MidnightMart.Combat;

namespace MidnightMart.UI
{
    /// <summary>
    /// Giao diện điều khiển Máy tính quầy thu ngân (Store Desktop Terminal UI).
    /// Gồm 2 phân hệ:
    /// 1. CSDL Tra cứu Công dân Blackwood (Phát hiện Kẻ giả dạng Doppelgänger).
    /// 2. Cửa hàng Tiếp tế Đêm (Mua đạn súng lục, đạn shotgun, nước tăng lực).
    /// </summary>
    public class StoreComputerUI : MonoBehaviour
    {
        public static StoreComputerUI Instance { get; private set; }

        [Header("Giao diện Tổng")]
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private TMP_Text _moneyTMP;

        [Header("Tab Tra cứu CSDL")]
        [SerializeField] private GameObject _databaseTabPanel;
        [SerializeField] private TMP_Text _citizenNameTMP;
        [SerializeField] private TMP_Text _citizenDetailsTMP;
        [SerializeField] private TMP_Text _citizenStatusTMP;

        [Header("Tab Cửa hàng Tiếp tế")]
        [SerializeField] private GameObject _supplyTabPanel;
        [SerializeField] private TMP_Text _supplyFeedbackTMP;

        [Header("Nút bấm Điều khiển")]
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _tabDatabaseBtn;
        [SerializeField] private Button _tabSupplyBtn;
        [SerializeField] private Button _buyPistolAmmoBtn;
        [SerializeField] private Button _buyShotgunAmmoBtn;
        [SerializeField] private Button _buyEnergyDrinkBtn;
        [SerializeField] private Button _buyBarricadeBtn;

        [Header("Tham chiếu Súng & Đạn")]
        [SerializeField] private ShotgunController _weaponController;

        private bool _isOpen;
        private int _currentTabIndex = 0; // 0: Database, 1: Supply
        private float _openTimestamp;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            if (_weaponController == null) _weaponController = FindAnyObjectByType<ShotgunController>();

            GameEvents.OnStoreComputerToggled += HandleComputerToggled;

            if (_closeBtn != null) _closeBtn.onClick.AddListener(CloseComputer);
            if (_tabDatabaseBtn != null) _tabDatabaseBtn.onClick.AddListener(() => SelectTab(0));
            if (_tabSupplyBtn != null) _tabSupplyBtn.onClick.AddListener(() => SelectTab(1));
            if (_buyPistolAmmoBtn != null) _buyPistolAmmoBtn.onClick.AddListener(BuyPistolAmmo);
            if (_buyShotgunAmmoBtn != null) _buyShotgunAmmoBtn.onClick.AddListener(BuyShotgunAmmo);
            if (_buyEnergyDrinkBtn != null) _buyEnergyDrinkBtn.onClick.AddListener(BuyEnergyDrink);
            if (_buyBarricadeBtn != null) _buyBarricadeBtn.onClick.AddListener(BuyDoorBarricade);

            if (_mainPanel != null) _mainPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            GameEvents.OnStoreComputerToggled -= HandleComputerToggled;
        }

        private void Update()
        {
            if (!_isOpen) return;

            // Chống đóng nhầm trong 0.35 giây đầu do phím [E] mở máy tính
            if (Time.unscaledTime - _openTimestamp < 0.35f) return;

            // Nhấn ESC hoặc E để thoát nhanh khỏi máy tính
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame ||
                    UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
                {
                    CloseComputer();
                    return;
                }

                // Phím tắt đổi tab F1 / 1 (CSDL), F2 / 2 (Tiếp tế)
                if (UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame ||
                    UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame)
                {
                    SelectTab(0);
                }
                else if (UnityEngine.InputSystem.Keyboard.current.digit2Key.wasPressedThisFrame ||
                         UnityEngine.InputSystem.Keyboard.current.f2Key.wasPressedThisFrame)
                {
                    SelectTab(1);
                }
            }
        }

        public void HandleComputerToggled(bool open)
        {
            if (open) OpenComputer();
            else CloseComputer();
        }

        public void OpenComputer()
        {
            _isOpen = true;
            _openTimestamp = Time.unscaledTime;
            if (_mainPanel != null) _mainPanel.SetActive(true);

            // Mở khóa chuột để click giao diện
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameEvents.TriggerDialogueStateChanged(true);

            UpdateMoneyDisplay();
            SelectTab(_currentTabIndex);
        }

        public void CloseComputer()
        {
            _isOpen = false;
            if (_mainPanel != null) _mainPanel.SetActive(false);

            // Khóa lại chuột cho góc nhìn FPS
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            GameEvents.TriggerDialogueStateChanged(false);
        }

        public void SelectTab(int tabIndex)
        {
            _currentTabIndex = tabIndex;
            if (_databaseTabPanel != null) _databaseTabPanel.SetActive(_currentTabIndex == 0);
            if (_supplyTabPanel != null) _supplyTabPanel.SetActive(_currentTabIndex == 1);

            if (_currentTabIndex == 0)
            {
                RefreshCitizenDatabase();
            }
            else
            {
                if (_supplyFeedbackTMP != null) _supplyFeedbackTMP.text = "Chọn gói tiếp tế cần mua để sẵn sàng chiến đấu!";
            }
        }

        /// <summary>
        /// Tải thông tin từ hồ sơ công dân đang đứng trước quầy để người chơi kiểm tra đối chiếu.
        /// </summary>
        private void RefreshCitizenDatabase()
        {
            CustomerNPC currentCustomer = null;
            for (int i = 0; i < CustomerNPC.ActiveCustomers.Count; i++)
            {
                if (CustomerNPC.ActiveCustomers[i] != null && 
                    CustomerNPC.ActiveCustomers[i].gameObject.activeInHierarchy &&
                    !CustomerNPC.ActiveCustomers[i].IsDead &&
                    !CustomerNPC.ActiveCustomers[i].IsLeaving)
                {
                    currentCustomer = CustomerNPC.ActiveCustomers[i];
                    break;
                }
            }

            if (currentCustomer == null)
            {
                if (_citizenNameTMP != null) _citizenNameTMP.text = "CHƯA CÓ KHÁCH HÀNG TẠI QUẦY";
                if (_citizenDetailsTMP != null) _citizenDetailsTMP.text = "Hệ thống đang quét mặt tiền siêu thị... Vui lòng chờ khách mang đồ lên quầy.";
                if (_citizenStatusTMP != null)
                {
                    _citizenStatusTMP.text = "TRẠNG THÁI: CHỜ KẾT NỐI CAMERA...";
                    _citizenStatusTMP.color = Color.gray;
                }
                return;
            }

            string name = currentCustomer.CustomerName;
            bool isMonster = currentCustomer.IsDoppelganger;
            CustomerIDCard card = currentCustomer.IDCard;

            if (_citizenNameTMP != null)
            {
                _citizenNameTMP.text = $"HỒ SƠ: {name.ToUpper()}";
            }

            if (isMonster)
            {
                if (_citizenStatusTMP != null)
                {
                    _citizenStatusTMP.text = "⚠️ BÁO ĐỘNG ĐỎ: ĐỐI TƯỢNG DỊ BIẾN / KẺ GIẢ DẠNG (DOPPELGÄNGER)!";
                    _citizenStatusTMP.color = new Color(1f, 0.2f, 0.2f);
                }

                if (_citizenDetailsTMP != null)
                {
                    _citizenDetailsTMP.text = 
                        "• Mã số lưu trữ: #ANOMALY-1845-DEAD\n" +
                        "• Cảnh báo Cảnh sát: Đối tượng đã mất tích trong rừng Blackwood từ hơn 100 năm trước!\n" +
                        "• Dấu hiệu dị thường: KHÔNG CÓ BÓNG ĐỔ dưới đèn huỳnh quang, cổ co giật góc 90 độ, năm sinh phi lý (năm 1820).\n" +
                        "• LỆNH HÀNH ĐỘNG: TIÊU DIỆT TỨC KHẮC! KHÔNG ĐƯỢC ĐỂ THOÁT RA CỬA!";
                }
            }
            else
            {
                if (_citizenStatusTMP != null)
                {
                    _citizenStatusTMP.text = "✓ HỒ SƠ HỢP LỆ: DÂN THƯỜNG THỊ TRẤN BLACKWOOD";
                    _citizenStatusTMP.color = new Color(0.2f, 0.9f, 0.3f);
                }

                int birthYear = card != null ? card.BirthYear : 1975;
                string address = card != null ? card.Address : "Khu dân cư Blackwood";

                if (_citizenDetailsTMP != null)
                {
                    _citizenDetailsTMP.text = 
                        $"• Năm sinh: {birthYear} (Tuổi hợp pháp)\n" +
                        $"• Địa chỉ cư trú: {address}\n" +
                        "• Tình trạng: Không có tiền án, công dân bình thường mua sắm ban đêm.\n" +
                        "• LỆNH HÀNH ĐỘNG: Phục vụ chu đáo, quét mã hàng và thối tiền đầy đủ.";
                }
            }
        }

        // ==================== MUA SẮM TIẾP TẾ ====================

        public void BuyPistolAmmo()
        {
            TryBuySupply("Hộp đạn Súng Lục 9mm (+10 viên)", 15f, () =>
            {
                if (_weaponController != null) _weaponController.AddReserveAmmo(10);
            });
        }

        public void BuyShotgunAmmo()
        {
            TryBuySupply("Hộp đạn Shotgun 12-Gauge (+4 viên)", 30f, () =>
            {
                if (_weaponController != null) _weaponController.AddReserveAmmo(4);
            });
        }

        public void BuyEnergyDrink()
        {
            TryBuySupply("Lon Nước Tăng Lực Midnight (+Hồi phục thể lực)", 12f, () =>
            {
                GameEvents.TriggerSubtitle("Người chơi", "Đã uống lon nước tăng lực! Cơ thể hồi phục tràn đầy sinh lực để chạy né quái!", 3.5f);
            });
        }

        public void BuyDoorBarricade()
        {
            TryBuySupply("Bộ Ván Gỗ Gia Cố Cửa Kính", 25f, () =>
            {
                GameEvents.TriggerSubtitle("Hệ thống", "Đã mua bộ ván gỗ gia cố cửa tiệm! Kính cửa được cường lực chống chịu quái vật đập vỡ.", 3.5f);
            });
        }

        // ==================== KHO VŨ KHÍ NÂNG CẤP (CẦY TIỀN MUA SẮM) ====================

        public void BuyShotgunWeapon()
        {
            TryBuySupply("Khẩu Shotgun Săn 2 Nòng (12-Gauge)", 120f, () =>
            {
                if (_weaponController != null)
                {
                    _weaponController.UpgradeWeapon("Shotgun Săn 12-Gauge", 2, 8, 28f, 2.8f, 1.8f, 12);
                }
            });
        }

        public void BuyHuntingRifleWeapon()
        {
            TryBuySupply("Súng Trường Săn Bắn (.308 Sniper)", 250f, () =>
            {
                if (_weaponController != null)
                {
                    _weaponController.UpgradeWeapon("Súng Trường .308", 10, 1, 110f, 0.1f, 1.5f, 20);
                }
            });
        }

        public void BuySubmachineGunWeapon()
        {
            TryBuySupply("Súng Tiểu Liên Đêm (Night SMG 9mm)", 350f, () =>
            {
                if (_weaponController != null)
                {
                    _weaponController.UpgradeWeapon("Tiểu Liên SMG 9mm", 30, 1, 45f, 0.8f, 1.2f, 60);
                }
            });
        }

        private void TryBuySupply(string itemName, float cost, System.Action onPurchased)
        {
            float currentMoney = NightShiftManager.Instance != null ? NightShiftManager.Instance.CurrentMoney : 150f;
            if (NightShiftManager.Instance != null)
            {
                if (!NightShiftManager.Instance.DeductMoney(cost))
                {
                    if (_supplyFeedbackTMP != null)
                    {
                        _supplyFeedbackTMP.text = $"❌ Không đủ tiền! Cần ${cost:F2} (Bạn chỉ có ${currentMoney:F2})";
                        _supplyFeedbackTMP.color = Color.red;
                    }
                    return;
                }
            }
            else
            {
                if (currentMoney < cost)
                {
                    if (_supplyFeedbackTMP != null)
                    {
                        _supplyFeedbackTMP.text = $"❌ Không đủ tiền! Cần ${cost:F2} (Bạn chỉ có ${currentMoney:F2})";
                        _supplyFeedbackTMP.color = Color.red;
                    }
                    return;
                }
                GameEvents.TriggerMoneyChanged(currentMoney - cost, -cost);
            }

            onPurchased?.Invoke();
            UpdateMoneyDisplay();

            if (_supplyFeedbackTMP != null)
            {
                _supplyFeedbackTMP.text = $"✓ Mua thành công: {itemName}! (-${cost:F2})";
                _supplyFeedbackTMP.color = Color.green;
            }
        }

        private void UpdateMoneyDisplay()
        {
            float money = NightShiftManager.Instance != null ? NightShiftManager.Instance.CurrentMoney : 150f;
            if (_moneyTMP != null)
            {
                _moneyTMP.text = $"NGÂN SÁCH HIỆN CÓ: ${money:F2}";
            }
        }
    }
}
