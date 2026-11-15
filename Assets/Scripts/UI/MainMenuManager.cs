using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MidnightMart.Core;
using MidnightMart.Prologue;
using MidnightMart.Tutorial;
using MidnightMart.Player;

namespace MidnightMart.UI
{
    /// <summary>
    /// Giao diện Menu Chính Toàn Màn Hình (Fullscreen Main Menu).
    /// Đơn giản, điện ảnh và kinh dị: Chỉ gồm 2 nút BẮT ĐẦU CHƠI (Play) và THOÁT GAME (Quit).
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance { get; private set; }

        [Header("Giao diện Panel")]
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Nút Hành Động Chính")]
        [SerializeField] private Button _btnPlay;
        [SerializeField] private Button _btnQuit;

        [Header("Tham Chiếu & Điểm Xuất Phát")]
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _homeSpawnPoint;
        [SerializeField] private PlayerHomeManager _playerHomeManager;
        [SerializeField] private TutorialManager _tutorialManager;

        private bool _isMenuOpen = true;
        public bool IsMenuOpen => _isMenuOpen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SetupButtons();
        }

        private void Start()
        {
            // Mở menu toàn màn hình ngay khi khởi động
            OpenMainMenu();
        }

        private void SetupButtons()
        {
            if (_btnPlay != null)
                _btnPlay.onClick.AddListener(OnPlayClicked);

            if (_btnQuit != null)
                _btnQuit.onClick.AddListener(OnQuitClicked);
        }

        public void OpenMainMenu()
        {
            _isMenuOpen = true;
            if (_mainMenuPanel != null) _mainMenuPanel.SetActive(true);
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }

            if (_btnPlay != null)
            {
                TMP_Text playText = _btnPlay.GetComponentInChildren<TMP_Text>();
                if (playText != null)
                {
                    if (SaveManager.HasSaveData && SaveManager.CurrentNight > 1)
                    {
                        playText.text = $"▶ TIẾP TỤC (ĐÊM {SaveManager.CurrentNight}) • ${SaveManager.PlayerMoney:0}";
                    }
                    else
                    {
                        playText.text = "▶ BẮT ĐẦU CHƠI (PLAY)";
                    }
                }
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameEvents.TriggerDialogueStateChanged(true);
        }

        public void CloseMainMenu()
        {
            _isMenuOpen = false;
            if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GameEvents.TriggerDialogueStateChanged(false);
        }

        public void OnPlayClicked()
        {
            CloseMainMenu();

            if (SaveManager.HasSaveData && SaveManager.CurrentNight > 1)
            {
                // Người chơi tiếp tục từ Đêm 2, Đêm 3... -> Kích hoạt thẳng ca trực siêu thị
                Night.NightShiftManager.Instance?.StartNightShift();
                return;
            }

            if (_playerTransform != null && _homeSpawnPoint != null)
            {
                TeleportPlayer(_homeSpawnPoint.position, _homeSpawnPoint.rotation);
            }

            if (_playerHomeManager != null)
            {
                _playerHomeManager.StartHomePrologue();
            }
            else
            {
                GameEvents.TriggerTimeUpdated("11:30 PM");
                GameEvents.TriggerObjectiveUpdated("MỤC TIÊU: Đi lại bàn làm việc kiểm tra Giấy báo nợ & Tin nhắn tuyển dụng");
                GameEvents.TriggerSubtitle("Độc thoại", "Đã 11:30 đêm rồi... Sắp đến giờ nhận ca trực tại siêu thị Midnight Mart.", 4.0f);
            }
        }

        public void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void TeleportPlayer(Vector3 targetPos, Quaternion targetRot)
        {
            if (_playerTransform == null) return;

            CharacterController cc = _playerTransform.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            _playerTransform.position = targetPos;
            _playerTransform.rotation = targetRot;

            if (cc != null) cc.enabled = true;
        }
    }
}
