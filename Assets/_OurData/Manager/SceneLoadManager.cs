using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Tên của Scene Main Menu chính xác như trong Build Settings")]
    [SerializeField] private string _mainMenuSceneName = "MainMenu"; 

    /// <summary>
    /// Hàm này public để UI Button có thể gọi được thông qua OnClick() sự kiện.
    /// </summary>
    public void ReturnToMainMenu()
    {
        // Safety check: Đảm bảo tên scene không bị bỏ trống
        if (string.IsNullOrEmpty(_mainMenuSceneName))
        {
            Debug.LogError("[SceneLoadManager] Tên scene Main Menu đang bị trống! Hãy kiểm tra lại Inspector.");
            return;
        }

        // Load lại scene Menu
        // Lưu ý: Việc load scene trực tiếp bằng LoadScene sẽ làm đứng game 1 tích tắc. 
        // Với game nhẹ như Visual Novel thì hoàn toàn ổn định.
        SceneManager.LoadScene(_mainMenuSceneName);
    }
}