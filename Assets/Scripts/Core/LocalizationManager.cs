using System;
using System.Collections.Generic;
using UnityEngine;

namespace MidnightMart.Core
{
    public enum GameLanguage
    {
        Vietnamese = 0,
        English = 1
    }

    /// <summary>
    /// Quản lý Đa ngôn ngữ (Localization Manager) cho Midnight Mart.
    /// Hỗ trợ 2 ngôn ngữ: Tiếng Việt (Mặc định) và Tiếng Anh (English).
    /// Tự động lưu lựa chọn người chơi vào PlayerPrefs và phát sự kiện OnLanguageChanged.
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        public static event Action<GameLanguage> OnLanguageChanged;

        private const string PREF_KEY = "MidnightMart_Language";
        private GameLanguage _currentLanguage = GameLanguage.Vietnamese;

        public GameLanguage CurrentLanguage => _currentLanguage;
        public bool IsVietnamese => _currentLanguage == GameLanguage.Vietnamese;

        private readonly Dictionary<string, (string vi, string en)> _dictionary = new Dictionary<string, (string vi, string en)>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            int savedLang = PlayerPrefs.GetInt(PREF_KEY, (int)GameLanguage.Vietnamese);
            _currentLanguage = (GameLanguage)savedLang;

            InitializeDictionary();
        }

        public void SetLanguage(GameLanguage lang)
        {
            if (_currentLanguage == lang) return;
            _currentLanguage = lang;
            PlayerPrefs.SetInt(PREF_KEY, (int)_currentLanguage);
            PlayerPrefs.Save();
            OnLanguageChanged?.Invoke(_currentLanguage);
            GameEvents.TriggerLanguageChanged(_currentLanguage == GameLanguage.Vietnamese ? "vi" : "en");
        }

        public void ToggleLanguage()
        {
            SetLanguage(_currentLanguage == GameLanguage.Vietnamese ? GameLanguage.English : GameLanguage.Vietnamese);
        }

        public string GetText(string key, string fallbackVi = "", string fallbackEn = "")
        {
            if (_dictionary.TryGetValue(key, out var entry))
            {
                return _currentLanguage == GameLanguage.Vietnamese ? entry.vi : entry.en;
            }

            if (!string.IsNullOrEmpty(fallbackVi) || !string.IsNullOrEmpty(fallbackEn))
            {
                return _currentLanguage == GameLanguage.Vietnamese ? fallbackVi : fallbackEn;
            }

            return key;
        }

        /// <summary>
        /// Hàm tiện ích tĩnh chuyển đổi nhanh 2 chuỗi song ngữ theo ngôn ngữ hiện tại.
        /// </summary>
        public static string T(string vi, string en)
        {
            if (Instance == null) return vi;
            return Instance.CurrentLanguage == GameLanguage.Vietnamese ? vi : en;
        }

        private void InitializeDictionary()
        {
            void Add(string key, string vi, string en)
            {
                _dictionary[key] = (vi, en);
            }

            // Menu & Settings
            Add("ui_pause_title", "CÀI ĐẶT & TẠM DỪNG", "PAUSE & SETTINGS");
            Add("ui_resume", "TIẾP TỤC [ESC]", "RESUME [ESC]");
            Add("ui_restart", "CHƠI LẠI CA TRỰC", "RESTART SHIFT");
            Add("ui_quit", "THOÁT GAME", "QUIT GAME");
            Add("ui_language", "NGÔN NGỮ / LANGUAGE", "LANGUAGE / NGÔN NGỮ");
            Add("ui_sfx_volume", "Âm lượng Hiệu ứng & Môi trường", "SFX & Ambience Volume");
            Add("ui_music_volume", "Âm lượng Nhạc nền (BGM)", "Music & Tension Volume");
            Add("ui_master_volume", "Tổng Âm lượng (Master)", "Master Volume");

            // Objectives
            Add("obj_home_notice", "MỤC TIÊU: Đọc giấy nợ tiền phòng & Tin nhắn việc làm trên bàn", "OBJECTIVE: Read rent debt note & job message on desk");
            Add("obj_home_door", "MỤC TIÊU: Đi ra cửa phòng để mở cửa bước ra sân", "OBJECTIVE: Head to room door, open it to porch");
            Add("obj_home_bike", "MỤC TIÊU: Bước ra sân và nhấn [E] lên xe máy", "OBJECTIVE: Step out to porch and press [E] to mount bike");
            Add("obj_ride_to_store", "MỤC TIÊU: Lái xe máy (Giữ [W]) qua rừng Blackwood đến siêu thị", "OBJECTIVE: Ride motorbike (Hold [W]) through Blackwood Forest to mart");
            Add("obj_read_memo", "MỤC TIÊU: Đọc Bản Ghi Nhớ Ca Đêm của Quản Lý dán trên tường cạnh cửa", "OBJECTIVE: Read Manager's Night Shift Memo posted beside entrance");
            Add("obj_preshift_clean", "MỤC TIÊU: Chuẩn bị ca trực - Xếp đồ rơi ({0}/3) & Lau vệt bẩn ({1}/3)", "OBJECTIVE: Pre-shift prep - Restock items ({0}/3) & Mop stains ({1}/3)");
            Add("obj_preshift_mop_first", "MỤC TIÊU: Nhặt CÂY LAU NHÀ ở góc trước tiệm để chuẩn bị lau sàn", "OBJECTIVE: Pick up CLEANING MOP by front corner to clean floor");
            Add("obj_scan_customer", "MỤC TIÊU: Quét mã vạch ({0} món hàng trên quầy) & Soi thẻ ID", "OBJECTIVE: Scan barcodes ({0} items on counter) & Inspect ID card");
            Add("obj_give_change", "MỤC TIÊU: Nhấn [E] vào Máy tính tiền để thối lại ${0:F2} cho khách", "OBJECTIVE: Press [E] on Cash Register to return ${0:F2} change to customer");
            Add("obj_check_drawer", "MỤC TIÊU: Kéo mạnh ngăn kéo bí mật dưới quầy", "OBJECTIVE: Force open secret drawer under counter");
            Add("obj_use_pc", "MỤC TIÊU: Quay lưng lại dùng Máy Tính bàn sau kiểm tra CSDL / Mua đạn", "OBJECTIVE: Turn around to use Desk Computer to verify citizen / buy ammo");
            Add("obj_boss_warning", "⚠️ CẢNH BÁO: CON TRÙM ĐANG ĐẾN! CHUẨN BỊ VŨ KHÍ ({0}s)!", "⚠️ WARNING: THE BOSS IS COMING! PREPARE WEAPONS ({0}s)!");
            Add("obj_boss_fight", "MỤC TIÊU: CHIẾN ĐẤU & TIÊU DIỆT TRÙM THE FLESH STALKER!", "OBJECTIVE: FIGHT & ELIMINATE THE FLESH STALKER BOSS!");
            Add("obj_return_home", "MỤC TIÊU: Bước ra cửa, lên xe máy lái về nhà nghỉ ngơi sau ca trực", "OBJECTIVE: Step outside, mount bike and ride back home to rest");
            Add("obj_sleep_bed", "MỤC TIÊU: Bước vào phòng trọ và [E] Lên giường đi ngủ để bắt đầu Ngày 2", "OBJECTIVE: Enter room and [E] Sleep on bed to begin Day 2");

            // Prompts
            Add("prompt_mop", "[E] Cầm Cây Lau Nhà", "[E] Pick up Cleaning Mop");
            Add("prompt_mop_needed", "[!] Cần nhặt Cây Lau Nhà ở góc cửa trước để lau sàn", "[!] Need to pick up Cleaning Mop by front door first");
            Add("prompt_stain_clean", "[E] Dùng Cây lau nhà chà sạch vết bẩn", "[E] Use Mop to scrub floor stain");
            Add("prompt_memo", "[E] Đọc Bản Ghi Nhớ Ca Đêm (Quản Lý)", "[E] Read Night Shift Memo (Manager)");
            Add("prompt_computer", "[E] Dùng Máy tính Quầy (Tra cứu Doppelgänger & Tiếp tế)", "[E] Use Terminal (Citizen Database & Supplies)");
            Add("prompt_bike_ride", "[E] Lên xe máy nổ máy đi làm", "[E] Mount Motorbike & Ride to Work");
            Add("prompt_bike_return", "[E] Lên xe máy lái về nhà nghỉ ngơi", "[E] Mount Motorbike & Ride Home");
            Add("prompt_bed_sleep", "[E] Lên giường đi ngủ (Chuyển sang Ngày 2)", "[E] Sleep on bed (Advance to Day 2)");
            Add("prompt_cash_register", "[E] Mở ngăn kéo thối tiền cho khách", "[E] Open Register & Return Change");
            Add("prompt_inspect_id", "[E] Soi Thẻ Căn Cước (ID Card)", "[E] Inspect Citizen ID Card");
            Add("prompt_scan_item", "[E] Quét mã vạch món hàng", "[E] Scan Barcode");

            // Summary & Day Transition
            Add("summary_title", "BẢNG TỔNG KẾT CA TRỰC ĐÊM 1 (06:00 AM)", "NIGHT SHIFT 1 SUMMARY (06:00 AM)");
            Add("summary_served", "- Khách hàng đã phục vụ: ", "- Customers served: ");
            Add("summary_killed", "- Quái vật giả dạng đã tiêu diệt: ", "- Doppelgängers eliminated: ");
            Add("summary_penalties", "- Bắn nhầm khách hàng thật: ", "- Innocent civilians shot: ");
            Add("summary_total", "TỔNG THU NHẬP CA TRỰC: ", "TOTAL SHIFT EARNINGS: ");
            Add("summary_congrats", "Chúc mừng bạn đã sống sót qua ca trực đêm đầu tiên!", "Congratulations! You survived your first night shift!");
            Add("summary_continue", "[Space / Click] Ra xe về nhà >>", "[Space / Click] Ride Home >>");

            // Day 2 Transition
            Add("day2_title", "--- KẾT THÚC NGÀY 1: BẠN ĐÃ SỐNG SÓT ---", "--- DAY 1 COMPLETED: YOU SURVIVED ---");
            Add("day2_narrative_1", "Bạn chìm vào giấc ngủ chập chờn sau một đêm sinh tử giữa rừng Blackwood...", "You fall into a restless sleep after a life-or-death night in Blackwood Forest...");
            Add("day2_narrative_2", "Bên ngoài khung cửa sổ phòng trọ, tiếng móng vuốt cào kéo dài trên vách gỗ suốt đêm...", "Outside your window, claws scratched against the wooden wall through the storm...");
            Add("day2_narrative_3", "11:00 PM: Chuông điện thoại reo vang! Tin nhắn mới từ Ông Chủ...", "11:00 PM: Your phone buzzes! A new message from the Manager...");
            Add("day2_boss_msg", "\"Cảnh sát vừa tìm thấy thêm 3 thi thể không còn mặt mũi trong rừng. Đêm nay bọn chúng sẽ hung hãn hơn. Đến quán ngay!\"", "\"Police just found 3 more faceless corpses in the forest. They're getting bolder tonight. Get to the store now!\"");
            Add("day2_start_btn", "[ BẮT ĐẦU CA TRỰC NGÀY 2 (DAY 2) ]", "[ BEGIN DAY 2 SHIFT ]");
        }
    }
}

