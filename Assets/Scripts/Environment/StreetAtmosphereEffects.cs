using UnityEngine;

namespace MidnightMart.Environment
{
    /// <summary>
    /// Hiệu ứng đèn đường chập chờn ma quái trên con đường rừng âm u (Street Lamp Flicker).
    /// Hoạt động Zero GC, tạo cảm giác bóng tối chập chờn khi người chơi chạy xe máy qua.
    /// </summary>
    public class StreetLampFlicker : MonoBehaviour
    {
        [SerializeField] private Light _targetLight;
        [SerializeField] private float _baseIntensity = 350f;
        [SerializeField] private float _flickerSpeed = 12f;
        [SerializeField] private float _dropChance = 0.08f;

        private float _timeOffset;

        private void Awake()
        {
            if (_targetLight == null) _targetLight = GetComponent<Light>();
            if (_targetLight != null && _targetLight.intensity > 0f)
            {
                _baseIntensity = _targetLight.intensity;
            }
            _timeOffset = Random.Range(0f, 100f);
        }

        private void Update()
        {
            if (_targetLight == null) return;

            // Tính toán độ sáng chập chờn bằng sóng Perlin Noise
            float noise = Mathf.PerlinNoise((Time.time + _timeOffset) * _flickerSpeed, 0f);
            float intensity = _baseIntensity * (0.75f + noise * 0.45f);

            // Thỉnh thoảng sụt nguồn điện chớp tắt đột ngột
            if (Random.value < _dropChance)
            {
                intensity *= 0.15f;
            }

            _targetLight.intensity = intensity;
        }
    }

    /// <summary>
    /// Hiệu ứng đèn khẩn cấp màu vàng cam nhấp nháy trên nóc xe cảnh sát bỏ hoang dưới mương (Hazard Pulse).
    /// </summary>
    public class HazardLightPulse : MonoBehaviour
    {
        [SerializeField] private Light _hazardLight;
        [SerializeField] private float _pulseInterval = 1.2f;
        [SerializeField] private float _peakIntensity = 180f;

        private float _timer;

        private void Awake()
        {
            if (_hazardLight == null) _hazardLight = GetComponent<Light>();
            if (_hazardLight != null && _hazardLight.intensity > 0f)
            {
                _peakIntensity = _hazardLight.intensity;
            }
        }

        private void Update()
        {
            if (_hazardLight == null) return;

            _timer += Time.deltaTime;
            if (_timer >= _pulseInterval)
            {
                _timer -= _pulseInterval;
            }

            // Nháy đúp (Double Flash) như đèn cứu hộ thực tế
            float flash = 0f;
            if (_timer < 0.15f)
            {
                flash = Mathf.Sin((_timer / 0.15f) * Mathf.PI);
            }
            else if (_timer > 0.25f && _timer < 0.40f)
            {
                flash = Mathf.Sin(((_timer - 0.25f) / 0.15f) * Mathf.PI);
            }

            _hazardLight.intensity = flash * _peakIntensity;
        }
    }
}

