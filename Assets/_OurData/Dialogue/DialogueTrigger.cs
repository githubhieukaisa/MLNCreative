using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public UnityEvent onDialogueEndEvent;
    private bool hasTriggered = false;

    public void TriggerDialogue()
    {
        if(dialogue == null) return;
        DialogueManager.Instance.StartDialogue(dialogue, TriggerEvent);
        hasTriggered = true;
    }

    private void TriggerEvent() 
    {
        onDialogueEndEvent?.Invoke();
        hasTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.TriggerDialogue();
        }
    }
}
