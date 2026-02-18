using UnityEngine;
using System.Collections.Generic;

namespace Core.Audio
{
    [CreateAssetMenu(fileName = "MusicDatabase", menuName = "EcoFactory/Audio/MusicDatabase")]
    public class MusicDatabaseSO : ScriptableObject
    {
        [System.Serializable]
        public struct MusicTrack
        {
            public MusicType type;
            public AudioClip clip;
            [Range(0f, 1f)] public float volumeScale; // Dùng để cân bằng âm lượng từng bài
        }

        [SerializeField] private List<MusicTrack> _tracks;
        private Dictionary<MusicType, MusicTrack> _trackDict;

        public void Initialize()
        {
            _trackDict = new Dictionary<MusicType, MusicTrack>();
            foreach (var track in _tracks)
            {
                if (!_trackDict.ContainsKey(track.type))
                {
                    _trackDict.Add(track.type, track);
                }
            }
        }

        public AudioClip GetClip(MusicType type, out float volumeScale)
        {
            if (_trackDict == null) Initialize();

            if (_trackDict.TryGetValue(type, out MusicTrack track))
            {
                volumeScale = track.volumeScale;
                return track.clip;
            }

            volumeScale = 1f;
            return null;
        }
    }
}