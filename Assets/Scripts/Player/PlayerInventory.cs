using UnityEngine;
using UnityEngine.InputSystem;
using MidnightMart.Core;
using MidnightMart.Combat;

namespace MidnightMart.Player
{
    public enum InventoryItemType
    {
        Empty = 0,
        Mop = 1,
        Gun = 2,
        PhysicalItem = 3,
        Scanner = 4
    }

    [System.Serializable]
    public class InventorySlotData
    {
        public InventoryItemType ItemType = InventoryItemType.Empty;
        public string DisplayName = string.Empty;
        public int AmmoCount = 0;
        public int ReserveAmmo = 0;
        public Transform PhysicalTransform = null;
        public Rigidbody PhysicalRb = null;

        public bool IsEmpty => ItemType == InventoryItemType.Empty;

        public void Clear()
        {
            ItemType = InventoryItemType.Empty;
            DisplayName = string.Empty;
            AmmoCount = 0;
            ReserveAmmo = 0;
            PhysicalTransform = null;
            PhysicalRb = null;
        }
    }

    /// <summary>
    /// Hệ thống Quản lý Kho đồ Động đa năng (Dynamic Multi-Slot Inventory System).
    /// Hỗ trợ 3 Slot linh hoạt: Món nào nhặt trước sẽ nằm ở ô thứ nhất, món nhặt sau sẽ ở ô tiếp theo theo thứ tự.
    /// Hỗ trợ:
    /// - Cây Lau Nhà (Mop)
    /// - Súng Lục 9mm (Gun)
    /// - Vật phẩm cầm tay / Hàng hóa rơi (Physical Items)
    /// Tuân thủ quy chuẩn Zero GC và New Input System.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance { get; private set; }

        public const int SLOT_COUNT = 3;

        [Header("Danh sách 3 Slot Kho đồ")]
        [SerializeField] private InventorySlotData[] _slots = new InventorySlotData[SLOT_COUNT];
        [SerializeField] private int _activeSlotIndex = 0;
        private bool _isHolstered = false;

        [Header("Mô hình FPS hiển thị trên tay")]
        [SerializeField] private GameObject _fpsMopModel;
        [SerializeField] private GameObject _fpsScannerModel;
        [SerializeField] private ShotgunController _shotgunController;
        [SerializeField] private PlayerInteraction _playerInteraction;

        public int ActiveSlotIndex => _activeSlotIndex;
        public bool IsHolstered => _isHolstered;
        public InventorySlotData ActiveSlotItem => _slots != null && _activeSlotIndex >= 0 && _activeSlotIndex < _slots.Length ? _slots[_activeSlotIndex] : null;

        private void Awake()
        {
            if (Instance == null) Instance = this;

            if (_shotgunController == null) _shotgunController = GetComponent<ShotgunController>();
            if (_playerInteraction == null) _playerInteraction = GetComponent<PlayerInteraction>();

            if (_slots == null || _slots.Length != SLOT_COUNT)
            {
                _slots = new InventorySlotData[SLOT_COUNT];
            }
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                if (_slots[i] == null) _slots[i] = new InventorySlotData();
            }
        }

        private void OnEnable()
        {
            GameEvents.OnShotgunUnlocked += HandleShotgunUnlocked;
            GameEvents.OnWeaponAmmoChanged += HandleWeaponAmmoChanged;
            GameEvents.OnWeaponReloaded += HandleWeaponReloaded;
            GameEvents.OnWeaponFired += HandleWeaponFired;
        }

        private void OnDisable()
        {
            GameEvents.OnShotgunUnlocked -= HandleShotgunUnlocked;
            GameEvents.OnWeaponAmmoChanged -= HandleWeaponAmmoChanged;
            GameEvents.OnWeaponReloaded -= HandleWeaponReloaded;
            GameEvents.OnWeaponFired -= HandleWeaponFired;
        }

        private void Start()
        {
            // Mặc định ban đầu chọn Slot 0 (Bàn tay tự do)
            SelectSlot(0);
        }

        private void Update()
        {
            PollInventoryInput();
        }

        /// <summary>
        /// Đọc phím 1, 2, 3, Q, G/X hoặc con lăn chuột từ New Input System để đổi slot nhanh hoặc cất/rút đồ.
        /// </summary>
        private void PollInventoryInput()
        {
            if (Keyboard.current == null) return;

            // Phím số 1: Chọn Ô 1 (Slot 0) - nếu đang cầm thì cất đi (Toggle Holster)
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                if (_activeSlotIndex == 0 && !_slots[0].IsEmpty)
                {
                    ToggleHolster();
                }
                else
                {
                    _isHolstered = false;
                    SelectSlot(0);
                }
            }
            // Phím số 2: Chọn Ô 2 (Slot 1) - nếu đang cầm thì cất đi (Toggle Holster)
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                if (_activeSlotIndex == 1 && !_slots[1].IsEmpty)
                {
                    ToggleHolster();
                }
                else
                {
                    _isHolstered = false;
                    SelectSlot(1);
                }
            }
            // Phím số 3: Chọn Ô 3 (Slot 2) - nếu đang cầm thì cất đi (Toggle Holster)
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                if (_activeSlotIndex == 2 && !_slots[2].IsEmpty)
                {
                    ToggleHolster();
                }
                else
                {
                    _isHolstered = false;
                    SelectSlot(2);
                }
            }
            // Phím G hoặc X: Cất/Rút món đồ trên tay (Holster)
            else if (Keyboard.current.gKey.wasPressedThisFrame || Keyboard.current.xKey.wasPressedThisFrame)
            {
                ToggleHolster();
            }
            // Phím Q: Chuyển đổi vòng lặp qua lại giữa các slot
            else if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                CycleNextSlot();
            }

            // Hỗ trợ cuộn chuột (Mouse Scroll Wheel)
            if (Mouse.current != null)
            {
                float scrollY = Mouse.current.scroll.ReadValue().y;
                if (scrollY > 0.1f)
                {
                    CyclePreviousSlot();
                }
                else if (scrollY < -0.1f)
                {
                    CycleNextSlot();
                }
            }
        }

        /// <summary>
        /// Bật/tắt trạng thái cất đồ vào người (Holster / Stow Item).
        /// </summary>
        public void ToggleHolster()
        {
            _isHolstered = !_isHolstered;
            SyncActiveSlotVisuals();
            GameEvents.TriggerInventorySlotChanged(_activeSlotIndex);
        }

        private void CycleNextSlot()
        {
            _isHolstered = false;
            int next = (_activeSlotIndex + 1) % SLOT_COUNT;
            SelectSlot(next);
        }

        private void CyclePreviousSlot()
        {
            _isHolstered = false;
            int prev = (_activeSlotIndex - 1 + SLOT_COUNT) % SLOT_COUNT;
            SelectSlot(prev);
        }

        /// <summary>
        /// Kiểm tra kho đồ đã có loại vật phẩm này chưa.
        /// </summary>
        public bool HasItemType(InventoryItemType type)
        {
            if (_slots == null) return false;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null && _slots[i].ItemType == type) return true;
            }
            return false;
        }

        /// <summary>
        /// Tìm vị trí slot chứa loại vật phẩm tương ứng (-1 nếu không có).
        /// </summary>
        public int FindSlotIndex(InventoryItemType type)
        {
            if (_slots == null) return -1;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null && _slots[i].ItemType == type) return i;
            }
            return -1;
        }

        /// <summary>
        /// Lấy thông tin dữ liệu slot theo chỉ mục.
        /// </summary>
        public InventorySlotData GetSlot(int index)
        {
            if (_slots == null || index < 0 || index >= _slots.Length) return null;
            return _slots[index];
        }

        /// <summary>
        /// Nhặt vật phẩm mới vào kho đồ động:
        /// "Cái nào nhặt trước thì ở ô thứ nhất xong sẽ lần lượt vào các ô tiếp theo".
        /// </summary>
        public bool TryAcquireItem(InventoryItemType type, string displayName, int initialAmmo = 0, int initialReserve = 0, Transform physicalObj = null, Rigidbody physicalRb = null)
        {
            // 1. Nếu đã sở hữu món đồ này trong kho đồ, tự động rút món đồ đó ra
            int existingSlot = FindSlotIndex(type);
            if (existingSlot != -1)
            {
                SelectSlot(existingSlot);
                return true;
            }

            // 2. Tìm ô trống ĐẦU TIÊN (Ưu tiên Ô 1, rồi đến Ô 2, Ô 3)
            int targetSlot = -1;
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                if (_slots[i].IsEmpty)
                {
                    targetSlot = i;
                    break;
                }
            }

            if (targetSlot == -1)
            {
                // Nếu cả 3 ô đều đầy, kiểm tra ô hiện tại có phải đồ vật cầm tay không để thay thế
                if (_slots[_activeSlotIndex].ItemType == InventoryItemType.PhysicalItem)
                {
                    targetSlot = _activeSlotIndex;
                }
                else
                {
                    return false;
                }
            }

            // 3. Gán dữ liệu vào slot
            _slots[targetSlot].ItemType = type;
            _slots[targetSlot].DisplayName = displayName;
            _slots[targetSlot].AmmoCount = initialAmmo;
            _slots[targetSlot].ReserveAmmo = initialReserve;
            _slots[targetSlot].PhysicalTransform = physicalObj;
            _slots[targetSlot].PhysicalRb = physicalRb;

            // 4. Tự động chọn và rút món đồ vừa nhặt
            SelectSlot(targetSlot);

            // Bắn sự kiện cập nhật giao diện
            GameEvents.TriggerHeldItemChanged(displayName);
            GameEvents.TriggerInventorySlotChanged(_activeSlotIndex);

            return true;
        }

        /// <summary>
        /// Xóa loại vật phẩm tương ứng ra khỏi kho đồ (ví dụ cất cây lau nhà vào xô).
        /// </summary>
        public bool RemoveItemType(InventoryItemType type)
        {
            int slot = FindSlotIndex(type);
            if (slot >= 0)
            {
                ClearSlot(slot);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Xóa dữ liệu một slot về trạng thái trống (Empty).
        /// </summary>
        public void ClearSlot(int slotIndex)
        {
            if (_slots == null || slotIndex < 0 || slotIndex >= _slots.Length) return;

            bool wasActive = (_activeSlotIndex == slotIndex);
            _slots[slotIndex].Clear();

            if (wasActive)
            {
                SyncActiveSlotVisuals();
                GameEvents.TriggerHeldItemChanged(null);
            }
            GameEvents.TriggerInventorySlotChanged(_activeSlotIndex);
        }

        /// <summary>
        /// Kích hoạt slot tương ứng và đồng bộ trạng thái mô hình cầm trên tay.
        /// </summary>
        public void SelectSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SLOT_COUNT) return;

            _activeSlotIndex = slotIndex;
            SyncActiveSlotVisuals();

            // Bắn sự kiện cập nhật giao diện kho đồ trên HUD
            GameEvents.TriggerInventorySlotChanged(_activeSlotIndex);
        }

        /// <summary>
        /// Đồng bộ mô hình súng, chổi lau hoặc đồ vật vật lý theo slot đang chọn.
        /// </summary>
        private void SyncActiveSlotVisuals()
        {
            // Nếu đang ở trạng thái cất đồ vào người (Holstered) thì ẩn toàn bộ vũ khí/dụng cụ trên tay
            if (_isHolstered)
            {
                if (_shotgunController != null) _shotgunController.SetWeaponEquipped(false);
                if (_fpsMopModel != null) _fpsMopModel.SetActive(false);
                if (_fpsScannerModel != null) _fpsScannerModel.SetActive(false);
                GameEvents.TriggerWeaponEquipped(false);
                return;
            }

            InventorySlotData active = ActiveSlotItem;
            InventoryItemType currentType = active != null ? active.ItemType : InventoryItemType.Empty;

            switch (currentType)
            {
                case InventoryItemType.Gun:
                    if (_fpsMopModel != null) _fpsMopModel.SetActive(false);
                    if (_fpsScannerModel != null) _fpsScannerModel.SetActive(false);
                    if (_shotgunController != null) _shotgunController.SetWeaponEquipped(true);
                    GameEvents.TriggerWeaponEquipped(true);
                    if (active != null)
                    {
                        GameEvents.TriggerWeaponAmmoChanged(active.AmmoCount, active.ReserveAmmo);
                    }
                    break;

                case InventoryItemType.Mop:
                    if (_shotgunController != null) _shotgunController.SetWeaponEquipped(false);
                    if (_fpsScannerModel != null) _fpsScannerModel.SetActive(false);
                    if (_fpsMopModel != null) _fpsMopModel.SetActive(true);
                    GameEvents.TriggerWeaponEquipped(false);
                    break;

                case InventoryItemType.Scanner:
                    if (_shotgunController != null) _shotgunController.SetWeaponEquipped(false);
                    if (_fpsMopModel != null) _fpsMopModel.SetActive(false);
                    if (_fpsScannerModel != null) _fpsScannerModel.SetActive(true);
                    GameEvents.TriggerWeaponEquipped(false);
                    break;

                case InventoryItemType.PhysicalItem:
                case InventoryItemType.Empty:
                default:
                    if (_shotgunController != null) _shotgunController.SetWeaponEquipped(false);
                    if (_fpsMopModel != null) _fpsMopModel.SetActive(false);
                    if (_fpsScannerModel != null) _fpsScannerModel.SetActive(false);
                    GameEvents.TriggerWeaponEquipped(false);
                    break;
            }
        }

        /// <summary>
        /// Thiết lập mô hình FPS Cây lau nhà trên tay nhân vật từ Builder.
        /// </summary>
        public void SetFPSMopModel(GameObject mopModel)
        {
            _fpsMopModel = mopModel;
            if (_fpsMopModel != null)
            {
                _fpsMopModel.SetActive(!_isHolstered && ActiveSlotItem != null && ActiveSlotItem.ItemType == InventoryItemType.Mop);
            }
        }

        /// <summary>
        /// Thiết lập mô hình FPS Máy quét mã vạch trên tay nhân vật từ Builder.
        /// </summary>
        public void SetFPSScannerModel(GameObject scannerModel)
        {
            _fpsScannerModel = scannerModel;
            if (_fpsScannerModel != null)
            {
                _fpsScannerModel.SetActive(!_isHolstered && ActiveSlotItem != null && ActiveSlotItem.ItemType == InventoryItemType.Scanner);
            }
        }

        private void HandleShotgunUnlocked()
        {
            // Nhặt súng từ ngăn kéo vào ô trống đầu tiên (theo thứ tự)
            TryAcquireItem(InventoryItemType.Gun, "SÚNG 9MM", 5, 15);
        }

        private void HandleWeaponAmmoChanged(int currentMag, int reserveAmmo)
        {
            int gunSlot = FindSlotIndex(InventoryItemType.Gun);
            if (gunSlot >= 0 && _slots[gunSlot] != null)
            {
                _slots[gunSlot].AmmoCount = currentMag;
                _slots[gunSlot].ReserveAmmo = reserveAmmo;
                GameEvents.TriggerInventorySlotChanged(_activeSlotIndex);
            }
        }

        private void HandleWeaponReloaded(int currentMag, int reserveAmmo)
        {
            HandleWeaponAmmoChanged(currentMag, reserveAmmo);
        }

        private void HandleWeaponFired(int currentMag, int reserveAmmo, Vector3 recoil)
        {
            HandleWeaponAmmoChanged(currentMag, reserveAmmo);
        }
    }
}
