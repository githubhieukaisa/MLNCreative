using UnityEngine;

namespace Core.Audio
{
    public class MusicDebugTool : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float testPollution = 0f;

        private bool _autoApplyPollution = false;

        private void Update()
        {
            // Nếu bật chế độ test pollution thì update liên tục
            if (_autoApplyPollution && AudioManager.Instance != null)
            {
                // AudioManager.Instance.UpdateMusicEffect(testPollution);
            }
        }

        private void OnGUI()
        {
            // Vẽ một hộp ở góc trái màn hình
            GUILayout.BeginArea(new Rect(10, 10, 250, 400), "MUSIC DEBUGGER", GUI.skin.window);

            GUILayout.Space(10);

            if (AudioManager.Instance == null)
            {
                GUILayout.Label("Waiting for AudioManager...");
                GUILayout.EndArea();
                return;
            }

            // --- CÁC NÚT BẤM KÍCH HOẠT NHẠC ---

            if (GUILayout.Button("1. Play Main Menu"))
            {
                AudioManager.Instance.PlayMusic(MusicType.MainMenu);
            }

            if (GUILayout.Button("2. Play Normal Gameplay"))
            {
                AudioManager.Instance.PlayMusic(MusicType.Gameplay);
                _autoApplyPollution = true; // Tự động bật test effect
            }

            if (GUILayout.Button("3. Play Polluted Gameplay"))
            {
                AudioManager.Instance.PlayMusic(MusicType.Crisis);
            }

            if (GUILayout.Button("4. Play Victory"))
            {
                AudioManager.Instance.PlayMusic(MusicType.Victory);
            }

            if (GUILayout.Button("5. Play Game Over"))
            {
                AudioManager.Instance.PlayMusic(MusicType.Defeat);
            }

            if (GUILayout.Button("Stop Music (None)"))
            {
                AudioManager.Instance.PlayMusic(MusicType.None);
            }

            GUILayout.Space(20);

            // --- TEST HIỆU ỨNG Ô NHIỄM (MÉO TIẾNG) ---
            GUILayout.Label($"Pollution Effect: {(int)(testPollution * 100)}%");

            // Thanh trượt ngang
            testPollution = GUILayout.HorizontalSlider(testPollution, 0f, 1f);

            _autoApplyPollution = GUILayout.Toggle(_autoApplyPollution, "Auto Apply Effect");

            GUILayout.EndArea();
        }
    }
}