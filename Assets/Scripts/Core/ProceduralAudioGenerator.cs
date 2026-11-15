using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MidnightMart.Core
{
    /// <summary>
    /// Bộ tổng hợp âm thanh thủ tục (Procedural Audio Synthesizer) cho Midnight Mart.
    /// Tự động sinh ra các hiệu ứng âm thanh (SFX) trong trẻo, vui tai và dễ chịu:
    /// - Tiếng máy quét mã vạch kêu tít (Supermarket Barcode Beep) cực kỳ nịnh tai.
    /// - Tiếng máy tính tiền reo chuông cổ điển (Cash Register Bell Ding) vui nhộn.
    /// - Tiếng chuông cửa chào mừng kiểu cửa hàng tiện lợi Nhật Bản (Convenience Store Ding-Dong).
    /// - Tiếng bong bóng nhặt đồ vật (Satisfying Item Pop).
    /// - Tiếng bật tắt công tắc đèn pin (Crisp Flashlight Click).
    /// - Tiếng kéo mở hộc bàn cơ khí (Tactile Drawer Slide).
    /// Hỗ trợ cả sinh file .wav chuẩn 44.1kHz 16-bit và tạo AudioClip động trong bộ nhớ RAM.
    /// </summary>
    public static class ProceduralAudioGenerator
    {
        public const string SFX_FOLDER = "Assets/Audio/SFX";

        /// <summary>
        /// Sinh ra toàn bộ các file âm thanh WAV chuẩn vào thư mục Assets/Audio/SFX.
        /// </summary>
        public static void GenerateAllSFXFiles()
        {
            if (!Directory.Exists(SFX_FOLDER))
            {
                Directory.CreateDirectory(SFX_FOLDER);
            }

            // 1. Tiếng quét mã vạch tít vui tai (2093Hz C7, ngân 80ms)
            SaveWav(Path.Combine(SFX_FOLDER, "Barcode_Scan_Beep.wav"), GenerateBarcodeScanBeep());

            // 2. Tiếng chuông máy tính tiền keng vui vẻ (Hòa âm E6 1318Hz + E7 2637Hz, ngân 750ms)
            SaveWav(Path.Combine(SFX_FOLDER, "Cash_Register_Ding.wav"), GenerateCashRegisterDing());

            // 3. Tiếng chuông cửa 2 nốt leng keng đón khách (E5 659Hz -> C5 523Hz, ngân 900ms)
            SaveWav(Path.Combine(SFX_FOLDER, "Door_Chime_DingDong.wav"), GenerateDoorChimeDingDong());

            // 4. Tiếng nhặt/dọn đồ pop nịnh tai (Quét tần số 400Hz -> 850Hz, ngân 100ms)
            SaveWav(Path.Combine(SFX_FOLDER, "Item_Pickup_Pop.wav"), GenerateItemPickupPop());

            // 5. Tiếng bật tắt đèn pin (Tactile Click 40ms)
            SaveWav(Path.Combine(SFX_FOLDER, "Flashlight_Click.wav"), GenerateFlashlightClick());

            // 6. Tiếng mở ngăn kéo gỗ/kim loại (Drawer Slide 300ms)
            SaveWav(Path.Combine(SFX_FOLDER, "Drawer_Slide_Open.wav"), GenerateDrawerSlide());

            // 7. Tiếng nổ súng Shotgun uy lực (Shotgun Blast 650ms)
            SaveWav(Path.Combine(SFX_FOLDER, "Shotgun_Blast.wav"), GenerateShotgunBlast());

            // 8. Tiếng súng lục 9mm đanh giòn (Pistol Fire 450ms)
            SaveWav(Path.Combine(SFX_FOLDER, "Pistol_Fire.wav"), GeneratePistolFire());

            // 9. Tiếng nạp đạn súng lục cơ khí (Pistol Reload 850ms)
            SaveWav(Path.Combine(SFX_FOLDER, "Pistol_Reload.wav"), GeneratePistolReload());

            // 10. Tiếng búa kim hỏa gõ buồng đạn rỗng (Empty Click 65ms)
            SaveWav(Path.Combine(SFX_FOLDER, "Gun_Empty_Click.wav"), GenerateGunEmptyClick());

            // 11. Tiếng động cơ xe máy nổ giòn lặp vòng (Motorbike Engine Loop 1.0s)
            SaveWav(Path.Combine(SFX_FOLDER, "Motorbike_Engine_Loop.wav"), GenerateMotorbikeEngineLoop());

#if UNITY_EDITOR
            AssetDatabase.Refresh();
            Debug.Log("[ProceduralAudioGenerator] Đã tạo thành công bộ âm thanh vui tai tại: " + SFX_FOLDER);
#endif
        }

        /// <summary>
        /// Tạo AudioClip trong bộ nhớ động (Runtime Memory) cho tiếng quét mã vạch.
        /// </summary>
        public static AudioClip CreateBarcodeBeepClip()
        {
            float[] samples = GenerateBarcodeScanBeep();
            AudioClip clip = AudioClip.Create("Barcode_Scan_Beep", samples.Length, 1, 44100, false);
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Tạo AudioClip trong bộ nhớ động (Runtime Memory) cho tiếng máy tính tiền keng.
        /// </summary>
        public static AudioClip CreateCashRegisterDingClip()
        {
            float[] samples = GenerateCashRegisterDing();
            AudioClip clip = AudioClip.Create("Cash_Register_Ding", samples.Length, 1, 44100, false);
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Tạo AudioClip trong bộ nhớ động (Runtime Memory) cho tiếng chuông cửa đón khách.
        /// </summary>
        public static AudioClip CreateDoorChimeClip()
        {
            float[] samples = GenerateDoorChimeDingDong();
            AudioClip clip = AudioClip.Create("Door_Chime_DingDong", samples.Length, 1, 44100, false);
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Tạo AudioClip trong bộ nhớ động (Runtime Memory) cho tiếng nhặt đồ bừa bộn.
        /// </summary>
        public static AudioClip CreateItemPickupPopClip()
        {
            float[] samples = GenerateItemPickupPop();
            AudioClip clip = AudioClip.Create("Item_Pickup_Pop", samples.Length, 1, 44100, false);
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Tạo AudioClip trong bộ nhớ động (Runtime Memory) cho tiếng nổ súng Shotgun uy lực.
        /// </summary>
        public static AudioClip CreateShotgunBlastClip()
        {
            float[] samples = GenerateShotgunBlast();
            AudioClip clip = AudioClip.Create("Shotgun_Blast", samples.Length, 1, 44100, false);
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Tạo AudioClip trong bộ nhớ động cho tiếng nổ súng lục 9mm đanh gọn.
        /// </summary>
        public static AudioClip CreatePistolFireClip()
        {
            float[] samples = GeneratePistolFire();
            AudioClip clip = AudioClip.Create("Pistol_Fire", samples.Length, 1, 44100, false);
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Tạo AudioClip trong bộ nhớ động cho tiếng nạp đạn súng lục.
        /// </summary>
        public static AudioClip CreatePistolReloadClip()
        {
            float[] samples = GeneratePistolReload();
            AudioClip clip = AudioClip.Create("Pistol_Reload", samples.Length, 1, 44100, false);
            clip.SetData(samples, 0);
            return clip;
        }

        /// <summary>
        /// Tạo AudioClip trong bộ nhớ động cho tiếng búa đập khi hết đạn (Empty Click).
        /// </summary>
        public static AudioClip CreateGunEmptyClickClip()
        {
            float[] samples = GenerateGunEmptyClick();
            AudioClip clip = AudioClip.Create("Gun_Empty_Click", samples.Length, 1, 44100, false);
            clip.SetData(samples, 0);
            return clip;
        }

        #region Thuật toán tổng hợp sóng âm thanh (Acoustic Synthesizer)

        /// <summary>
        /// Tiếng máy quét siêu thị: Âm sin thuần 2093Hz (C7) với attack 10ms và decay 70ms. Rất trong và sảng khoái.
        /// </summary>
        public static float[] GenerateBarcodeScanBeep()
        {
            int sampleRate = 44100;
            float duration = 0.085f; // 85ms
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            float freq = 2093.0f; // Nốt C7
            float harmonicFreq = 4186.0f; // Nốt C8

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float envelope;
                if (t < 0.01f)
                {
                    envelope = t / 0.01f; // Tăng dần 10ms
                }
                else
                {
                    envelope = Mathf.Exp(-(t - 0.01f) * 35.0f); // Suy hao mượt
                }

                float wave = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.85f + 
                             Mathf.Sin(2f * Mathf.PI * harmonicFreq * t) * 0.15f;

                samples[i] = Mathf.Clamp(wave * envelope * 0.7f, -1.0f, 1.0f);
            }

            return samples;
        }

        /// <summary>
        /// Tiếng máy tính tiền keng vui nhộn: Hòa âm kim loại E6 (1318Hz) + E7 (2637Hz) + B7 (3951Hz) với độ vang dài 750ms.
        /// </summary>
        public static float[] GenerateCashRegisterDing()
        {
            int sampleRate = 44100;
            float duration = 0.75f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            float f1 = 1318.5f; // E6
            float f2 = 2637.0f; // E7
            float f3 = 3951.0f; // B7
            float f4 = 5274.0f; // E8 kim khí sáng

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float attack = Mathf.Clamp01(t / 0.003f); // Attack gõ búa cực nhanh 3ms
                float decay1 = Mathf.Exp(-t * 5.0f);
                float decay2 = Mathf.Exp(-t * 8.0f);
                float decay3 = Mathf.Exp(-t * 14.0f);

                float wave = Mathf.Sin(2f * Mathf.PI * f1 * t) * 0.50f * decay1 +
                             Mathf.Sin(2f * Mathf.PI * f2 * t) * 0.30f * decay1 +
                             Mathf.Sin(2f * Mathf.PI * f3 * t) * 0.15f * decay2 +
                             Mathf.Sin(2f * Mathf.PI * f4 * t) * 0.10f * decay3;

                // Thêm một chút dao động ngân vang (Vibrato)
                float shimmer = 1.0f + 0.05f * Mathf.Sin(2f * Mathf.PI * 18.0f * t);

                samples[i] = Mathf.Clamp(wave * attack * shimmer * 0.75f, -1.0f, 1.0f);
            }

            return samples;
        }

        /// <summary>
        /// Chuông cửa 2 nốt ấm áp phong cách cửa hàng tiện lợi (Ding-Dong: E5 -> C5).
        /// </summary>
        public static float[] GenerateDoorChimeDingDong()
        {
            int sampleRate = 44100;
            float noteDuration = 0.42f;
            int samplesPerNote = (int)(sampleRate * noteDuration);
            int totalSamples = samplesPerNote * 2;
            float[] samples = new float[totalSamples];

            // Nốt 1: Ding (E5 = 659.25Hz)
            float freq1 = 659.25f;
            // Nốt 2: Dong (C5 = 523.25Hz)
            float freq2 = 523.25f;

            for (int i = 0; i < samplesPerNote; i++)
            {
                float t = (float)i / sampleRate;
                float attack = Mathf.Clamp01(t / 0.005f);
                float decay = Mathf.Exp(-t * 6.5f);
                float wave = Mathf.Sin(2f * Mathf.PI * freq1 * t) * 0.7f +
                             Mathf.Sin(2f * Mathf.PI * freq1 * 2f * t) * 0.25f +
                             Mathf.Sin(2f * Mathf.PI * freq1 * 3f * t) * 0.1f;

                samples[i] = Mathf.Clamp(wave * attack * decay * 0.7f, -1.0f, 1.0f);
            }

            for (int i = 0; i < samplesPerNote; i++)
            {
                float t = (float)i / sampleRate;
                float attack = Mathf.Clamp01(t / 0.005f);
                float decay = Mathf.Exp(-t * 5.0f);
                float wave = Mathf.Sin(2f * Mathf.PI * freq2 * t) * 0.75f +
                             Mathf.Sin(2f * Mathf.PI * freq2 * 2f * t) * 0.25f +
                             Mathf.Sin(2f * Mathf.PI * freq2 * 3f * t) * 0.08f;

                samples[samplesPerNote + i] = Mathf.Clamp(wave * attack * decay * 0.7f, -1.0f, 1.0f);
            }

            return samples;
        }

        /// <summary>
        /// Tiếng búp/pop nịnh tai khi nhặt hoặc dọn đồ đạc (Pitch Sweep từ 420Hz lên 880Hz).
        /// </summary>
        public static float[] GenerateItemPickupPop()
        {
            int sampleRate = 44100;
            float duration = 0.11f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            float phase = 0f;
            for (int i = 0; i < totalSamples; i++)
            {
                float progress = (float)i / totalSamples;
                float currentFreq = Mathf.Lerp(420f, 880f, Mathf.Pow(progress, 0.6f));
                phase += 2f * Mathf.PI * currentFreq / sampleRate;

                // Parabolic envelope
                float envelope = Mathf.Sin(progress * Mathf.PI);

                float wave = Mathf.Sin(phase);
                samples[i] = Mathf.Clamp(wave * envelope * 0.75f, -1.0f, 1.0f);
            }

            return samples;
        }

        /// <summary>
        /// Tiếng click cơ khí sắc gọn khi bấm đèn pin (40ms).
        /// </summary>
        public static float[] GenerateFlashlightClick()
        {
            int sampleRate = 44100;
            float duration = 0.045f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            System.Random rand = new System.Random(42);
            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env1 = Mathf.Exp(-t * 120f);
                float clickTone = Mathf.Sin(2f * Mathf.PI * 1800f * t) * 0.6f;
                float noise = ((float)rand.NextDouble() * 2f - 1f) * 0.4f;

                samples[i] = Mathf.Clamp((clickTone + noise) * env1 * 0.7f, -1.0f, 1.0f);
            }

            return samples;
        }

        /// <summary>
        /// Tiếng trượt hộc bàn gỗ cơ khí (Drawer Slide 280ms).
        /// </summary>
        public static float[] GenerateDrawerSlide()
        {
            int sampleRate = 44100;
            float duration = 0.28f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            System.Random rand = new System.Random(108);
            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float decay = Mathf.Exp(-t * 10f);
                float lowThud = Mathf.Sin(2f * Mathf.PI * 120f * t) * 0.4f;
                float frictionNoise = ((float)rand.NextDouble() * 2f - 1f) * 0.3f;

                samples[i] = Mathf.Clamp((lowThud + frictionNoise) * decay * 0.65f, -1.0f, 1.0f);
            }

            return samples;
        }

        /// <summary>
        /// Tiếng nổ súng Shotgun 12-gauge uy lực (650ms):
        /// - Tiếng xung kích nổ ban đầu (Explosive Noise Punch + Attack 3ms).
        /// - Sóng trầm Sub-bass tụt tần số từ 160Hz về 35Hz tạo cảm giác rung chấn tức thì.
        /// - Độ vang dội cơ khí của kim loại và súng săn.
        /// </summary>
        public static float[] GenerateShotgunBlast()
        {
            int sampleRate = 44100;
            float duration = 0.65f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            System.Random rand = new System.Random(357);
            float bassPhase = 0f;

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;

                // 1. Tiếng xung kích nổ bột súng (Noise transient)
                float noiseEnv = Mathf.Exp(-t * 22f);
                float rawNoise = ((float)rand.NextDouble() * 2f - 1f);

                // 2. Tiếng sub-bass đấm mạnh tức thì (Kick drop 160Hz -> 38Hz)
                float bassFreq = Mathf.Lerp(160f, 38f, Mathf.Clamp01(t * 8f));
                bassPhase += 2f * Mathf.PI * bassFreq / sampleRate;
                float bassEnv = Mathf.Exp(-t * 11f);
                float bassWave = Mathf.Sin(bassPhase) * bassEnv * 0.95f;

                // 3. Tiếng dội tiếng nổ cơ khí kim loại (Metallic tail)
                float metalEnv = Mathf.Exp(-t * 7f);
                float metalTone = Mathf.Sin(2f * Mathf.PI * 450f * t) * 0.15f * metalEnv;

                float combined = (rawNoise * noiseEnv * 0.85f) + bassWave + metalTone;

                // Soft saturation limiter
                float saturated = Mathf.Clamp(combined * 1.25f, -1.0f, 1.0f);
                samples[i] = saturated;
            }

            return samples;
        }

        /// <summary>
        /// Tiếng nổ súng lục 9mm đanh thép, gọn gàng (450ms):
        /// Đòn nổ transient cao, tiếng nứt kim loại xé gió và tiêu giảm nhanh.
        /// </summary>
        public static float[] GeneratePistolFire()
        {
            int sampleRate = 44100;
            float duration = 0.45f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            System.Random rand = new System.Random(918);
            float bassPhase = 0f;

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;

                // 1. Tiếng đòn nổ thuốc súng (Sharp transient crack)
                float noiseEnv = Mathf.Exp(-t * 35f);
                float rawNoise = ((float)rand.NextDouble() * 2f - 1f);

                // 2. Tiếng nén áp lực (Punch 220Hz -> 50Hz)
                float bassFreq = Mathf.Lerp(220f, 50f, Mathf.Clamp01(t * 12f));
                bassPhase += 2f * Mathf.PI * bassFreq / sampleRate;
                float bassEnv = Mathf.Exp(-t * 16f);
                float bassWave = Mathf.Sin(bassPhase) * bassEnv * 0.9f;

                // 3. Tiếng đanh cơ khí kim loại súng ngắn
                float metalEnv = Mathf.Exp(-t * 20f);
                float metalTone = Mathf.Sin(2f * Mathf.PI * 1200f * t) * 0.25f * metalEnv;

                float combined = (rawNoise * noiseEnv * 0.95f) + bassWave + metalTone;
                samples[i] = Mathf.Clamp(combined * 1.3f, -1.0f, 1.0f);
            }

            return samples;
        }

        /// <summary>
        /// Tiếng nạp đạn cơ khí của súng lục (850ms):
        /// Gồm: tháo băng đạn cũ (click-slide) -> lắp băng đạn mới (clack) -> kéo khóa nòng (slide chamber).
        /// </summary>
        public static float[] GeneratePistolReload()
        {
            int sampleRate = 44100;
            float duration = 0.85f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];
            System.Random rand = new System.Random(404);

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float sound = 0f;

                // Pha 1: Tháo băng đạn (t = 0.05s .. 0.15s)
                if (t >= 0.05f && t < 0.18f)
                {
                    float p1 = (t - 0.05f) / 0.13f;
                    float env = Mathf.Sin(p1 * Mathf.PI);
                    float noise = ((float)rand.NextDouble() * 2f - 1f) * 0.3f;
                    sound += (Mathf.Sin(2f * Mathf.PI * 800f * t) * 0.4f + noise) * env;
                }

                // Pha 2: Đẩy băng đạn mới vào chuôi súng (t = 0.40s .. 0.55s - Tiếng cạch đanh)
                if (t >= 0.40f && t < 0.55f)
                {
                    float p2 = (t - 0.40f) / 0.15f;
                    float env2 = Mathf.Exp(-p2 * 15f);
                    float clickNoise = ((float)rand.NextDouble() * 2f - 1f) * 0.6f;
                    sound += (Mathf.Sin(2f * Mathf.PI * 1500f * t) * 0.5f + clickNoise) * env2;
                }

                // Pha 3: Kéo nhả khóa nòng trượt (t = 0.65s .. 0.80s - Tiếng trượt sắt bén)
                if (t >= 0.65f && t < 0.82f)
                {
                    float p3 = (t - 0.65f) / 0.17f;
                    float env3 = Mathf.Exp(-p3 * 12f);
                    float slideNoise = ((float)rand.NextDouble() * 2f - 1f) * 0.5f;
                    sound += (Mathf.Sin(2f * Mathf.PI * 2200f * t) * 0.6f + slideNoise) * env3;
                }

                samples[i] = Mathf.Clamp(sound * 1.1f, -1.0f, 1.0f);
            }

            return samples;
        }

        /// <summary>
        /// Tiếng búa kim hỏa đập vào buồng đạn rỗng khô khốc khi hết sạch đạn (Empty Click 65ms).
        /// </summary>
        public static float[] GenerateGunEmptyClick()
        {
            int sampleRate = 44100;
            float duration = 0.065f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];
            System.Random rand = new System.Random(77);

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 60f);
                float noise = ((float)rand.NextDouble() * 2f - 1f) * 0.4f;
                float clickTone = Mathf.Sin(2f * Mathf.PI * 2600f * t) * 0.7f;
                samples[i] = Mathf.Clamp((clickTone + noise) * env, -1.0f, 1.0f);
            }

            return samples;
        }

        /// <summary>
        /// Tạo tiếng nổ động cơ xe máy 4 thì êm giòn, lặp vòng mượt mà (Motorbike Engine Loop 1.0s).
        /// </summary>
        public static float[] GenerateMotorbikeEngineLoop()
        {
            int sampleRate = 44100;
            float duration = 1.0f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];
            System.Random rand = new System.Random(42);

            float baseFreq = 48.0f; // 48 Hz tiếng nổ trầm ấm
            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                // Nhịp xung piston 4 thì
                float pulse = Mathf.Sin(2f * Mathf.PI * baseFreq * t);
                float h2 = Mathf.Sin(2f * Mathf.PI * (baseFreq * 2f) * t) * 0.45f;
                float h3 = Mathf.Sin(2f * Mathf.PI * (baseFreq * 3f) * t) * 0.25f;
                float h4 = Mathf.Sin(2f * Mathf.PI * (baseFreq * 4f) * t) * 0.15f;
                float exhaustThump = Mathf.Pow(Mathf.Abs(Mathf.Sin(Mathf.PI * baseFreq * t)), 3f) * 0.4f;
                float airHiss = ((float)rand.NextDouble() * 2f - 1f) * 0.08f;

                samples[i] = Mathf.Clamp((pulse + h2 + h3 + h4 + exhaustThump + airHiss) * 0.4f, -1.0f, 1.0f);
            }

            return samples;
        }

        /// <summary>
        /// Tạo AudioClip động cho tiếng động cơ xe máy.
        /// </summary>
        public static AudioClip CreateMotorbikeEngineClip()
        {
            float[] samples = GenerateMotorbikeEngineLoop();
            AudioClip clip = AudioClip.Create("Motorbike_Engine_Loop", samples.Length, 1, 44100, false);
            clip.SetData(samples, 0);
            return clip;
        }

        #endregion

        #region Xuất file WAV chuẩn 16-bit PCM

        private static void SaveWav(string filePath, float[] samples)
        {
            byte[] wavBytes = ConvertSamplesToWavBytes(samples, 44100);
            File.WriteAllBytes(filePath, wavBytes);
        }

        private static byte[] ConvertSamplesToWavBytes(float[] samples, int sampleRate)
        {
            int channels = 1;
            int bitsPerSample = 16;
            int byteRate = sampleRate * channels * (bitsPerSample / 8);
            int blockAlign = channels * (bitsPerSample / 8);
            int subChunk2Size = samples.Length * (bitsPerSample / 8);
            int chunkSize = 36 + subChunk2Size;

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                // RIFF chunk descriptor
                bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                bw.Write(chunkSize);
                bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

                // "fmt " sub-chunk
                bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                bw.Write(16); // Subchunk1Size for PCM
                bw.Write((short)1); // AudioFormat 1 = PCM
                bw.Write((short)channels);
                bw.Write(sampleRate);
                bw.Write(byteRate);
                bw.Write((short)blockAlign);
                bw.Write((short)bitsPerSample);

                // "data" sub-chunk
                bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                bw.Write(subChunk2Size);

                // Write 16-bit PCM audio samples
                for (int i = 0; i < samples.Length; i++)
                {
                    short pcmSample = (short)Mathf.Clamp(samples[i] * 32767f, -32768f, 32767f);
                    bw.Write(pcmSample);
                }

                return ms.ToArray();
            }
        }

        #endregion
    }
}

