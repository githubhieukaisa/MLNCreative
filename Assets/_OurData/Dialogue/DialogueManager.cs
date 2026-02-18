using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : TeamBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue Manager")]
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines = new Queue<DialogueLine>();

    public bool isDialogueActive = false;
    public float typingSpeed = 0.02f;
    public bool isTyping = false;
    public string currentSentence = "";

    public Animator animator;
    [SerializeField] private Action onDialogueEndedCallback;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
        this.animator.Play("Hide");
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadAnimator();
        this.LoadCharacterIcon();
        this.LoadCharacterName();
        this.LoadDialogueArea();
    }

    protected virtual void LoadAnimator()
    {
        if (this.animator != null) return;
        this.animator = GetComponent<Animator>();
        Debug.Log(this.transform.name + ": LoadAnimator");
    }

    protected virtual void LoadCharacterIcon()
    {
        if (this.characterIcon != null) return;
        this.characterIcon = transform.Find("AvatarIcon").GetComponent<Image>();
        Debug.Log(this.transform.name + ": LoadCharacterIcon");
    }

    protected virtual void LoadCharacterName()
    {
        if (this.characterName != null) return;
        this.characterName = transform.Find("Header").GetChild(0).GetComponent<TextMeshProUGUI>();
        Debug.Log(this.transform.name + ": LoadCharacterName");
    }

    protected virtual void LoadDialogueArea()
    {
        if (this.dialogueArea != null) return;
        this.dialogueArea = transform.Find("Body").GetChild(0).GetComponent<TextMeshProUGUI>();
        Debug.Log(this.transform.name + ": LoadDialogueArea");
    }

    public void StartDialogue(Dialogue dialogue, Action onEnded = null)
    {
        this.isDialogueActive = true;
        this.animator.Play("Show");
        this.lines.Clear();

        this.onDialogueEndedCallback = onEnded;

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            this.lines.Enqueue(dialogueLine);
        }

        this.DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            this.dialogueArea.text = currentSentence; // Hiển thị full text
            isTyping = false; // Đánh dấu là đã gõ xong
            return; // Thoát hàm, không load câu tiếp theo
        }

        if (this.lines.Count == 0)
        {
            this.EndDialogue();
            return;
        }

        DialogueLine currentLine = this.lines.Dequeue();
        this.characterIcon.sprite = currentLine.character.icon;
        this.characterName.text = currentLine.character.name;

        this.currentSentence = currentLine.line;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        this.isTyping = true;
        this.dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            this.dialogueArea.text += letter;
            yield return new WaitForSeconds(this.typingSpeed);
        }
        this.isTyping = false;
    }

    public void EndDialogue()
    {
        this.isDialogueActive = false;
        this.animator.Play("Hide");

        this.onDialogueEndedCallback?.Invoke();
        this.onDialogueEndedCallback = null;
    }

    private void Update()
    {
        this.HandleNextDialogue();
    }

    public void HandleNextDialogue()
    {
        if (!Input.GetKeyDown(KeyCode.Z) && !Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKeyDown(KeyCode.X) || !this.isDialogueActive) return;

        this.DisplayNextDialogueLine();
    }
}
