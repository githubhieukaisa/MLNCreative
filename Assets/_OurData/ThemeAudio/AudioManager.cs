using UnityEngine;
using System.Collections;

namespace Core.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : TeamBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private MusicDatabaseSO _musicDatabase;
        [SerializeField] private float _crossFadeDuration = 2.0f;

        [Header("Debug")]
        [SerializeField] private AudioSource _primarySource;
        [SerializeField] private AudioSource _secondarySource;

        [Header("Effect Configuration")]
        [SerializeField] private AudioLowPassFilter _musicLowPassFilter;
        [SerializeField] private float _normalPitch = 1.0f;
        [SerializeField] private float _dampenedPitch = 0.8f;
        [SerializeField] private float _normalCutoff = 22000f;
        [SerializeField] private float _dampenedCutoff = 800f;

        private bool _isPrimaryPlaying = true;
        private Coroutine _fadeCoroutine;
        //Ở đây tôi giữ Enum MusicType nhưng bạn phải định nghĩa nó ở file riêng.
        private MusicType _currentMusicType = MusicType.None;

        protected override void Awake()
        {
            base.Awake();
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _musicDatabase.Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected override void LoadComponents()
        {
            base.LoadComponents();

            AudioSource[] sources = GetComponents<AudioSource>();

            if (sources.Length >= 1) _primarySource = sources[0];
            else _primarySource = gameObject.AddComponent<AudioSource>();

            if (sources.Length >= 2) _secondarySource = sources[1];
            else _secondarySource = gameObject.AddComponent<AudioSource>();

            if (_musicLowPassFilter == null)
            {
                _musicLowPassFilter = transform.GetComponent<AudioLowPassFilter>();
                _musicLowPassFilter.cutoffFrequency = _normalCutoff;
            }

            SetupSource(_primarySource);
            SetupSource(_secondarySource);
        }

        // Override từ TeamBehaviour: Reset giá trị mặc định
        protected override void ResetValue()
        {
            base.ResetValue();
            if (_primarySource != null) _primarySource.volume = 0;
            if (_secondarySource != null) _secondarySource.volume = 0;
        }

        private void SetupSource(AudioSource source)
        {
            source.loop = true;
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = 0f;
        }

        public void PlayMusic(MusicType type)
        {
            if (_currentMusicType == type) return;
            if (_musicDatabase == null)
            {
                Debug.LogWarning("AudioManager: Missing MusicDatabaseSO!");
                return;
            }

            AudioClip nextClip = _musicDatabase.GetClip(type, out float volumeScale);
            if (nextClip == null) return;

            _currentMusicType = type;

            AudioSource currentSource = _isPrimaryPlaying ? _primarySource : _secondarySource;
            if (currentSource.clip == nextClip && currentSource.isPlaying) return;

            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(CrossFadeMusic(nextClip, volumeScale));
        }

        public void SetMusicDampening(float intensity)
        {
            if (_primarySource == null) return;

            intensity = Mathf.Clamp01(intensity);

            float targetPitch = Mathf.Lerp(_normalPitch, _dampenedPitch, intensity);
            float targetCutoff = Mathf.Lerp(_normalCutoff, _dampenedCutoff, intensity);

            // Apply cho cả 2 nguồn để khi crossfade không bị lệch
            _primarySource.pitch = targetPitch;
            _secondarySource.pitch = targetPitch;

            if (_musicLowPassFilter != null)
            {
                _musicLowPassFilter.cutoffFrequency = targetCutoff;
            }
        }

        private IEnumerator CrossFadeMusic(AudioClip newClip, float targetVolume)
        {
            AudioSource activeSource = _isPrimaryPlaying ? _primarySource : _secondarySource;
            AudioSource newSource = _isPrimaryPlaying ? _secondarySource : _primarySource;

            // Setup new source
            newSource.clip = newClip;
            newSource.volume = 0;
            newSource.Play();

            float timer = 0f;
            float startVolume = activeSource.volume;

            while (timer < _crossFadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                float t = timer / _crossFadeDuration;

                // Fade In New Source
                newSource.volume = Mathf.Lerp(0, targetVolume, t);

                // Fade Out Old Source
                if (activeSource.isPlaying)
                {
                    activeSource.volume = Mathf.Lerp(startVolume, 0, t);
                }

                yield return null;
            }

            activeSource.Stop();
            activeSource.volume = 0;
            newSource.volume = targetVolume;

            // Swap active flag
            _isPrimaryPlaying = !_isPrimaryPlaying;
        }

        public void UpdateMusicEffect(float pollutionPercent)
        {
            // Chỉ áp dụng khi đang chơi Normal Gameplay
            if (_primarySource == null) return;

            // Lerp các giá trị dựa theo % ô nhiễm (0 -> 1)
            // Càng ô nhiễm, nhạc càng trầm và đục
            float targetPitch = Mathf.Lerp(_normalPitch, _dampenedPitch, pollutionPercent);
            float targetCutoff = Mathf.Lerp(_normalCutoff, _dampenedCutoff, pollutionPercent);

            _primarySource.pitch = targetPitch;

            if (_musicLowPassFilter != null)
            {
                _musicLowPassFilter.cutoffFrequency = targetCutoff;
            }
        }
    }
}