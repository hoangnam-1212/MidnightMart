using UnityEngine;

namespace MidnightMart.Core
{
    /// <summary>
    /// Quản lý Lưu & Tải Tiến độ Game (Game Progress & Night Save System).
    /// Hỗ trợ lưu trữ bền vững:
    /// - Đêm trực hiện tại (Đêm 1, Đêm 2, Đêm 3...)
    /// - Số tiền quỹ tiệm tích lũy
    /// - Số đạn súng lục và súng shotgun hiện có
    /// - Hỗ trợ Reset ca trực khi gặp lỗi hoặc Reset toàn bộ tiến độ chơi mới từ đầu.
    /// </summary>
    public static class SaveManager
    {
        private const string PREF_KEY_NIGHT = "MidnightMart_CurrentNight";
        private const string PREF_KEY_MONEY = "MidnightMart_PlayerMoney";
        private const string PREF_KEY_PISTOL_AMMO = "MidnightMart_PistolAmmo";
        private const string PREF_KEY_SHOTGUN_AMMO = "MidnightMart_ShotgunAmmo";
        private const string PREF_KEY_HAS_SAVE = "MidnightMart_HasSaveData";

        public static bool HasSaveData => PlayerPrefs.GetInt(PREF_KEY_HAS_SAVE, 0) == 1;

        public static int CurrentNight
        {
            get => Mathf.Max(1, PlayerPrefs.GetInt(PREF_KEY_NIGHT, 1));
            set => PlayerPrefs.SetInt(PREF_KEY_NIGHT, value);
        }

        public static float PlayerMoney
        {
            get => PlayerPrefs.GetFloat(PREF_KEY_MONEY, 150.0f);
            set => PlayerPrefs.SetFloat(PREF_KEY_MONEY, value);
        }

        public static int PistolAmmo
        {
            get => PlayerPrefs.GetInt(PREF_KEY_PISTOL_AMMO, 20);
            set => PlayerPrefs.SetInt(PREF_KEY_PISTOL_AMMO, value);
        }

        public static int ShotgunAmmo
        {
            get => PlayerPrefs.GetInt(PREF_KEY_SHOTGUN_AMMO, 8);
            set => PlayerPrefs.SetInt(PREF_KEY_SHOTGUN_AMMO, value);
        }

        /// <summary>
        /// Lưu trạng thái tiến độ hiện tại vào bộ nhớ máy (PlayerPrefs).
        /// </summary>
        public static void SaveProgress(int night, float money, int pistolAmmo = 20, int shotgunAmmo = 8)
        {
            PlayerPrefs.SetInt(PREF_KEY_HAS_SAVE, 1);
            PlayerPrefs.SetInt(PREF_KEY_NIGHT, night);
            PlayerPrefs.SetFloat(PREF_KEY_MONEY, money);
            PlayerPrefs.SetInt(PREF_KEY_PISTOL_AMMO, pistolAmmo);
            PlayerPrefs.SetInt(PREF_KEY_SHOTGUN_AMMO, shotgunAmmo);
            PlayerPrefs.Save();
            Debug.Log($"[SaveManager] Đã lưu tiến độ: Đêm {night}, Quỹ ${money:F2}, Đạn 9mm: {pistolAmmo}, Shotgun: {shotgunAmmo}");
        }

        /// <summary>
        /// Xóa sạch dữ liệu lưu để người chơi bắt đầu lại từ Đêm 1 (Màn mở đầu phòng trọ).
        /// </summary>
        public static void ResetAllProgress()
        {
            PlayerPrefs.DeleteKey(PREF_KEY_HAS_SAVE);
            PlayerPrefs.DeleteKey(PREF_KEY_NIGHT);
            PlayerPrefs.DeleteKey(PREF_KEY_MONEY);
            PlayerPrefs.DeleteKey(PREF_KEY_PISTOL_AMMO);
            PlayerPrefs.DeleteKey(PREF_KEY_SHOTGUN_AMMO);
            PlayerPrefs.Save();
            Debug.Log("[SaveManager] Đã xóa toàn bộ tiến độ chơi. Bắt đầu lại từ đầu!");
        }
    }
}

