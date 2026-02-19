using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc để chuyển cảnh
using UnityEngine.UI; // Bắt buộc để dùng Button

public class MainMenuController : MonoBehaviour
{
    [Header("SCENE CONFIG")]
    // Tên Scene Gameplay phải trùng khớp với tên file trong Project
    [SerializeField] private string gameplaySceneName = "GameplayScene";

    [Header("MAIN MENU REFERENCES")]
    [SerializeField] private GameObject panelMainMenu; // Cái Panel chứa 3 nút chính
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnOptions;
    [SerializeField] private Button btnExit;

    [Header("OPTIONS REFERENCES")]
    [SerializeField] private GameObject panelOptions; // Cái Panel Options vừa tạo
    [SerializeField] private Button btnBackFromOptions; // Nút Back/Close trong Options

    private void Awake()
    {
        // 1. Đảm bảo trạng thái ban đầu: Hiện Menu, Ẩn Options
        panelMainMenu.SetActive(true);
        panelOptions.SetActive(false);

        // 2. Gán sự kiện cho các nút (Code sạch hơn kéo thả trong Inspector)
        btnStart.onClick.AddListener(OnStartClicked);
        btnOptions.onClick.AddListener(OnOptionsClicked);
        btnExit.onClick.AddListener(OnExitClicked);

        if (btnBackFromOptions != null)
            btnBackFromOptions.onClick.AddListener(OnBackFromOptionsClicked);
    }

    // --- LOGIC XỬ LÝ ---

    private void OnStartClicked()
    {
        // Kiểm tra xem Scene có tồn tại trong Build Settings chưa để tránh lỗi
        if (Application.CanStreamedLevelBeLoaded(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            Debug.LogError($"Lỗi: Không tìm thấy Scene tên '{gameplaySceneName}'. Hãy vào Build Settings để thêm Scene này vào!");
        }
    }

    private void OnOptionsClicked()
    {
        // Ẩn menu chính, hiện bảng Options
        panelMainMenu.SetActive(false);
        panelOptions.SetActive(true);
    }

    private void OnBackFromOptionsClicked()
    {
        // Ẩn bảng Options, hiện lại menu chính
        panelOptions.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    private void OnExitClicked()
    {
        Debug.Log("Đã bấm thoát game!");

#if UNITY_EDITOR
        // Nếu đang test trong Unity Editor thì dừng Play
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Nếu là bản build thật thì tắt ứng dụng
            Application.Quit();
#endif
    }

    // Dọn dẹp sự kiện khi object bị hủy (Good practice)
    private void OnDestroy()
    {
        btnStart.onClick.RemoveAllListeners();
        btnOptions.onClick.RemoveAllListeners();
        btnExit.onClick.RemoveAllListeners();
        if (btnBackFromOptions != null) btnBackFromOptions.onClick.RemoveAllListeners();
    }
}