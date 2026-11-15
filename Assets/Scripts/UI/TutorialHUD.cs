using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using MidnightMart.Core;
using MidnightMart.Player;

namespace MidnightMart.UI
{
    /// <summary>
    /// Giao diện HUD chính của trò chơi (Tutorial & Night Shift HUD).
    /// Hiển thị:
    /// - Hộp nhiệm vụ mục tiêu hiện tại (Góc trên bên trái).
    /// - Đồng hồ thời gian & Số tiền hiện có (Góc trên bên phải: 01:00 AM | $150).
    /// - Tâm ngắm (Crosshair Dot) & Gợi ý tương tác [E] (Chính giữa màn hình).
    /// - Kho đồ nhanh 2 Slot là 2 ô vuông ở đáy màn hình, chỉ hiện khi nhặt đồ hoặc súng.
    /// - Phụ đề & Hộp thoại đọc thông báo ở chính giữa màn hình với hướng dẫn [Space] để tiếp tục.
    /// - Khi hộp thoại đang mở, tuyệt đối không hiện nút [E] để tránh đè giao diện.
    /// - Chỉ số đạn Shotgun (2/2) & Bảng tổng kết kết quả ca trực lúc 06:00 AM.
    /// Tuân thủ quy chuẩn Event-Driven và Zero GC.
    /// </summary>
    public class TutorialHUD : MonoBehaviour
    {
        [Header("Mục tiêu Nhiệm vụ (Objective)")]
        [SerializeField] private TextMeshProUGUI _objectiveTMP;

        [Header("Đồng hồ & Tiền (Clock & Cash)")]
        [SerializeField] private GameObject _clockMoneyPanel;
        [SerializeField] private TextMeshProUGUI _clockMoneyTMP;

        [Header("Gợi ý Tương tác (Interaction Prompt)")]
        [SerializeField] private GameObject _promptPanel;
        [SerializeField] private TextMeshProUGUI _interactionPromptTMP;
        [SerializeField] private GameObject _crosshairObject;

        [Header("Kho đồ nhanh 3 Slot Động (Dynamic Quick Inventory HUD)")]
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private Image _slot1Bg;
        [SerializeField] private Image _slot2Bg;
        [SerializeField] private Image _slot3Bg;
        [SerializeField] private TextMeshProUGUI _slot1Text;
        [SerializeField] private TextMeshProUGUI _slot2Text;
        [SerializeField] private TextMeshProUGUI _slot3Text;

        [Header("Phụ đề Hội thoại (Subtitles)")]
        [SerializeField] private GameObject _subtitlePanel;
        [SerializeField] private TextMeshProUGUI _subtitleSpeakerTMP;
        [SerializeField] private TextMeshProUGUI _subtitleContentTMP;
        [SerializeField] private TextMeshProUGUI _subtitlePromptTMP;

        [Header("Chỉ số Vũ khí (Ammo Counter)")]
        [SerializeField] private GameObject _ammoPanel;
        [SerializeField] private TextMeshProUGUI _ammoTMP;

        [Header("Bảng tổng kết Ca trực (Shift Summary)")]
        [SerializeField] private GameObject _summaryPanel;
        [SerializeField] private TextMeshProUGUI _summaryText;

        [Header("Thanh Thời gian Chuẩn bị Boss (Countdown HUD)")]
        [SerializeField] private GameObject _bossCountdownPanel;
        [SerializeField] private TextMeshProUGUI _bossCountdownTMP;
        [SerializeField] private Image _bossCountdownFill;

        [Header("Thanh Máu Boss (Boss Health Bar)")]
        [SerializeField] private GameObject _bossHealthPanel;
        [SerializeField] private TextMeshProUGUI _bossNameTMP;
        [SerializeField] private TextMeshProUGUI _bossHealthTMP;
        [SerializeField] private Image _bossHealthFill;

        private Coroutine _bossCountdownCoroutine;
        private Coroutine _autoDismissDialogueCoroutine;

        private struct DialogueEntry
        {
            public string Speaker;
            public string Content;
            public float Duration;
        }

        private readonly System.Collections.Generic.Queue<DialogueEntry> _dialogueQueue = new System.Collections.Generic.Queue<DialogueEntry>();
        private bool _isDialogueActive;
        private string _heldItemName;
        private int _currentAmmo = 5;
        private int _reserveAmmo = 15;
        private int _activeSlot = 0;
        private string _currentTimeStr = "00:00 AM";
        private float _currentCash = 150f;

        private static readonly Color SlotActiveColor = new Color(0.95f, 0.8f, 0.1f, 0.95f);
        private static readonly Color SlotInactiveColor = new Color(0.12f, 0.14f, 0.18f, 0.75f);
        private static readonly Color TextActiveColor = Color.black;
        private static readonly Color TextInactiveColor = new Color(0.85f, 0.9f, 0.95f);

        private void Awake()
        {
            EnforceCanvasAndHUDLayout();

            if (_promptPanel != null) _promptPanel.SetActive(false);
            if (_subtitlePanel != null) _subtitlePanel.SetActive(false);
            if (_ammoPanel != null) _ammoPanel.SetActive(false);
            if (_summaryPanel != null) _summaryPanel.SetActive(false);
            if (_bossCountdownPanel != null) _bossCountdownPanel.SetActive(false);
            if (_bossHealthPanel != null) _bossHealthPanel.SetActive(false);
            if (_interactionPromptTMP != null) _interactionPromptTMP.text = string.Empty;

            RefreshSlotVisuals();
            UpdateClockMoneyDisplay();
        }

        private void Update()
        {
            // Nếu bảng tổng kết ca trực đang mở: Nhấn Space hoặc Click chuột để đóng bảng và ra xe về nhà
            if (_summaryPanel != null && _summaryPanel.activeSelf)
            {
                bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
                bool enterPressed = Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame;
                bool clickPressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
                if (spacePressed || enterPressed || clickPressed)
                {
                    _summaryPanel.SetActive(false);
                    if (MidnightMart.Night.NightShiftManager.Instance != null)
                    {
                        MidnightMart.Night.NightShiftManager.Instance.OnSummaryDismissed();
                    }
                    return;
                }
            }

            // Nhấn F hoặc Enter để đọc nhanh câu thoại tiếp theo hoặc đóng phụ đề (Tách rời khỏi phím Space nhảy)
            if (_isDialogueActive)
            {
                bool skipPressed = Keyboard.current != null && (Keyboard.current.fKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame);
                if (skipPressed)
                {
                    AdvanceOrCloseDialogue();
                }
            }
        }

        /// <summary>
        /// Chuẩn hóa vị trí và kích thước toàn bộ các thành phần HUD tại Runtime theo kích thước gấp đôi (Doubled HD Size).
        /// Đảm bảo giao diện luôn phẳng (ScreenSpaceOverlay), không bị nghiêng lệch,
        /// và hộp thoại đọc thông báo luôn nằm CHÍNH GIỮA MÀN HÌNH, kích thước chữ to rõ ràng không bị che lấp.
        /// </summary>
        private void EnforceCanvasAndHUDLayout()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                if (scaler != null)
                {
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920f, 1080f);
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    scaler.matchWidthOrHeight = 0.5f;
                }
            }

            // 1. Hộp thoại Đọc Tin nhắn & Phụ đề: BANNER LỚN PHONG CÁCH GAME AAA Ở PHÍA DƯỚI MÀN HÌNH (Gấp đôi kích thước)
            if (_subtitlePanel != null)
            {
                RectTransform subRt = _subtitlePanel.GetComponent<RectTransform>();
                if (subRt != null)
                {
                    subRt.anchorMin = new Vector2(0.5f, 0f);
                    subRt.anchorMax = new Vector2(0.5f, 0f);
                    subRt.pivot = new Vector2(0.5f, 0f);
                    subRt.anchoredPosition = new Vector2(0f, 105f);
                    subRt.sizeDelta = new Vector2(1320f, 240f);
                    subRt.localRotation = Quaternion.identity;
                    subRt.localScale = Vector3.one;
                }
            }

            // 2. Gợi ý Tương tác [E]: Ngay dưới tâm ngắm chính giữa màn hình (Gấp đôi kích thước)
            if (_promptPanel != null)
            {
                RectTransform promptRt = _promptPanel.GetComponent<RectTransform>();
                if (promptRt != null)
                {
                    promptRt.anchorMin = new Vector2(0.5f, 0.5f);
                    promptRt.anchorMax = new Vector2(0.5f, 0.5f);
                    promptRt.pivot = new Vector2(0.5f, 0.5f);
                    promptRt.anchoredPosition = new Vector2(0f, -70f);
                    promptRt.sizeDelta = new Vector2(720f, 68f);
                    promptRt.localRotation = Quaternion.identity;
                    promptRt.localScale = Vector3.one;
                }
            }

            // 3. Kho đồ nhanh 3 Slot Động: 3 Ô vuông linh hoạt ở đáy màn hình
            if (_inventoryPanel != null)
            {
                RectTransform invRt = _inventoryPanel.GetComponent<RectTransform>();
                if (invRt != null)
                {
                    invRt.anchorMin = new Vector2(0.5f, 0f);
                    invRt.anchorMax = new Vector2(0.5f, 0f);
                    invRt.pivot = new Vector2(0.5f, 0f);
                    invRt.anchoredPosition = new Vector2(0f, 16f);
                    invRt.sizeDelta = new Vector2(310f, 96f);
                    invRt.localRotation = Quaternion.identity;
                    invRt.localScale = Vector3.one;
                }
            }
        }

        private void OnEnable()
        {
            GameEvents.OnObjectiveUpdated += SetObjectiveText;
            GameEvents.OnInteractionHoverChanged += SetInteractionPrompt;
            GameEvents.OnSubtitleTriggered += DisplaySubtitle;
            GameEvents.OnWeaponEquipped += HandleWeaponEquip;
            GameEvents.OnWeaponFired += HandleWeaponFired;
            GameEvents.OnWeaponReloaded += HandleWeaponReloaded;
            GameEvents.OnWeaponAmmoChanged += HandleWeaponAmmoChanged;
            GameEvents.OnShotgunFired += HandleShotgunFired;
            GameEvents.OnShotgunReloaded += HandleShotgunReloaded;
            GameEvents.OnInventorySlotChanged += HandleInventorySlotChanged;
            GameEvents.OnTimeUpdated += HandleTimeUpdated;
            GameEvents.OnMoneyChanged += HandleMoneyChanged;
            GameEvents.OnShiftEnded += DisplayShiftSummary;
            GameEvents.OnHeldItemChanged += HandleHeldItemChanged;
            GameEvents.OnShotgunUnlocked += HandleShotgunUnlocked;
            GameEvents.OnBossCountdownStarted += HandleBossCountdownStarted;
            GameEvents.OnBossSpawned += HandleBossSpawned;
            GameEvents.OnBossHealthChanged += HandleBossHealthChanged;
            GameEvents.OnBossDefeated += HandleBossDefeated;
        }

        private void OnDisable()
        {
            GameEvents.OnObjectiveUpdated -= SetObjectiveText;
            GameEvents.OnInteractionHoverChanged -= SetInteractionPrompt;
            GameEvents.OnSubtitleTriggered -= DisplaySubtitle;
            GameEvents.OnWeaponEquipped -= HandleWeaponEquip;
            GameEvents.OnWeaponFired -= HandleWeaponFired;
            GameEvents.OnWeaponReloaded -= HandleWeaponReloaded;
            GameEvents.OnWeaponAmmoChanged -= HandleWeaponAmmoChanged;
            GameEvents.OnShotgunFired -= HandleShotgunFired;
            GameEvents.OnShotgunReloaded -= HandleShotgunReloaded;
            GameEvents.OnInventorySlotChanged -= HandleInventorySlotChanged;
            GameEvents.OnTimeUpdated -= HandleTimeUpdated;
            GameEvents.OnMoneyChanged -= HandleMoneyChanged;
            GameEvents.OnShiftEnded -= DisplayShiftSummary;
            GameEvents.OnHeldItemChanged -= HandleHeldItemChanged;
            GameEvents.OnShotgunUnlocked -= HandleShotgunUnlocked;
            GameEvents.OnBossCountdownStarted -= HandleBossCountdownStarted;
            GameEvents.OnBossSpawned -= HandleBossSpawned;
            GameEvents.OnBossHealthChanged -= HandleBossHealthChanged;
            GameEvents.OnBossDefeated -= HandleBossDefeated;
            if (_bossCountdownCoroutine != null) StopCoroutine(_bossCountdownCoroutine);
        }

        private void SetObjectiveText(string text)
        {
            if (_objectiveTMP != null)
            {
                _objectiveTMP.text = text;
            }
        }

        private void SetInteractionPrompt(string prompt)
        {
            bool hasPrompt = !string.IsNullOrEmpty(prompt);

            if (_promptPanel != null)
            {
                _promptPanel.SetActive(hasPrompt);
            }

            if (_interactionPromptTMP != null)
            {
                if (hasPrompt)
                {
                    // Làm nổi bật phím [E] bằng chữ vàng đậm để người chơi nhận diện phím bấm ngay lập tức
                    if (prompt.StartsWith("[E]"))
                    {
                        _interactionPromptTMP.text = "<color=#FFE600><b>[E]</b></color>" + prompt.Substring(3);
                    }
                    else
                    {
                        _interactionPromptTMP.text = prompt;
                    }
                }
                else
                {
                    _interactionPromptTMP.text = string.Empty;
                }
            }
        }

        private void HandleHeldItemChanged(string itemName)
        {
            _heldItemName = itemName;
            RefreshSlotVisuals();
        }

        private void HandleShotgunUnlocked()
        {
            RefreshSlotVisuals();
        }

        private void HandleInventorySlotChanged(int activeSlot)
        {
            _activeSlot = activeSlot;
            RefreshSlotVisuals();
        }

        /// <summary>
        /// Cập nhật hình ảnh 3 ô kho đồ nhanh linh hoạt:
        /// - "Cái nào nhặt trước thì ở ô thứ nhất xong sẽ lần lượt vào các ô tiếp theo".
        /// - Hiển thị nhãn [1], [2], [3] tương ứng phím bấm.
        /// - Hiện rõ Cây Lau Nhà, Súng 9mm (kèm số đạn), Đồ vật rơi, hoặc trạng thái Trống.
        /// - Đổi màu viền/nền vàng sáng theo ô đang kích hoạt.
        /// </summary>
        private void RefreshSlotVisuals()
        {
            Image[] slotBgs = new Image[] { _slot1Bg, _slot2Bg, _slot3Bg };
            TextMeshProUGUI[] slotTexts = new TextMeshProUGUI[] { _slot1Text, _slot2Text, _slot3Text };

            PlayerInventory inv = PlayerInventory.Instance;

            for (int i = 0; i < slotBgs.Length; i++)
            {
                if (slotBgs[i] == null) continue;

                bool isActive = (i == _activeSlot);
                slotBgs[i].color = isActive ? SlotActiveColor : SlotInactiveColor;

                if (slotTexts[i] == null) continue;
                slotTexts[i].color = isActive ? TextActiveColor : TextInactiveColor;

                if (inv != null)
                {
                    InventorySlotData slotData = inv.GetSlot(i);
                    if (slotData == null || slotData.IsEmpty)
                    {
                        slotTexts[i].text = $"<size=10><color=#778899>[{i + 1}]</color></size>\n<color=#556677>TRỐNG</color>";
                    }
                    else if (slotData.ItemType == InventoryItemType.Mop)
                    {
                        slotTexts[i].text = $"<size=10>[{i + 1}] DỤNG CỤ</size>\n<b>CHỔI LAU</b>";
                    }
                    else if (slotData.ItemType == InventoryItemType.Scanner)
                    {
                        slotTexts[i].text = $"<size=10>[{i + 1}] DỤNG CỤ</size>\n<b>MÁY QUÉT</b>";
                    }
                    else if (slotData.ItemType == InventoryItemType.Gun)
                    {
                        if (slotData.AmmoCount == 0 && slotData.ReserveAmmo == 0)
                        {
                            slotTexts[i].text = $"<size=10>[{i + 1}] SÚNG</size>\n<color=#FF3333>0/0</color>";
                        }
                        else
                        {
                            slotTexts[i].text = $"<size=10>[{i + 1}] SÚNG 9MM</size>\n<b>{slotData.AmmoCount}/{slotData.ReserveAmmo}</b>";
                        }
                    }
                    else if (slotData.ItemType == InventoryItemType.PhysicalItem)
                    {
                        string displayName = slotData.DisplayName.Replace("(Clone)", "").Trim();
                        if (displayName.Contains("Snack") || displayName.Contains("Bánh")) displayName = "BÁNH";
                        else if (displayName.Contains("Nước") || displayName.Contains("Soda") || displayName.Contains("Chai")) displayName = "NƯỚC";
                        else if (displayName.Contains("Hộp") || displayName.Contains("Canned") || displayName.Contains("Cốc")) displayName = "ĐỒ HỘP";
                        else if (displayName.Length > 8) displayName = displayName.Substring(0, 8);

                        slotTexts[i].text = $"<size=10>[{i + 1}] ĐỒ VẬT</size>\n<b>{displayName.ToUpper()}</b>";
                    }
                }
                else
                {
                    slotTexts[i].text = string.Empty;
                }
            }
        }

        private void HandleTimeUpdated(string timeStr)
        {
            _currentTimeStr = timeStr;
            UpdateClockMoneyDisplay();
        }

        private void HandleMoneyChanged(float totalMoney, float delta)
        {
            _currentCash = totalMoney;
            UpdateClockMoneyDisplay();
        }

        private void UpdateClockMoneyDisplay()
        {
            if (_clockMoneyTMP != null)
            {
                _clockMoneyTMP.text = $"{_currentTimeStr}  |  ${_currentCash:F0}";
            }
        }

        /// <summary>
        /// Kích hoạt hiển thị phụ đề hoặc đưa vào hàng đợi nếu đang đọc câu trước.
        /// </summary>
        private void DisplaySubtitle(string speaker, string content, float duration)
        {
            float actualDuration = duration > 0f ? duration : 4.5f;
            if (_isDialogueActive)
            {
                _dialogueQueue.Enqueue(new DialogueEntry { Speaker = speaker, Content = content, Duration = actualDuration });
                return;
            }

            ShowDialogue(speaker, content, actualDuration);
        }

        private void ShowDialogue(string speaker, string content, float duration)
        {
            _isDialogueActive = true;

            if (_subtitlePanel != null)
            {
                RectTransform subRt = _subtitlePanel.GetComponent<RectTransform>();
                if (subRt != null)
                {
                    subRt.anchorMin = new Vector2(0.5f, 0f);
                    subRt.anchorMax = new Vector2(0.5f, 0f);
                    subRt.pivot = new Vector2(0.5f, 0f);
                    subRt.anchoredPosition = new Vector2(0f, 105f);
                    subRt.sizeDelta = new Vector2(1320f, 240f);
                    subRt.localRotation = Quaternion.identity;
                    subRt.localScale = Vector3.one;
                }
                _subtitlePanel.SetActive(true);
            }

            if (_subtitleSpeakerTMP != null)
            {
                _subtitleSpeakerTMP.fontSize = 26f;
                _subtitleSpeakerTMP.text = $"<color=#FFE600>[ {speaker.ToUpper()} ]</color>";
            }

            if (_subtitleContentTMP != null)
            {
                _subtitleContentTMP.enableAutoSizing = true;
                _subtitleContentTMP.fontSizeMin = 18f;
                _subtitleContentTMP.fontSizeMax = 26f;
                _subtitleContentTMP.lineSpacing = 6f;
                _subtitleContentTMP.text = content;
            }

            if (_subtitlePromptTMP != null)
            {
                _subtitlePromptTMP.fontSize = 20f;
                _subtitlePromptTMP.text = LocalizationManager.T("<color=#FFE600>[F] Tiếp tục >></color>", "<color=#FFE600>[F] Continue >></color>");
            }

            if (_autoDismissDialogueCoroutine != null)
            {
                StopCoroutine(_autoDismissDialogueCoroutine);
            }
            _autoDismissDialogueCoroutine = StartCoroutine(AutoDismissDialogueRoutine(duration));
        }

        private System.Collections.IEnumerator AutoDismissDialogueRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            AdvanceOrCloseDialogue();
        }

        /// <summary>
        /// Chuyển sang câu thoại tiếp theo trong hàng đợi hoặc đóng hộp thoại.
        /// </summary>
        private void AdvanceOrCloseDialogue()
        {
            if (_autoDismissDialogueCoroutine != null)
            {
                StopCoroutine(_autoDismissDialogueCoroutine);
                _autoDismissDialogueCoroutine = null;
            }

            if (_dialogueQueue.Count > 0)
            {
                DialogueEntry next = _dialogueQueue.Dequeue();
                ShowDialogue(next.Speaker, next.Content, next.Duration);
            }
            else
            {
                _isDialogueActive = false;
                if (_subtitlePanel != null)
                {
                    _subtitlePanel.SetActive(false);
                }
            }
        }

        private void HandleWeaponEquip(bool isEquipped)
        {
            if (_ammoPanel != null)
            {
                _ammoPanel.SetActive(isEquipped);
            }
        }

        private void HandleWeaponFired(int currentMag, int reserveAmmo, Vector3 recoil)
        {
            _currentAmmo = currentMag;
            _reserveAmmo = reserveAmmo;
            UpdateAmmoDisplay(_currentAmmo, _reserveAmmo);
            RefreshSlotVisuals();
        }

        private void HandleWeaponReloaded(int currentMag, int reserveAmmo)
        {
            _currentAmmo = currentMag;
            _reserveAmmo = reserveAmmo;
            UpdateAmmoDisplay(_currentAmmo, _reserveAmmo);
            RefreshSlotVisuals();
        }

        private void HandleWeaponAmmoChanged(int currentMag, int reserveAmmo)
        {
            _currentAmmo = currentMag;
            _reserveAmmo = reserveAmmo;
            UpdateAmmoDisplay(_currentAmmo, _reserveAmmo);
            RefreshSlotVisuals();
        }

        private void HandleShotgunFired(int remainingAmmo, Vector3 recoil)
        {
            _currentAmmo = remainingAmmo;
            UpdateAmmoDisplay(_currentAmmo, _reserveAmmo);
            RefreshSlotVisuals();
        }

        private void HandleShotgunReloaded(int currentAmmo)
        {
            _currentAmmo = currentAmmo;
            UpdateAmmoDisplay(_currentAmmo, _reserveAmmo);
            RefreshSlotVisuals();
        }

        private void UpdateAmmoDisplay(int currentMag, int reserveAmmo)
        {
            if (_ammoTMP != null)
            {
                if (currentMag == 0 && reserveAmmo == 0)
                {
                    _ammoTMP.text = "<color=#FF3333>0 / 0</color>";
                }
                else
                {
                    _ammoTMP.text = $"{currentMag} / {reserveAmmo}";
                }
            }
        }

        /// <summary>
        /// Hiển thị bảng tổng kết kết quả ca trực lúc 06:00 AM.
        /// </summary>
        private void DisplayShiftSummary(int served, int killed, int penalties, float money)
        {
            if (_summaryPanel != null)
            {
                RectTransform sumRt = _summaryPanel.GetComponent<RectTransform>();
                if (sumRt != null)
                {
                    sumRt.sizeDelta = new Vector2(980f, 580f);
                }
                _summaryPanel.SetActive(true);
            }

            if (_summaryText != null)
            {
                _summaryText.fontSize = 24f;
                _summaryText.text = LocalizationManager.T(
                    $"<b>BẢNG TỔNG KẾT CA TRỰC ĐÊM (06:00 AM)</b>\n\n" +
                    $"- Khách hàng đã phục vụ: <color=#FFE600>{served}</color>\n" +
                    $"- Quái vật giả dạng đã tiêu diệt: <color=#00FF66>{killed}</color> (+$50/con)\n" +
                    $"- Bắn nhầm khách hàng thật: <color=#FF3333>{penalties}</color> (-10% phạt)\n\n" +
                    $"<b>TỔNG LƯƠNG & TIỀN HIỆN CÓ: <color=#FFE600>${money:F0}</color></b>\n\n" +
                    $"<size=22>Chúc mừng bạn đã sống sót qua ca trực đêm đầu tiên!</size>\n\n" +
                    $"<size=20><color=#FFE600>[Space] Nhận Tiền Lương & Lái Xe Về Nhà >></color></size>",

                    $"<b>NIGHT SHIFT SUMMARY REPORT (06:00 AM)</b>\n\n" +
                    $"- Customers served: <color=#FFE600>{served}</color>\n" +
                    $"- Doppelgängers eliminated: <color=#00FF66>{killed}</color> (+$50 each)\n" +
                    $"- Civilians accidentally shot: <color=#FF3333>{penalties}</color> (-10% penalty)\n\n" +
                    $"<b>TOTAL EARNED & CASH IN HAND: <color=#FFE600>${money:F0}</color></b>\n\n" +
                    $"<size=22>Congratulations! You survived your first night shift!</size>\n\n" +
                    $"<size=20><color=#FFE600>[Space] Collect Pay & Ride Back Home >></color></size>"
                );
            }
        }

        // ==================== BOSS EVENT HANDLERS ====================

        private void HandleBossCountdownStarted(float duration, string bossName)
        {
            if (_bossCountdownCoroutine != null) StopCoroutine(_bossCountdownCoroutine);
            _bossCountdownCoroutine = StartCoroutine(BossCountdownRoutine(duration, bossName));
        }

        private System.Collections.IEnumerator BossCountdownRoutine(float duration, string bossName)
        {
            if (_bossCountdownPanel != null) _bossCountdownPanel.SetActive(true);
            float remaining = duration;

            while (remaining > 0f)
            {
                remaining -= Time.deltaTime;
                float progress = Mathf.Clamp01(remaining / duration);

                if (_bossCountdownTMP != null)
                {
                    _bossCountdownTMP.text = $"⚠️ CẢNH BÁO: {bossName.ToUpper()} ĐANG ĐẾN TRONG {Mathf.CeilToInt(remaining)}s!";
                }

                if (_bossCountdownFill != null)
                {
                    _bossCountdownFill.fillAmount = progress;
                }

                yield return null;
            }

            if (_bossCountdownPanel != null) _bossCountdownPanel.SetActive(false);
        }

        private void HandleBossSpawned(string bossName, float maxHp, float currentHp)
        {
            if (_bossCountdownPanel != null) _bossCountdownPanel.SetActive(false);
            if (_bossHealthPanel != null) _bossHealthPanel.SetActive(true);

            if (_bossNameTMP != null) _bossNameTMP.text = bossName;
            UpdateBossHealthUI(currentHp, maxHp);
        }

        private void HandleBossHealthChanged(float currentHp, float maxHp)
        {
            UpdateBossHealthUI(currentHp, maxHp);
        }

        private void UpdateBossHealthUI(float currentHp, float maxHp)
        {
            if (_bossHealthTMP != null)
            {
                _bossHealthTMP.text = $"{Mathf.Max(0f, currentHp):0} / {maxHp:0} HP";
            }

            if (_bossHealthFill != null)
            {
                _bossHealthFill.fillAmount = Mathf.Clamp01(currentHp / maxHp);
            }
        }

        private void HandleBossDefeated(string bossName, float reward)
        {
            if (_bossHealthPanel != null) _bossHealthPanel.SetActive(false);
        }
    }
}
