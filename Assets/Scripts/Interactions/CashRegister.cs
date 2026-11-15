using UnityEngine;
using TMPro;
using MidnightMart.Core;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Máy tính tiền và khay tiền tại quầy thu ngân (Cash Register).
    /// Quản lý việc tính tổng tiền các món hàng, nhận tiền từ khách và thối tiền thừa.
    /// </summary>
    public class CashRegister : MonoBehaviour, IInteractable
    {
        [Header("Màn hình Hiển thị Giá tiền")]
        [SerializeField] private TMP_Text _screenDisplayTMP;
        [SerializeField] private Transform _drawerTransform;
        [SerializeField] private Vector3 _drawerOpenOffset = new Vector3(0f, 0f, 0.4f);

        [Header("Âm thanh Cơ khí")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _drawerOpenSound; // Tiếng "Keng!"
        [SerializeField] private AudioClip _buttonBeepSound;

        // Trạng thái đơn hàng
        private float _currentTotal;
        private float _customerCashReceived;
        private bool _isDrawerOpen;
        private Vector3 _drawerClosedPos;
        private bool _isWaitingForPayment;
        private InteractionHighlighter _highlighter;

        public float CurrentTotal => _currentTotal;
        public bool IsDrawerOpen => _isDrawerOpen;

        private void Awake()
        {
            if (_drawerTransform != null)
            {
                _drawerClosedPos = _drawerTransform.localPosition;
            }
            _highlighter = GetComponent<InteractionHighlighter>();
            if (_highlighter == null)
            {
                _highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0.5f;

#if UNITY_EDITOR
            if (_drawerOpenSound == null)
            {
                _drawerOpenSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Cash_Register_Ding.wav");
            }
#endif
            if (_drawerOpenSound == null)
            {
                _drawerOpenSound = ProceduralAudioGenerator.CreateCashRegisterDingClip();
            }

            UpdateScreenText("$0.00");
        }

        private void OnEnable()
        {
            GameEvents.OnItemScanned += HandleItemScanned;
            GameEvents.OnCustomerPaid += HandleCustomerPaid;
        }

        private void OnDisable()
        {
            GameEvents.OnItemScanned -= HandleItemScanned;
            GameEvents.OnCustomerPaid -= HandleCustomerPaid;
        }

        public string GetInteractionPrompt()
        {
            if (_isWaitingForPayment)
            {
                float change = Mathf.Max(0f, _customerCashReceived - _currentTotal);
                return $"[E] Thối lại ${change:F2}";
            }
            return _isDrawerOpen ? "[E] Đóng ngăn kéo tiền" : "[E] Mở ngăn kéo tiền";
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
            if (_isWaitingForPayment)
            {
                // Thối tiền cho khách và hoàn tất giao dịch
                CompleteTransaction();
            }
            else
            {
                ToggleDrawer();
            }
        }

        /// <summary>
        /// Nhận dữ liệu từ máy quét mã vạch để cộng dồn giá tiền.
        /// </summary>
        private void HandleItemScanned(string itemName, float price)
        {
            _currentTotal += price;
            UpdateScreenText($"${_currentTotal:F2}");
        }

        /// <summary>
        /// Nhận sự kiện khi khách đặt tiền lên quầy.
        /// </summary>
        private void HandleCustomerPaid(float total, float cashReceived)
        {
            _currentTotal = total;
            _customerCashReceived = cashReceived;
            _isWaitingForPayment = true;

            float change = _customerCashReceived - _currentTotal;
            UpdateScreenText($"NHẬN: ${_customerCashReceived:F2}\nTHỐI: ${change:F2}");
        }

        /// <summary>
        /// Hoàn tất thanh toán, thối tiền và phát sự kiện thành công.
        /// </summary>
        private void CompleteTransaction()
        {
            _isWaitingForPayment = false;
            _currentTotal = 0f;
            _customerCashReceived = 0f;

            if (_audioSource != null && _drawerOpenSound != null)
            {
                _audioSource.PlayOneShot(_drawerOpenSound);
            }

            UpdateScreenText("CẢM ƠN QUÝ KHÁCH!");

            // Thông báo toàn cục hoàn tất đơn hàng
            GameEvents.TriggerCheckoutCompleted();
        }

        /// <summary>
        /// Đóng/mở khay ngăn kéo máy tính tiền.
        /// </summary>
        public void ToggleDrawer()
        {
            _isDrawerOpen = !_isDrawerOpen;

            if (_drawerTransform != null)
            {
                _drawerTransform.localPosition = _isDrawerOpen ? (_drawerClosedPos + _drawerOpenOffset) : _drawerClosedPos;
            }

            if (_audioSource != null && _drawerOpenSound != null)
            {
                _audioSource.PlayOneShot(_drawerOpenSound);
            }
        }

        private void UpdateScreenText(string text)
        {
            if (_screenDisplayTMP != null)
            {
                _screenDisplayTMP.text = text;
            }
        }
    }
}

