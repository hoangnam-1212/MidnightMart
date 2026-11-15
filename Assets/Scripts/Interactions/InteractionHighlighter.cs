using UnityEngine;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Tạo hiệu ứng viền trắng (White Outline) và phát sáng khi người chơi lia tâm ngắm vào vật thể.
    /// Hoạt động độc lập và tối ưu: Sử dụng MaterialPropertyBlock để không sinh rác bộ nhớ (Zero GC).
    /// Hỗ trợ cả Unity HDRP và Standard Render Pipeline.
    /// </summary>
    [DisallowMultipleComponent]
    public class InteractionHighlighter : MonoBehaviour
    {
        [Header("Cấu hình Viền sáng")]
        [SerializeField] private Color _highlightColor = Color.white;
        [SerializeField] private float _outlineScaleMultiplier = 1.06f;

        private Renderer _targetRenderer;
        private GameObject _outlineMeshObject;
        private MaterialPropertyBlock _propertyBlock;
        private bool _isHighlighted;

        private static readonly int EmissiveColorId = Shader.PropertyToID("_EmissiveColor");
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _targetRenderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();

            CreateOutlineShell();
        }

        /// <summary>
        /// Tạo một hình khối vỏ bọc màu trắng phát sáng (Inverted Hull Outline) bao quanh vật thể.
        /// Sử dụng Cull Front hoặc Transparent Additive để viền trắng ôm sát biên dạng mà không che lấp vật thể gốc.
        /// </summary>
        private void CreateOutlineShell()
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) return;

            _outlineMeshObject = new GameObject("Highlight_White_Outline");
            _outlineMeshObject.transform.SetParent(transform);
            _outlineMeshObject.transform.localPosition = Vector3.zero;
            _outlineMeshObject.transform.localRotation = Quaternion.identity;
            _outlineMeshObject.transform.localScale = Vector3.one * _outlineScaleMultiplier;

            MeshFilter outlineMf = _outlineMeshObject.AddComponent<MeshFilter>();
            outlineMf.sharedMesh = mf.sharedMesh;

            MeshRenderer outlineMr = _outlineMeshObject.AddComponent<MeshRenderer>();
            
            // Tìm Shader Unlit hoặc HDRP Unlit để tạo màu trắng phát sáng
            Shader unlitShader = Shader.Find("HDRP/Unlit");
            if (unlitShader == null) unlitShader = Shader.Find("Unlit/Color");
            if (unlitShader == null) unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
            if (unlitShader == null) unlitShader = Shader.Find("Standard");

            Material outlineMat = new Material(unlitShader);
            outlineMat.name = "Mat_White_Outline_Shell";
            outlineMat.color = _highlightColor;
            
            // Cấu hình vật liệu HDRP Unlit viền sáng
            if (outlineMat.HasProperty(BaseColorId)) outlineMat.SetColor(BaseColorId, _highlightColor);
            if (outlineMat.HasProperty(EmissiveColorId)) outlineMat.SetColor(EmissiveColorId, _highlightColor * 3.0f);
            
            // Inverted Hull: Loại bỏ mặt trước (Cull Front) để chỉ hiển thị vành ngoài viền trắng bao quanh
            if (outlineMat.HasProperty("_CullMode")) outlineMat.SetFloat("_CullMode", 1f); // 1 = Front Cull
            if (outlineMat.HasProperty("_Cull")) outlineMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Front);

            outlineMr.sharedMaterial = outlineMat;
            outlineMr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            outlineMr.receiveShadows = false;

            // Mặc định ẩn viền trắng đi khi chưa lia chuột vào
            _outlineMeshObject.SetActive(false);
        }

        /// <summary>
        /// Bật hoặc tắt hiệu ứng viền trắng quanh vật thể.
        /// </summary>
        /// <param name="active">true nếu người chơi đang nhìn vào vật thể.</param>
        public void SetHighlight(bool active)
        {
            if (_isHighlighted == active) return;
            _isHighlighted = active;

            // 1. Bật/tắt vỏ viền trắng bao quanh
            if (_outlineMeshObject != null)
            {
                _outlineMeshObject.SetActive(_isHighlighted);
            }

            // 2. Kích hoạt hiệu ứng phát sáng nhẹ trên vật liệu chính (Zero GC)
            if (_targetRenderer != null)
            {
                _targetRenderer.GetPropertyBlock(_propertyBlock);
                Color glowColor = _isHighlighted ? (_highlightColor * 1.5f) : Color.black;
                _propertyBlock.SetColor(EmissiveColorId, glowColor);
                _targetRenderer.SetPropertyBlock(_propertyBlock);
            }
        }

        private void OnDisable()
        {
            // Tắt viền nếu object bị ẩn đi
            SetHighlight(false);
        }
    }
}
