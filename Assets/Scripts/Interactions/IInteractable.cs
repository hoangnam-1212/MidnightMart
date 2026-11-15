using UnityEngine;

namespace MidnightMart.Interactions
{
    /// <summary>
    /// Giao diện chuẩn cho tất cả các đối tượng có thể tương tác trong thế giới 3D của cửa hàng.
    /// Áp dụng cho: Cửa, công tắc đèn, máy quét mã vạch, ngăn kéo, món hàng, đồ bừa bộn.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Chuỗi văn bản gợi ý hành động hiển thị trên màn hình người chơi khi nhìn vào vật thể.
        /// Ví dụ: "Nhấn [E] để nhặt bánh snack", "Nhấn [E] để mở ngăn kéo".
        /// </summary>
        string GetInteractionPrompt();

        /// <summary>
        /// Kích hoạt hành động tương tác chính khi người chơi nhấn nút tương tác (mặc định phím E).
        /// </summary>
        /// <param name="playerTransform">Vị trí Transform của người chơi để tính toán góc nhìn/hướng ném.</param>
        void OnInteract(Transform playerTransform);

        /// <summary>
        /// Kiểm tra xem vật thể hiện tại có đang cho phép tương tác hay không.
        /// </summary>
        bool CanInteract();

        /// <summary>
        /// Bật hoặc tắt hiệu ứng viền sáng trắng (Highlight) khi người chơi nhìn thẳng vào vật thể.
        /// </summary>
        /// <param name="isHighlighted">true nếu người chơi đang lia tâm ngắm vào vật thể.</param>
        void SetHighlight(bool isHighlighted);
    }
}

