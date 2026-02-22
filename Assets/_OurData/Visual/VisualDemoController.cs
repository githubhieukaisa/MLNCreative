using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualDemoController : TeamBehaviour
{
    [Header("References")]
    [SerializeField] private SceneVisualManager visualManager;
    [SerializeField] private Button btnNextDemo;

    [Header("Demo Sequence")]
    [SerializeField] private List<VisualState> demoSequence;

    private int currentIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        // Cảnh báo Performance/Lỗi: Tránh GetComponent trong Update, cache nó ở đây.
        if (btnNextDemo != null)
        {
            btnNextDemo.onClick.AddListener(OnNextButtonClicked);
        }
    }

    private void Start()
    {
        // Khởi chạy state đầu tiên khi vào game
        if (demoSequence.Count > 0)
        {
            visualManager.ApplyVisualState(demoSequence[0]);
        }
    }

    private void OnNextButtonClicked()
    {
        currentIndex++;

        if (currentIndex < demoSequence.Count)
        {
            // Gọi Manager để đổi hình
            visualManager.ApplyVisualState(demoSequence[currentIndex]);
        }
        else
        {
            Debug.Log("Đã hết Sequence Demo!");
            // Nút bấm không làm gì nữa, hoặc reset về 0 tùy bạn
            currentIndex = demoSequence.Count - 1;
        }
    }

    private void OnDestroy()
    {
        if (btnNextDemo != null) btnNextDemo.onClick.RemoveListener(OnNextButtonClicked);
    }
}