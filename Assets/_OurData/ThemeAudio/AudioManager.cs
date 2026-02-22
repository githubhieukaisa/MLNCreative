using UnityEngine;
using System.Collections;

namespace Core.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : TeamBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Databases")]
        [SerializeField] private MusicDatabaseSO _musicDatabase;
        [SerializeField] private SFXDatabaseSO _sfxDatabase;

        [Header("Settings")]
        [SerializeField] private float _crossFadeDuration = 1f;

        [Header("Audio Sources (Auto-Generated)")]
        [SerializeField] private AudioSource _musicSourcePrimary;   // Music A
        [SerializeField] private AudioSource _musicSourceSecondary; // Music B (Crossfade)
        [SerializeField] private AudioSource _sfxSource;            // SFX riêng

        [Header("Crisis Effect (Low Pass Filter)")]
        [SerializeField] private AudioLowPassFilter _lowPassFilter;
        [SerializeField] private float _normalCutoff = 22000f;   // Âm thanh trong trẻo
        [SerializeField] private float _crisisCutoff = 800f;     // Âm thanh bị nghẹt (muffled)

        private bool _isPrimaryMusicActive = true;
        private Coroutine _fadeCoroutine;
        private MusicType _currentMusicType = MusicType.None;

        protected override void Awake()
        {
            base.Awake();
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // Init database ngay lập tức
                _musicDatabase?.Initialize();
                _sfxDatabase?.Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Tự động tìm hoặc tạo AudioSource khi Reset hoặc chạy
        protected override void LoadComponents()
        {
            base.LoadComponents();

            var sources = GetComponents<AudioSource>();

            _musicSourcePrimary = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
            _musicSourceSecondary = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();
            _sfxSource = sources.Length > 2 ? sources[2] : gameObject.AddComponent<AudioSource>();

            if (_lowPassFilter == null)
            {
                _lowPassFilter = GetComponent<AudioLowPassFilter>() ?? gameObject.AddComponent<AudioLowPassFilter>();
            }

            SetupMusicSource(_musicSourcePrimary);
            SetupMusicSource(_musicSourceSecondary);
            SetupSFXSource(_sfxSource);
        }

        private void SetupMusicSource(AudioSource source)
        {
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 0f;
            source.spatialBlend = 0f;
        }

        private void SetupSFXSource(AudioSource source)
        {
            source.loop = false;
            source.playOnAwake = false;
            source.volume = 1f;
            source.spatialBlend = 0f;
        }

        protected override void ResetValue()
        {
            base.ResetValue();
            if (_lowPassFilter != null) _lowPassFilter.cutoffFrequency = _normalCutoff;
        }

        private void Start()
        {
            StartCoroutine(StartMusicDelayed());
        }

        private IEnumerator StartMusicDelayed()
        {
            yield return null;
            PlayMusic(MusicType.MainMenu);
        }

        /// <summary>
        /// Phát nhạc nền với hiệu ứng Crossfade mượt mà
        /// </summary>
        public void PlayMusic(MusicType type)
        {
            if (_currentMusicType == type) return;

            // volScale ở đây là float, nó sẽ luôn có giá trị (mặc định là 1f từ DatabaseSO)
            float volScale = 1f;
            var clip = _musicDatabase?.GetClip(type, out volScale);

            if (clip == null && type != MusicType.None) return;

            _currentMusicType = type;

            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

            // Bỏ đoạn "?? 1f" đi vì volScale chắc chắn có giá trị float
            _fadeCoroutine = StartCoroutine(CrossFadeRoutine(clip, volScale));
        }

        /// <summary>
        /// Phát tiếng động (có thể chồng đè nhiều tiếng cùng lúc)
        /// </summary>
        public void PlaySFX(SFXType type)
        {
            if (_sfxDatabase == null) return;

            var clip = _sfxDatabase.GetClip(type, out float volScale);
            if (clip != null)
            {
                _sfxSource.PlayOneShot(clip, volScale);
            }
        }

        /// <summary>
        /// Hiệu ứng "Khủng hoảng".
        /// intensity: 0 = Bình thường, 1 = Căng thẳng cực độ (nghẹt tiếng).
        /// </summary>
        public void SetStressLevel(float intensity)
        {
            if (_lowPassFilter == null) return;

            intensity = Mathf.Clamp01(intensity);

            // Lerp từ Normal (22000Hz) xuống Crisis (800Hz)
            float targetCutoff = Mathf.Lerp(_normalCutoff, _crisisCutoff, intensity);

            _lowPassFilter.cutoffFrequency = targetCutoff;
        }

        // ================= INTERNAL LOGIC =================

        private IEnumerator CrossFadeRoutine(AudioClip nextClip, float targetVol)
        {
            var activeSource = _isPrimaryMusicActive ? _musicSourcePrimary : _musicSourceSecondary;
            var nextSource = _isPrimaryMusicActive ? _musicSourceSecondary : _musicSourcePrimary;

            if (!activeSource.isPlaying) activeSource.volume = 0f;

            // Prepare next source
            nextSource.clip = nextClip;
            nextSource.volume = 0;
            if (nextClip != null) nextSource.Play();

            float timer = 0f;
            float startVol = activeSource.volume;

            while (timer < _crossFadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                float t = timer / _crossFadeDuration;

                // Fade In Next
                if (nextClip != null) nextSource.volume = Mathf.Lerp(0, targetVol, t);

                // Fade Out Active
                if (activeSource.isPlaying) activeSource.volume = Mathf.Lerp(startVol, 0, t);

                yield return null;
            }

            activeSource.Stop();
            activeSource.volume = 0;

            // Ensure final volume is precise
            if (nextClip != null) nextSource.volume = targetVol;

            _isPrimaryMusicActive = !_isPrimaryMusicActive;
        }
    }
}