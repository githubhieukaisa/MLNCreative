using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioUIManager : TeamBehaviour
{
    public static ScenarioUIManager Instance { get; private set; }

    [Header("Question Panel")]
    [SerializeField] private GameObject _questionPanel;
    [SerializeField] private TextMeshProUGUI _questionText;

    [Header("Options Panel")]
    [SerializeField] private Button _btnOptionA;
    [SerializeField] private Button _btnOptionB;
    [SerializeField] private Button _btnOptionC;
    private TextMeshProUGUI _txtOptionA;
    private TextMeshProUGUI _txtOptionB;
    private TextMeshProUGUI _txtOptionC;

    [Header("Feedback Panel")]
    [SerializeField] private GameObject _feedbackPanel;
    [SerializeField] private TextMeshProUGUI _feedbackText;
    [SerializeField] private Button _btnContinueFeedback;

    private Action<Option> _onOptionSelectedCallback;
    private Action _onFeedbackContinueCallback;
    private QuestionStep _currentStep;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Map events
        _btnOptionA.onClick.AddListener(() => SelectOption(_currentStep.optionA));
        _btnOptionB.onClick.AddListener(() => SelectOption(_currentStep.optionB));
        _btnOptionC.onClick.AddListener(() => SelectOption(_currentStep.optionC));

        _btnContinueFeedback.onClick.AddListener(() =>
        {
            _feedbackPanel.SetActive(false);
            _onFeedbackContinueCallback?.Invoke();
        });

        HideAll();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        if (_btnOptionA) _txtOptionA = _btnOptionA.GetComponentInChildren<TextMeshProUGUI>();
        if (_btnOptionB) _txtOptionB = _btnOptionB.GetComponentInChildren<TextMeshProUGUI>();
        if (_btnOptionC) _txtOptionC = _btnOptionC.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ShowQuestion(QuestionStep step, Action<Option> onOptionSelected)
    {
        _currentStep = step;
        _onOptionSelectedCallback = onOptionSelected;

        _questionText.text = step.questionText;

        // Setup Option A
        SetupButton(_btnOptionA, _txtOptionA, step.optionA);
        // Setup Option B
        SetupButton(_btnOptionB, _txtOptionB, step.optionB);
        // Setup Option C (Có thể rỗng nên cần check)
        SetupButton(_btnOptionC, _txtOptionC, step.optionC);

        _questionPanel.SetActive(true);
    }

    private void SetupButton(Button btn, TextMeshProUGUI txt, Option optionData)
    {
        if (optionData == null || string.IsNullOrEmpty(optionData.text))
        {
            btn.gameObject.SetActive(false);
        }
        else
        {
            btn.gameObject.SetActive(true);
            txt.text = optionData.text;
        }
    }

    private void SelectOption(Option selectedOption)
    {
        _questionPanel.SetActive(false); // Ẩn câu hỏi đi
        _onOptionSelectedCallback?.Invoke(selectedOption); // Gửi data về cho GameManager
    }

    public void ShowFeedback(string feedbackMsg, Action onContinue)
    {
        _onFeedbackContinueCallback = onContinue;
        _feedbackText.text = feedbackMsg;
        _feedbackPanel.SetActive(true);
    }

    public void HideAll()
    {
        _questionPanel.SetActive(false);
        _feedbackPanel.SetActive(false);
    }
}
