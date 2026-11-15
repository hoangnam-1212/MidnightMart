using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MidnightMart.Core;

namespace MidnightMart.UI
{
    /// <summary>
    /// Giao diện Tạm dừng & Cài đặt Game (Pause & Settings Menu UI).
    /// Hỗ trợ:
    /// - Nhấn [ESC] hoặc click nút bánh răng trên HUD để dừng game (Time.timeScale = 0) và mở khóa chuột.
    /// - Chuyển đổi ngôn ngữ Tiếng Việt 🇻🇳 / English 🇬🇧 tức thời bằng 1 cú click.
    /// - Thanh trượt (Slider) chỉnh Âm lượng Môi trường & SFX (tiếng gió mưa, bước chân, súng đạn) và Nhạc nền (BGM).
    /// - Tiếp tục chơi [ESC], Chơi lại ca trực, hoặc Thoát game.
    /// </summary>
    public class PauseSettingsUI : MonoBehaviour
    {
        public static PauseSettingsUI Instance { get; private set; }

        [Header("Khung Giao diện Chính")]
        [SerializeField] private GameObject _pauseMenuPanel;
        [SerializeField] private TMP_Text _titleTMP;

        [Header("Ngôn ngữ (Localization)")]
        [SerializeField] private TMP_Text _langSectionTitleTMP;
        [SerializeField] private Button _btnVietnamese;
        [SerializeField] private Button _btnEnglish;
        [SerializeField] private Image _btnViBg;
        [SerializeField] private Image _btnEnBg;

        [Header("Âm thanh (Audio Settings)")]
        [SerializeField] private TMP_Text _audioSectionTitleTMP;
        [SerializeField] private TMP_Text _sfxLabelTMP;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private TMP_Text _sfxValueTMP;

        [SerializeField] private TMP_Text _musicLabelTMP;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private TMP_Text _musicValueTMP;

        [Header("Các Nút Hành động")]
        [SerializeField] private Button _resumeBtn;
        [SerializeField] private TMP_Text _resumeBtnTMP;
        [SerializeField] private Button _restartBtn;
        [SerializeField] private TMP_Text _restartBtnTMP;
        [SerializeField] private Button _quitBtn;
        [SerializeField] private TMP_Text _quitBtnTMP;

        [Header("Nút Mở Nhanh Trên HUD (Pause Gear Icon)")]
        [SerializeField] private Button _hudPauseBtn;

        private bool _isPaused = false;
        private bool _isComputerOpen = false;

        public bool IsPaused => _isPaused;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            GameEvents.OnStoreComputerToggled += HandleStoreComputerToggled;
            LocalizationManager.OnLanguageChanged += HandleLanguageChanged;

            SetupButtonListeners();
            InitializeAudioSliders();

            if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            GameEvents.OnStoreComputerToggled -= HandleStoreComputerToggled;
            LocalizationManager.OnLanguageChanged -= HandleLanguageChanged;
        }

        private void Start()
        {
            RefreshLocalizedTexts();
            UpdateLanguageButtonHighlights();
        }

        private void SetupButtonListeners()
        {
            if (_resumeBtn != null) _resumeBtn.onClick.AddListener(ResumeGame);
            if (_restartBtn != null) _restartBtn.onClick.AddListener(RestartShift);
            if (_quitBtn != null) _quitBtn.onClick.AddListener(QuitGame);

            if (_btnVietnamese != null) _btnVietnamese.onClick.AddListener(() => SetGameLanguage(GameLanguage.Vietnamese));
            if (_btnEnglish != null) _btnEnglish.onClick.AddListener(() => SetGameLanguage(GameLanguage.English));

            if (_sfxSlider != null)
            {
                _sfxSlider.onValueChanged.AddListener(val =>
                {
                    AudioManager.Instance?.SetSFXVolume(val);
                    if (_sfxValueTMP != null) _sfxValueTMP.text = $"{Mathf.RoundToInt(val * 100f)}%";
                });
            }

            if (_musicSlider != null)
            {
                _musicSlider.onValueChanged.AddListener(val =>
                {
                    AudioManager.Instance?.SetMusicVolume(val);
                    if (_musicValueTMP != null) _musicValueTMP.text = $"{Mathf.RoundToInt(val * 100f)}%";
                });
            }

            if (_hudPauseBtn != null)
            {
                _hudPauseBtn.onClick.AddListener(TogglePause);
            }
        }

        private void InitializeAudioSliders()
        {
            float sfx = AudioManager.Instance != null ? AudioManager.Instance.SFXVolume : 0.85f;
            float music = AudioManager.Instance != null ? AudioManager.Instance.MusicVolume : 0.75f;

            if (_sfxSlider != null)
            {
                _sfxSlider.value = sfx;
                if (_sfxValueTMP != null) _sfxValueTMP.text = $"{Mathf.RoundToInt(sfx * 100f)}%";
            }

            if (_musicSlider != null)
            {
                _musicSlider.value = music;
                if (_musicValueTMP != null) _musicValueTMP.text = $"{Mathf.RoundToInt(music * 100f)}%";
            }
        }

        private void Update()
        {
            // Nhấn ESC hoặc P để bật / tắt Pause Menu
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame ||
                    UnityEngine.InputSystem.Keyboard.current.pKey.wasPressedThisFrame)
                {
                    // Nếu máy tính quầy đang mở, ưu tiên để máy tính đóng trước
                    if (_isComputerOpen) return;

                    TogglePause();
                }
            }
        }

        public void TogglePause()
        {
            if (_isPaused) ResumeGame();
            else PauseGame();
        }

        public void PauseGame()
        {
            _isPaused = true;
            Time.timeScale = 0f;

            if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(true);

            // Mở khóa chuột để người chơi tùy chỉnh
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameEvents.TriggerGamePauseToggled(true);
            GameEvents.TriggerDialogueStateChanged(true);

            RefreshLocalizedTexts();
            UpdateLanguageButtonHighlights();
        }

        public void ResumeGame()
        {
            _isPaused = false;
            Time.timeScale = 1f;

            if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(false);

            // Khóa lại chuột cho góc nhìn FPS
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            GameEvents.TriggerGamePauseToggled(false);
            GameEvents.TriggerDialogueStateChanged(false);
        }

        public void RestartShift()
        {
            Time.timeScale = 1f;
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
        }

        public void QuitGame()
        {
            Time.timeScale = 1f;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SetGameLanguage(GameLanguage lang)
        {
            LocalizationManager.Instance?.SetLanguage(lang);
            UpdateLanguageButtonHighlights();
            RefreshLocalizedTexts();
        }

        private void HandleStoreComputerToggled(bool isOpen)
        {
            _isComputerOpen = isOpen;
        }

        private void HandleLanguageChanged(GameLanguage lang)
        {
            RefreshLocalizedTexts();
            UpdateLanguageButtonHighlights();
        }

        private void UpdateLanguageButtonHighlights()
        {
            bool isVi = LocalizationManager.Instance == null || LocalizationManager.Instance.IsVietnamese;

            if (_btnViBg != null)
            {
                _btnViBg.color = isVi ? new Color(0.12f, 0.55f, 0.25f, 1.0f) : new Color(0.15f, 0.18f, 0.22f, 0.8f);
            }

            if (_btnEnBg != null)
            {
                _btnEnBg.color = !isVi ? new Color(0.12f, 0.55f, 0.25f, 1.0f) : new Color(0.15f, 0.18f, 0.22f, 0.8f);
            }
        }

        public void RefreshLocalizedTexts()
        {
            if (LocalizationManager.Instance == null) return;

            if (_titleTMP != null) _titleTMP.text = LocalizationManager.Instance.GetText("ui_pause_title", "CÀI ĐẶT & TẠM DỪNG", "PAUSE & SETTINGS");
            if (_langSectionTitleTMP != null) _langSectionTitleTMP.text = LocalizationManager.Instance.GetText("ui_language", "NGÔN NGỮ / LANGUAGE", "LANGUAGE / NGÔN NGỮ");
            if (_audioSectionTitleTMP != null) _audioSectionTitleTMP.text = LocalizationManager.T("CÀI ĐẶT ÂM THANH", "AUDIO SETTINGS");
            if (_sfxLabelTMP != null) _sfxLabelTMP.text = LocalizationManager.Instance.GetText("ui_sfx_volume", "Âm lượng Môi trường & SFX", "SFX & Ambience Volume");
            if (_musicLabelTMP != null) _musicLabelTMP.text = LocalizationManager.Instance.GetText("ui_music_volume", "Âm lượng Nhạc nền (BGM)", "Music & Tension Volume");

            if (_resumeBtnTMP != null) _resumeBtnTMP.text = LocalizationManager.Instance.GetText("ui_resume", "TIẾP TỤC [ESC]", "RESUME [ESC]");
            if (_restartBtnTMP != null) _restartBtnTMP.text = LocalizationManager.Instance.GetText("ui_restart", "CHƠI LẠI CA TRỰC", "RESTART SHIFT");
            if (_quitBtnTMP != null) _quitBtnTMP.text = LocalizationManager.Instance.GetText("ui_quit", "THOÁT GAME", "QUIT GAME");
        }
    }
}

