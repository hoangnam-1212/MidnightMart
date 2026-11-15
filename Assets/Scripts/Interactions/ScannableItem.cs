using UnityEngine;
using MidnightMart.Core;
using MidnightMart.Player;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Đại diện cho các món hàng hóa có tem mã vạch trong tiệm (Bao thuốc lá, lon nước, gói mì).
    /// Hỗ trợ cầm nắm vật lý và cho phép máy quét BarcodeScanner nhận diện để tính tiền.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ScannableItem : MonoBehaviour, IInteractable
    {
        [Header("Thông tin Sản phẩm")]
        [SerializeField] private string _productName = "Bao thuốc lá Gold";
        [SerializeField] private float _price = 8.50f;
        [SerializeField] private Transform _barcodeTransform;

        private Rigidbody _rigidbody;
        private bool _isScanned;
        private InteractionHighlighter _highlighter;

        public string ProductName => _productName;
        public float Price => _price;
        public bool IsScanned => _isScanned;
        public Transform BarcodeTransform => _barcodeTransform != null ? _barcodeTransform : transform;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _highlighter = GetComponent<InteractionHighlighter>();
            if (_highlighter == null)
            {
                _highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }
        }

        public string GetInteractionPrompt()
        {
            if (!_isScanned)
            {
                return $"[E / Chuột Trái] Quét mã: {_productName} (${_price:F2})";
            }
            return $"<color=#00FF88>✓ Đã quét mã: {_productName} (${_price:F2})</color>";
        }

        public bool CanInteract()
        {
            // Chỉ cho phép tương tác quét mã khi chưa quét; tuyệt đối không cho người chơi cầm lên tay
            return !_isScanned;
        }

        public void SetupItem(string productName, float price)
        {
            _productName = productName;
            _price = price;
            _isScanned = false;

            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = true;
                _rigidbody.useGravity = false;
            }
        }

        public void SetHighlight(bool isHighlighted)
        {
            if (_isScanned)
            {
                _highlighter?.SetHighlight(false);
                return;
            }
            _highlighter?.SetHighlight(isHighlighted);
        }

        public void OnInteract(Transform playerTransform)
        {
            if (_isScanned) return;

            SetHighlight(false);

            // Tự động quét mã vạch và báo tít khi người chơi nhấn E
            MarkAsScanned();
            AudioClip beep = ProceduralAudioGenerator.CreateBarcodeBeepClip();
            if (beep != null)
            {
                AudioSource.PlayClipAtPoint(beep, transform.position);
            }
            GameEvents.TriggerItemScanned(_productName, _price);
        }

        /// <summary>
        /// Đánh dấu món hàng đã được quét mã vạch thành công.
        /// </summary>
        public void MarkAsScanned()
        {
            _isScanned = true;
        }
    }
}

