using UnityEngine;

namespace MidnightMart.Core
{
    /// <summary>
    /// Tiện ích tính toán độ cong uốn lượn (Curved Spline / S-Curve) của con đường rừng Blackwood.
    /// Đảm bảo tính nhất quán tuyệt đối giữa:
    /// - StoreSceneBuilder (Dựng đường nhựa, vạch kẻ, hộ lan, đèn đường và cây cối bám theo lề cong).
    /// - MotorbikeRideController (Xe máy tự động bám theo tim đường cong, đánh lái mượt mà).
    /// </summary>
    public static class RoadCurveUtility
    {
        public const float ROAD_START_Z = -280.0f;
        public const float ROAD_END_Z = -10.0f;
        public const float CURVE_AMPLITUDE = 6.0f; // Độ uốn cong tối đa sang trái/phải (~4.0m)

        /// <summary>
        /// Tính toán tọa độ X của tim đường tại vị trí tọa độ Z bất kỳ.
        /// Sử dụng hàm cửa sổ sin^2 mượt mà để:
        /// - Tại nhà (Z <= -280m): Đường thẳng tắp (X = 0, góc 0 độ).
        /// - Đoạn giữa (-280m -> -10m): Uốn lượn 2 chu kỳ S-Curve mềm mại qua rừng thông u ám dài 270m.
        /// - Tại siêu thị (Z >= -10m): Nắn thẳng hoàn hảo (X = 0, góc 0 độ) vào trước cửa kính.
        /// </summary>
        public static float GetRoadCenterX(float z)
        {
            if (z <= ROAD_START_Z || z >= ROAD_END_Z)
            {
                return 0f;
            }

            float t = (z - ROAD_START_Z) / (ROAD_END_Z - ROAD_START_Z); // 0.0 -> 1.0
            float window = Mathf.Sin(Mathf.PI * t);
            float smoothWindow = window * window; // Đảm bảo đạo hàm = 0 tại 2 đầu

            // Uốn lượn 2 nhịp S-Curve tự nhiên xuyên qua rừng rậm
            return smoothWindow * CURVE_AMPLITUDE * Mathf.Sin(4f * Mathf.PI * t);
        }

        /// <summary>
        /// Tính toán góc tiếp tuyến (Góc xoay Yaw Y tính bằng độ) của con đường tại tọa độ Z.
        /// Giúp xe máy và các thanh hộ lan tự động xoay nghiêng theo đúng độ uốn của khúc cua.
        /// </summary>
        public static float GetRoadTangentAngle(float z)
        {
            float delta = 0.5f;
            float x1 = GetRoadCenterX(z - delta);
            float x2 = GetRoadCenterX(z + delta);
            float dx = x2 - x1;
            float dz = delta * 2f;

            return Mathf.Atan2(dx, dz) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Lấy vector hướng tiến tiếp tuyến của con đường tại tọa độ Z.
        /// </summary>
        public static Vector3 GetRoadForward(float z)
        {
            float angle = GetRoadTangentAngle(z);
            return Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
        }
    }
}

