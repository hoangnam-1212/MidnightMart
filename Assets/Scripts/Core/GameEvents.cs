using System;
using UnityEngine;

namespace MidnightMart.Core
{
    /// <summary>
    /// Trung tâm phân phối sự kiện toàn cục (Global Event Bus) của trò chơi.
    /// Sử dụng C# Action tĩnh để khử ghép nối (Decouple) giữa các hệ thống: Player, UI, Shop, Combat.
    /// Tuân thủ nguyên tắc Zero GC: Không tạo rác trong quá trình truyền nhận sự kiện.
    /// </summary>
    public static class GameEvents
    {
        // ==================== PLAYER & INTERACTION EVENTS ====================

        /// <summary>
        /// Kích hoạt khi người chơi nhìn vào một vật thể tương tác được.
        /// Tham số string: Nội dung gợi ý hiển thị trên HUD (Ví dụ: "Nhấn [E] để nhặt").
        /// </summary>
        public static event Action<string> OnInteractionHoverChanged;

        /// <summary>
        /// Kích hoạt khi người chơi chuyển đổi trạng thái bật/tắt đèn pin.
        /// Tham số bool: Trạng thái đèn (true: bật, false: tắt).
        /// </summary>
        public static event Action<bool> OnFlashlightToggled;

        // ==================== TUTORIAL & OBJECTIVE EVENTS ====================

        /// <summary>
        /// Kích hoạt khi mục tiêu nhiệm vụ tutorial thay đổi.
        /// Tham số string: Tiêu đề nhiệm vụ hiển thị trên HUD.
        /// </summary>
        public static event Action<string> OnObjectiveUpdated;

        /// <summary>
        /// Kích hoạt khi người chơi dọn dẹp xong 1 món đồ bừa bộn trên sàn.
        /// Tham số 1 (int): Số lượng món đã dọn.
        /// Tham số 2 (int): Tổng số lượng cần dọn.
        /// </summary>
        public static event Action<int, int> OnMessCleaned;

        /// <summary>
        /// Kích hoạt khi hiển thị một dòng phụ đề thoại (Subtitle) của khách hoặc nhân vật.
        /// Tham số 1 (string): Tên người nói.
        /// Tham số 2 (string): Lời thoại.
        /// Tham số 3 (float): Thời gian hiển thị (giây).
        /// </summary>
        public static event Action<string, string, float> OnSubtitleTriggered;

        // ==================== SHOP & TRANSACTIONS ====================

        /// <summary>
        /// Kích hoạt khi máy quét mã vạch quét thành công một món hàng.
        /// Tham số 1 (string): Tên món hàng.
        /// Tham số 2 (float): Giá tiền món hàng.
        /// </summary>
        public static event Action<string, float> OnItemScanned;

        /// <summary>
        /// Kích hoạt khi khách hàng đặt tiền lên quầy để thanh toán.
        /// Tham số 1 (float): Tổng tiền đơn hàng.
        /// Tham số 2 (float): Tiền khách đưa.
        /// </summary>
        public static event Action<float, float> OnCustomerPaid;

        /// <summary>
        /// Kích hoạt khi giao dịch thanh toán hoàn tất thành công.
        /// </summary>
        public static event Action OnCheckoutCompleted;

        // ==================== INVENTORY & SLOTS ====================

        /// <summary>
        /// Kích hoạt khi vật phẩm trên tay người chơi thay đổi (nhặt đồ mới hoặc thả/đặt lên kệ).
        /// Tham số (string): Tên vật phẩm đang cầm (null nếu tay không).
        /// </summary>
        public static event Action<string> OnHeldItemChanged;

        /// <summary>
        /// Kích hoạt khi một đoạn thoại hoặc thông báo bắt đầu/kết thúc hiển thị trên màn hình.
        /// Tham số (bool): true nếu đang mở hộp thoại, false nếu đã đóng.
        /// </summary>
        public static event Action<bool> OnDialogueStateChanged;

        /// <summary>
        /// Kích hoạt khi người chơi chuyển đổi giữa 2 slot trang bị (Slot 0: Tay/Vật phẩm, Slot 1: Shotgun).
        /// </summary>
        public static event Action<int> OnInventorySlotChanged;

        /// <summary>
        /// Kích hoạt khi súng Shotgun được mở khóa lần đầu từ ngăn kéo bí mật.
        /// </summary>
        public static event Action OnShotgunUnlocked;

        // ==================== ECONOMY & NIGHT SHIFT ====================

        /// <summary>
        /// Kích hoạt khi số tiền của người chơi thay đổi.
        /// Tham số 1 (float): Tổng tiền hiện tại.
        /// Tham số 2 (float): Biến thiên tiền (dương: cộng, âm: phạt).
        /// </summary>
        public static event Action<float, float> OnMoneyChanged;

        /// <summary>
        /// Kích hoạt khi đồng hồ thời gian ca trực cập nhật (01:00 AM, 02:00 AM, 03:30 AM...).
        /// </summary>
        public static event Action<string> OnTimeUpdated;

        /// <summary>
        /// Kích hoạt khi người chơi bắn nhầm khách hàng thật (Phạt 10% tiền).
        /// </summary>
        public static event Action<float> OnCivilianShot;

        /// <summary>
        /// Kích hoạt khi người chơi tiêu diệt đúng quái vật giả dạng (+ $50 thưởng).
        /// </summary>
        public static event Action<float> OnMonsterKilled;

        /// <summary>
        /// Kích hoạt vào thời khắc cao trào 03:30 AM (Cúp điện, đèn tắt, còi báo động).
        /// </summary>
        public static event Action OnBlackoutTriggered;

        /// <summary>
        /// Kích hoạt lúc 06:00 AM khi kết thúc ca trực đêm thành công.
        /// Tham số: (khách đã phục vụ, quái đã diệt, số lần bắn nhầm, tổng tiền).
        /// </summary>
        public static event Action<int, int, int, float> OnShiftEnded;

        // ==================== COMBAT & WEAPON ====================

        /// <summary>
        /// Kích hoạt khi rút hoặc cất vũ khí (Súng lục / Súng săn).
        /// Tham số bool: true nếu rút súng, false nếu cất súng.
        /// </summary>
        public static event Action<bool> OnWeaponEquipped;

        /// <summary>
        /// Kích hoạt khi súng khai hỏa.
        /// Tham số 1 (int): Số đạn còn lại trong băng nòng.
        /// Tham số 2 (int): Số đạn dự trữ còn lại.
        /// Tham số 3 (Vector3): Lực giật camera (Recoil Vector).
        /// </summary>
        public static event Action<int, int, Vector3> OnWeaponFired;

        /// <summary>
        /// Kích hoạt khi nạp đạn cho vũ khí.
        /// Tham số 1 (int): Số đạn trong băng sau khi nạp.
        /// Tham số 2 (int): Số đạn dự trữ còn lại.
        /// </summary>
        public static event Action<int, int> OnWeaponReloaded;

        /// <summary>
        /// Kích hoạt khi số lượng đạn thay đổi (khởi tạo, bắn hoặc nạp).
        /// </summary>
        public static event Action<int, int> OnWeaponAmmoChanged;

        /// <summary>
        /// Sự kiện tương thích: Kích hoạt khi súng Shotgun khai hỏa.
        /// Tham số 1 (int): Số đạn còn lại trong nòng.
        /// Tham số 2 (Vector3): Lực giật lùi camera (Recoil Vector).
        /// </summary>
        public static event Action<int, Vector3> OnShotgunFired;

        /// <summary>
        /// Sự kiện tương thích: Kích hoạt khi nạp đạn cho súng Shotgun.
        /// </summary>
        public static event Action<int> OnShotgunReloaded;

        // ==================== STORE COMPUTER & SUPPLIES ====================

        /// <summary>
        /// Kích hoạt khi mở hoặc đóng màn hình máy tính cửa hàng.
        /// Tham số bool: true khi mở giao diện máy tính, false khi thoát.
        /// </summary>
        public static event Action<bool> OnStoreComputerToggled;

        /// <summary>
        /// Kích hoạt khi người chơi mua một gói tiếp tế tại máy tính cửa hàng.
        /// Tham số 1 (string): Tên vật phẩm tiếp tế (Đạn súng lục, đạn shotgun, nước tăng lực...).
        /// Tham số 2 (float): Số tiền đã trả.
        /// </summary>
        public static event Action<string, float> OnSupplyPurchased;

        // ==================== BOSS ENCOUNTER EVENTS ====================

        /// <summary>
        /// Kích hoạt đếm ngược thời gian chuẩn bị phòng thủ trước khi Boss quái vật tấn công.
        /// Tham số 1 (float): Thời gian đếm ngược (giây, ví dụ: 30s).
        /// Tham số 2 (string): Tên con trùm sắp tới.
        /// </summary>
        public static event Action<float, string> OnBossCountdownStarted;

        /// <summary>
        /// Kích hoạt khi Boss chính thức xuất hiện trong siêu thị.
        /// Tham số 1 (string): Tên Boss.
        /// Tham số 2 (float): Máu tối đa.
        /// Tham số 3 (float): Máu hiện tại.
        /// </summary>
        public static event Action<string, float, float> OnBossSpawned;

        /// <summary>
        /// Kích hoạt khi máu của Boss thay đổi khi trúng đạn.
        /// Tham số 1 (float): Máu hiện tại.
        /// Tham số 2 (float): Máu tối đa.
        /// </summary>
        public static event Action<float, float> OnBossHealthChanged;

        /// <summary>
        /// Kích hoạt khi người chơi tiêu diệt thành công Boss quái vật.
        /// Tham số 1 (string): Tên Boss.
        /// Tham số 2 (float): Tiền thưởng hạ Boss ($200).
        /// </summary>
        public static event Action<string, float> OnBossDefeated;

        // ==================== PAUSE, LOCALIZATION & DAY ADVANCE EVENTS ====================

        public static event Action<bool> OnGamePauseToggled;
        public static event Action<string> OnLanguageChanged;
        public static event Action<int> OnDayAdvanced;
        public static event Action OnReturnHomeRideStarted;
        public static event Action OnDayTransitionStarted;

        // ==================== METHODS TRIGGER AN TOÀN ====================

        public static void TriggerGamePauseToggled(bool isPaused) => OnGamePauseToggled?.Invoke(isPaused);
        public static void TriggerLanguageChanged(string lang) => OnLanguageChanged?.Invoke(lang);
        public static void TriggerDayAdvanced(int day) => OnDayAdvanced?.Invoke(day);
        public static void TriggerReturnHomeRideStarted() => OnReturnHomeRideStarted?.Invoke();
        public static void TriggerDayTransitionStarted() => OnDayTransitionStarted?.Invoke();

        public static void TriggerStoreComputerToggled(bool isOpen) => OnStoreComputerToggled?.Invoke(isOpen);
        public static void TriggerSupplyPurchased(string itemName, float cost) => OnSupplyPurchased?.Invoke(itemName, cost);
        public static void TriggerBossCountdownStarted(float duration, string bossName) => OnBossCountdownStarted?.Invoke(duration, bossName);
        public static void TriggerBossSpawned(string bossName, float maxHp, float currentHp) => OnBossSpawned?.Invoke(bossName, maxHp, currentHp);
        public static void TriggerBossHealthChanged(float currentHp, float maxHp) => OnBossHealthChanged?.Invoke(currentHp, maxHp);
        public static void TriggerBossDefeated(string bossName, float reward) => OnBossDefeated?.Invoke(bossName, reward);

        public static void TriggerInteractionHover(string prompt) => OnInteractionHoverChanged?.Invoke(prompt);
        public static void TriggerFlashlightToggled(bool isOn) => OnFlashlightToggled?.Invoke(isOn);
        public static void TriggerObjectiveUpdated(string objective) => OnObjectiveUpdated?.Invoke(objective);
        public static void TriggerMessCleaned(int current, int total) => OnMessCleaned?.Invoke(current, total);
        public static void TriggerSubtitle(string speaker, string text, float duration) => OnSubtitleTriggered?.Invoke(speaker, text, duration);
        public static void TriggerItemScanned(string itemName, float price) => OnItemScanned?.Invoke(itemName, price);
        public static void TriggerCustomerPaid(float total, float paid) => OnCustomerPaid?.Invoke(total, paid);
        public static void TriggerCheckoutCompleted() => OnCheckoutCompleted?.Invoke();
        public static void TriggerHeldItemChanged(string itemName) => OnHeldItemChanged?.Invoke(itemName);
        public static void TriggerDialogueStateChanged(bool isActive) => OnDialogueStateChanged?.Invoke(isActive);
        public static void TriggerInventorySlotChanged(int slotIndex) => OnInventorySlotChanged?.Invoke(slotIndex);
        public static void TriggerShotgunUnlocked() => OnShotgunUnlocked?.Invoke();
        public static void TriggerMoneyChanged(float totalMoney, float delta) => OnMoneyChanged?.Invoke(totalMoney, delta);
        public static void TriggerTimeUpdated(string timeStr) => OnTimeUpdated?.Invoke(timeStr);
        public static void TriggerCivilianShot(float penalty) => OnCivilianShot?.Invoke(penalty);
        public static void TriggerMonsterKilled(float reward) => OnMonsterKilled?.Invoke(reward);
        public static void TriggerBlackout() => OnBlackoutTriggered?.Invoke();
        public static void TriggerShiftEnded(int served, int killed, int penalties, float money) => OnShiftEnded?.Invoke(served, killed, penalties, money);
        public static void TriggerWeaponEquipped(bool equipped) => OnWeaponEquipped?.Invoke(equipped);
        public static void TriggerWeaponFired(int currentMag, int reserveAmmo, Vector3 recoil)
        {
            OnWeaponFired?.Invoke(currentMag, reserveAmmo, recoil);
            OnShotgunFired?.Invoke(currentMag, recoil);
            OnWeaponAmmoChanged?.Invoke(currentMag, reserveAmmo);
        }
        public static void TriggerWeaponReloaded(int currentMag, int reserveAmmo)
        {
            OnWeaponReloaded?.Invoke(currentMag, reserveAmmo);
            OnShotgunReloaded?.Invoke(currentMag);
            OnWeaponAmmoChanged?.Invoke(currentMag, reserveAmmo);
        }
        public static void TriggerWeaponAmmoChanged(int currentMag, int reserveAmmo) => OnWeaponAmmoChanged?.Invoke(currentMag, reserveAmmo);
        public static void TriggerShotgunFired(int remainingAmmo, Vector3 recoil) => TriggerWeaponFired(remainingAmmo, 0, recoil);
        public static void TriggerShotgunReloaded(int currentAmmo) => TriggerWeaponReloaded(currentAmmo, 0);
    }
}

