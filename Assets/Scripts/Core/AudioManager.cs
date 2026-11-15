using System;
using System.Collections.Generic;
using UnityEngine;

namespace MidnightMart.Core
{
    public enum AudioChannel
    {
        Master,
        SFXAndAmbience,
        Music
    }

    /// <summary>
    /// Quản lý Âm thanh Toàn cục (Audio Manager).
    /// Cung cấp 2 kênh điều chỉnh âm lượng riêng biệt:
    /// 1. Âm thanh Môi trường & Hiệu ứng (SFX & Ambience Volume: tiếng gió mưa, bước chân, súng đạn, chổi lau, chuông cửa).
    /// 2. Nhạc nền (Music & Tension Volume: drone căng thẳng, radio, nhạc chiến đấu với trùm).
    /// Tự động lưu thiết lập vào PlayerPrefs và cập nhật tức thời khi người chơi kéo Slider trong Menu Cài đặt.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private const string PREF_MASTER = "MidnightMart_MasterVolume";
        private const string PREF_SFX = "MidnightMart_SFXVolume";
        private const string PREF_MUSIC = "MidnightMart_MusicVolume";

        [Range(0f, 1f)] [SerializeField] private float _masterVolume = 1.0f;
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 0.85f;
        [Range(0f, 1f)] [SerializeField] private float _musicVolume = 0.75f;

        public float MasterVolume => _masterVolume;
        public float SFXVolume => _sfxVolume;
        public float MusicVolume => _musicVolume;

        private readonly List<AudioSource> _sfxSources = new List<AudioSource>();
        private readonly List<AudioSource> _musicSources = new List<AudioSource>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadVolumes();
        }

        private void LoadVolumes()
        {
            _masterVolume = PlayerPrefs.GetFloat(PREF_MASTER, 1.0f);
            _sfxVolume = PlayerPrefs.GetFloat(PREF_SFX, 0.85f);
            _musicVolume = PlayerPrefs.GetFloat(PREF_MUSIC, 0.75f);
            ApplyVolumes();
        }

        public void SetMasterVolume(float vol)
        {
            _masterVolume = Mathf.Clamp01(vol);
            PlayerPrefs.SetFloat(PREF_MASTER, _masterVolume);
            PlayerPrefs.Save();
            ApplyVolumes();
        }

        public void SetSFXVolume(float vol)
        {
            _sfxVolume = Mathf.Clamp01(vol);
            PlayerPrefs.SetFloat(PREF_SFX, _sfxVolume);
            PlayerPrefs.Save();
            ApplyVolumes();
        }

        public void SetMusicVolume(float vol)
        {
            _musicVolume = Mathf.Clamp01(vol);
            PlayerPrefs.SetFloat(PREF_MUSIC, _musicVolume);
            PlayerPrefs.Save();
            ApplyVolumes();
        }

        public void RegisterSource(AudioSource source, AudioChannel channel)
        {
            if (source == null) return;
            if (channel == AudioChannel.Music)
            {
                if (!_musicSources.Contains(source)) _musicSources.Add(source);
                source.volume = _masterVolume * _musicVolume;
            }
            else
            {
                if (!_sfxSources.Contains(source)) _sfxSources.Add(source);
                source.volume = _masterVolume * _sfxVolume;
            }
        }

        public void UnregisterSource(AudioSource source)
        {
            if (source == null) return;
            _sfxSources.Remove(source);
            _musicSources.Remove(source);
        }

        public void ApplyVolumes()
        {
            AudioListener.volume = _masterVolume;

            float effectiveSFX = _sfxVolume;
            for (int i = _sfxSources.Count - 1; i >= 0; i--)
            {
                if (_sfxSources[i] == null) _sfxSources.RemoveAt(i);
                else _sfxSources[i].volume = effectiveSFX;
            }

            float effectiveMusic = _musicVolume;
            for (int i = _musicSources.Count - 1; i >= 0; i--)
            {
                if (_musicSources[i] == null) _musicSources.RemoveAt(i);
                else _musicSources[i].volume = effectiveMusic;
            }
        }
    }
}

