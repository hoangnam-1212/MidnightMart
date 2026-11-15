using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Player;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Ngăn kéo bí mật dưới quầy thu ngân (Secret Under-counter Drawer).
    /// Chứa chiếc điện thoại vỡ của nhân viên cũ David và khẩu súng săn 2 nòng (Double-Barrel Shotgun).
    /// Hỗ trợ mở ra, nhặt súng, và đóng lại gọn gàng để không gây vướng đường đi của người chơi.
    /// </summary>
    public class SecretDrawer : MonoBehaviour, IInteractable
    {
        [Header("Cấu hình Ngăn kéo")]
        [SerializeField] private Transform _drawerModel;
        [SerializeField] private Vector3 _openedOffset = new Vector3(-0.45f, 0f, 0f);

        [Header("Vật phẩm bên trong")]
        [SerializeField] private GameObject _davidsBrokenPhone;
        [SerializeField] private GameObject _shotgunWorldModel;

        [Header("Âm thanh")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _openStuckSound;
        [SerializeField] private AudioClip _shotgunCockSound;

        private bool _isOpen;
        private bool _hasTakenShotgun;
        private Vector3 _closedPos;
        private InteractionHighlighter _highlighter;

        public bool IsOpen => _isOpen;

        private void Awake()
        {
            if (_drawerModel != null)
            {
                _closedPos = _drawerModel.localPosition;
            }

            _highlighter = GetComponent<InteractionHighlighter>();
            if (_highlighter == null)
            {
                _highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }
        }

        public string GetInteractionPrompt()
        {
            if (!_isOpen)
            {
                return "[E] Kéo mở ngăn kéo";
            }

            if (!_hasTakenShotgun)
            {
                return "[E] Nhặt Súng Lục 9mm & Đóng lại";
            }

            return "[E] Đóng ngăn kéo lại";
        }

        public bool CanInteract()
        {
            return true;
        }

        public void SetHighlight(bool isHighlighted)
        {
            _highlighter?.SetHighlight(isHighlighted);
        }

        public void OnInteract(Transform playerTransform)
        {
            if (!_isOpen)
            {
                OpenDrawer();
            }
            else if (!_hasTakenShotgun)
            {
                TakeShotgun();
                // Tự động đóng lại ngay sau khi nhặt súng để giải phóng lối đi quầy thu ngân
                CloseDrawer();
            }
            else
            {
                CloseDrawer();
            }
        }

        /// <summary>
        /// Mở bung ngăn kéo ra phía sau quầy để người chơi quan sát và nhặt súng.
        /// </summary>
        public void OpenDrawer()
        {
            _isOpen = true;

            if (_drawerModel != null)
            {
                _drawerModel.localPosition = _closedPos + _openedOffset;
            }

            if (_audioSource != null && _openStuckSound != null)
            {
                _audioSource.PlayOneShot(_openStuckSound);
            }

            // Kích hoạt hiển thị chiếc điện thoại và súng săn (nếu chưa nhặt)
            if (_davidsBrokenPhone != null) _davidsBrokenPhone.SetActive(true);
            if (_shotgunWorldModel != null && !_hasTakenShotgun) _shotgunWorldModel.SetActive(true);

            if (!_hasTakenShotgun)
            {
                GameEvents.TriggerSubtitle("Nhân viên mới", "Cái quái gì thế này...? Điện thoại của David vỡ nát... và một khẩu súng lục 9mm nạp sẵn đạn?!", 4f);
            }
        }

        /// <summary>
        /// Đóng ngăn kéo lại gọn gàng để không cản trở lối đi của người chơi.
        /// </summary>
        public void CloseDrawer()
        {
            _isOpen = false;

            if (_drawerModel != null)
            {
                _drawerModel.localPosition = _closedPos;
            }

            if (_audioSource != null && _openStuckSound != null)
            {
                _audioSource.PlayOneShot(_openStuckSound);
            }
        }

        /// <summary>
        /// Người chơi nhặt khẩu súng săn vào người.
        /// </summary>
        public void TakeShotgun()
        {
            _hasTakenShotgun = true;

            if (_shotgunWorldModel != null)
            {
                _shotgunWorldModel.SetActive(false);
            }

            if (_audioSource != null && _shotgunCockSound != null)
            {
                _audioSource.PlayOneShot(_shotgunCockSound);
            }

            // Trang bị súng cho nhân vật vào ô trống tiếp theo của kho đồ động
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.TryAcquireItem(InventoryItemType.Gun, "SÚNG 9MM", 5, 15);
            }
            GameEvents.TriggerWeaponEquipped(true);
            GameEvents.TriggerWeaponReloaded(5, 15);

            // Báo cho Tutorial Manager hoàn thành bước nhặt vũ khí
            Tutorial.TutorialManager.Instance?.ReportShotgunAcquired();
        }
    }
}
