using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : TeamBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image _characterIcon;
    [SerializeField] private TextMeshProUGUI _characterNameText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _dialoguePanel;

    [Header("Settings")]
    [SerializeField] private float _typingSpeed = 0.02f;
    [SerializeField] private AudioSource _audioSource;

    // Runtime State
    public bool IsActive { get; private set; } = false; // Public read-only để Trigger kiểm tra
    private Queue<DialogueLine> _linesQueue = new();
    private bool _isTyping = false;
    private string _currentFullSentence = "";
    private Action _onDialogueEndedCallback;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (_dialoguePanel) _dialoguePanel.SetActive(false);
    }

    // Nếu dùng TeamBehaviour thì giữ, nếu dùng MonoBehaviour thì xóa override
    protected override void LoadComponents()
    {
        if (!_audioSource) _audioSource = GetComponent<AudioSource>();
    }

    // --- INPUT HANDLING (MỚI) ---
    private void Update()
    {
        // Chỉ lắng nghe khi hội thoại đang bật
        if (!IsActive) return;

        // Bấm chuột trái, Space, hoặc Enter để next
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            OnNextAndInteraction();
        }
    }

    public void StartDialogue(DialogueData data, Action onEnded = null)
    {
        if (IsActive) return; // Chặn spam

        IsActive = true;
        _onDialogueEndedCallback = onEnded;
        _linesQueue.Clear();

        foreach (var line in data.lines)
        {
            _linesQueue.Enqueue(line);
        }

        if (_dialoguePanel) _dialoguePanel.SetActive(true);

        DisplayNextLine();
    }

    public void OnNextAndInteraction()
    {
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (_isTyping)
        {
            StopAllCoroutines();
            _dialogueText.text = _currentFullSentence;
            _isTyping = false;
            return;
        }

        if (_linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = _linesQueue.Dequeue();

        if (currentLine.speaker != null)
        {
            _characterNameText.text = currentLine.speaker.characterName;
            _characterIcon.sprite = currentLine.speaker.defaultIcon;

            AudioClip clipToPlay = currentLine.voiceClipOverride != null
                ? currentLine.voiceClipOverride
                : currentLine.speaker.defaultVoice;

            PlayAudio(clipToPlay);
        }

        _currentFullSentence = currentLine.content;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine.content));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        _isTyping = true;
        _dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTyping = false;
    }

    private void PlayAudio(AudioClip clip)
    {
        if (clip && _audioSource)
        {
            _audioSource.Stop();
            _audioSource.PlayOneShot(clip);
        }
    }

    private void EndDialogue()
    {
        IsActive = false;
        if (_dialoguePanel) _dialoguePanel.SetActive(false);

        _onDialogueEndedCallback?.Invoke();
        _onDialogueEndedCallback = null;
    }
}