using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private DialogueData _conversationData;

    [Header("Events")]
    public UnityEvent OnDialogueEnd;

    private bool _isPlayerInRange;

    public void TriggerDialogue()
    {
        // Chặn nếu chưa gán Data hoặc Đang có hội thoại khác diễn ra
        if (_conversationData == null || DialogueManager.Instance.IsActive) return;

        DialogueManager.Instance.StartDialogue(_conversationData, OnDialogueFinish);
    }

    private void OnDialogueFinish()
    {
        OnDialogueEnd?.Invoke();
    }
}