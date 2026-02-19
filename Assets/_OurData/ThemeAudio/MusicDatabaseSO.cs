using UnityEngine;
using System.Collections.Generic;

namespace Core.Audio
{
    public abstract class AudioLibrarySO<T> : ScriptableObject where T : System.Enum
    {
        [System.Serializable]
        public struct AudioTrack
        {
            public T type;
            public AudioClip clip;
            [Range(0f, 1f)] public float volumeScale; // Cân bằng âm lượng từng file
        }

        [SerializeField] protected List<AudioTrack> _tracks;

        // Dictionary để tra cứu nhanh O(1) thay vì duyệt List O(n)
        protected Dictionary<T, AudioTrack> _trackDict;

        public virtual void Initialize()
        {
            _trackDict = new Dictionary<T, AudioTrack>();
            foreach (var track in _tracks)
            {
                if (!_trackDict.ContainsKey(track.type))
                {
                    _trackDict.Add(track.type, track);
                }
            }
        }

        public AudioClip GetClip(T type, out float volumeScale)
        {
            if (_trackDict == null) Initialize();

            if (_trackDict.TryGetValue(type, out AudioTrack track))
            {
                volumeScale = track.volumeScale;
                return track.clip;
            }

            volumeScale = 1f;
            return null;
        }
    }

    // Định nghĩa MusicDatabase (Tạo trong Editor)
    [CreateAssetMenu(fileName = "MusicDatabase", menuName = "KinhTeChinhTri/Audio/MusicDatabase")]
    public class MusicDatabaseSO : AudioLibrarySO<MusicType> { }

    // Định nghĩa SFXDatabase (Tạo trong Editor)
    [CreateAssetMenu(fileName = "SFXDatabase", menuName = "KinhTeChinhTri/Audio/SFXDatabase")]
    public class SFXDatabaseSO : AudioLibrarySO<SFXType> { }
}