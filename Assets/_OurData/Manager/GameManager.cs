using UnityEngine;

public class GameManager : TeamBehaviour
{
    [Header("Story Data")]
    [SerializeField] private LevelData _chapter1Data; // Kéo file LevelData vào đây

    private int _currentStepIndex = 0;

    private void Start()
    {
        // Đợi 1 frame để các Manager khác khởi tạo xong rồi mới chạy
        Invoke(nameof(StartGame), 0.1f);
    }

    private void StartGame()
    {
        if (_chapter1Data == null || _chapter1Data.steps.Count == 0) return;

        _currentStepIndex = 0;
        PlayCurrentStep();
    }

    private void PlayCurrentStep()
    {
        if (_currentStepIndex >= _chapter1Data.steps.Count)
        {
            Debug.Log("HẾT CHƯƠNG 1!");
            return; // Ở đây sau này có thể load level tiếp theo
        }

        ScenarioStep step = _chapter1Data.steps[_currentStepIndex];

        // Dùng Pattern Matching của C# mới cực kỳ sạch
        switch (step)
        {
            case DialogueStep dialogueStep:
                // Tắt UI câu hỏi (nếu đang bật)
                ScenarioUIManager.Instance.HideAll();
                // Chạy thoại, xong thoại thì tự gọi NextStep
                DialogueManager.Instance.StartDialogue(dialogueStep.dialogueData, NextStep);
                break;

            case QuestionStep questionStep:
                // Hiện UI Câu hỏi, chọn xong thì gọi OnOptionSelected
                ScenarioUIManager.Instance.ShowQuestion(questionStep, OnOptionSelected);
                break;
        }
    }

    private void OnOptionSelected(Option selectedOption)
    {
        // 1. Cập nhật chỉ số dùng PlayerDataManager của bạn!
        if (selectedOption.capitalChange != 0)
            PlayerDataManager.Instance.ModifyStat(StatType.Capital, selectedOption.capitalChange);

        if (selectedOption.brandChange != 0)
            PlayerDataManager.Instance.ModifyStat(StatType.Brand, selectedOption.brandChange);

        if (selectedOption.techChange != 0)
            PlayerDataManager.Instance.ModifyStat(StatType.Tech, selectedOption.techChange);

        // 2. Hiện Feedback, bấm Continue thì đi tiếp
        ScenarioUIManager.Instance.ShowFeedback(selectedOption.feedback, NextStep);

        // Note: Nếu Vốn <= 0, xử lý Game Over ở đây.
        if (PlayerDataManager.Instance.GetStat(StatType.Capital) <= 0)
        {
            Debug.Log("GAME OVER: PHÁ SẢN!");
            // Gọi màn hình thua cuộc...
        }
    }

    private void NextStep()
    {
        _currentStepIndex++;
        PlayCurrentStep();
    }
}
