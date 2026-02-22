using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : TeamBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Story Data")]
    [SerializeField] private LevelData _startingLevel;
    [SerializeField] private LevelData _gameOverLevelData;

    public Action<int, int, int> OnStatsChanged;
    public Action<bool> OnGameEnded;

    private int _currentStepIndex = 0;
    private LevelData _currentLevel;
    private LevelData _pendingBranch;

    private struct SequenceState
    {
        public LevelData level;
        public int index;
    }
    private Stack<SequenceState> _sequenceStack = new Stack<SequenceState>();

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (_startingLevel == null || _startingLevel.steps.Count == 0) return;

        _sequenceStack.Clear();
        LoadSequence(_startingLevel);
    }

    private void LoadSequence(LevelData newSequence)
    {
        if (newSequence == null || newSequence.steps.Count == 0)
        {
            Debug.LogWarning("Sequence trống hoặc bị Null!");
            return;
        }

        _currentLevel = newSequence;
        _currentStepIndex = 0;
        PlayCurrentStep();
    }

    private void PlayCurrentStep()
    {
        if (_currentStepIndex >= _currentLevel.steps.Count)
        {
            // Nếu đang ở màn Game Over thì dừng hẳn, không quay về nữa
            if (_currentLevel == _gameOverLevelData)
            {
                Debug.Log("TRÒ CHƠI KẾT THÚC (PHÁ SẢN)!");
                OnGameEnded?.Invoke(false);
                return;
            }

            // Nếu trong Stack còn trí nhớ -> Trở về nhánh chính
            if (_sequenceStack.Count > 0)
            {
                SequenceState prevState = _sequenceStack.Pop();
                _currentLevel = prevState.level;
                _currentStepIndex = prevState.index;

                PlayCurrentStep(); // Chạy tiếp bước kế tiếp của nhánh chính
                return;
            }

            Debug.Log("HẾT CỐT TRUYỆN!");
            OnGameEnded?.Invoke(true);
            return;
        }

        ScenarioStep step = _currentLevel.steps[_currentStepIndex];

        // Dùng Pattern Matching của C# mới cực kỳ sạch
        switch (step)
        {
            case DialogueStep dialogueStep:
                ScenarioUIManager.Instance.HideAll();
                DialogueManager.Instance.StartDialogue(dialogueStep.dialogueData, NextStep);
                if (SceneVisualManager.Instance != null && dialogueStep.startingVisual != null)
                {
                    SceneVisualManager.Instance.ApplyVisualState(dialogueStep.startingVisual);
                }
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

        //2.THÊM LOGIC ÂM THANH NGAY Ở ĐÂY
        if (selectedOption.capitalChange > 0)
        {
            Core.Audio.AudioManager.Instance.PlaySFX(Core.Audio.SFXType.Cash_In);
        }
        else if (selectedOption.capitalChange < 0)
        {
            Core.Audio.AudioManager.Instance.PlaySFX(Core.Audio.SFXType.Cash_Out);
        }
        else if (selectedOption.brandChange < 0)
        {
            Core.Audio.AudioManager.Instance.PlaySFX(Core.Audio.SFXType.Wrong_Action);
        }
        else if (selectedOption.brandChange > 0 || selectedOption.techChange > 0)
        {
            Core.Audio.AudioManager.Instance.PlaySFX(Core.Audio.SFXType.Correct_Action);
        }

        _pendingBranch = selectedOption.nextBranch;

        OnStatsChanged?.Invoke(
            PlayerDataManager.Instance.GetStat(StatType.Capital),
            PlayerDataManager.Instance.GetStat(StatType.Brand),
            PlayerDataManager.Instance.GetStat(StatType.Tech)
        );

        // Note: Nếu Vốn <= 0, xử lý Game Over ở đây.
        if (PlayerDataManager.Instance.GetStat(StatType.Capital) <= 0)
        {
            Debug.Log("GAME OVER: PHÁ SẢN!");
            _pendingBranch = _gameOverLevelData;
        }

        // 2. Hiện Feedback, bấm Continue thì đi tiếp
        ScenarioUIManager.Instance.ShowFeedback(selectedOption.feedback, NextStep);
    }

    private void NextStep()
    {
        if (_pendingBranch != null)
        {
            Debug.Log($"Rẽ nhánh sang kịch bản: {_pendingBranch.levelName}");

            _sequenceStack.Push(new SequenceState
            {
                level = _currentLevel,
                index = _currentStepIndex + 1
            });

            LevelData branchToLoad = _pendingBranch;
            _pendingBranch = null;

            LoadSequence(branchToLoad);
        }
        else
        {
            _currentStepIndex++;
            PlayCurrentStep();
        }
    }
}
