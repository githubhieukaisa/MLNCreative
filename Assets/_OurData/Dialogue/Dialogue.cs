using System.Collections.Generic;
using UnityEngine;

// 1. Tạo hồ sơ nhân vật (Lưu 1 lần dùng mãi mãi)
[CreateAssetMenu(fileName = "NewProfile", menuName = "Dialogue/Character Profile")]
public class CharacterProfile : ScriptableObject
{
    public string characterName;
    public Sprite defaultIcon;
    public AudioClip defaultVoice; // Voice mặc định (nếu có)
}

[System.Serializable]
public class DialogueLine
{
    [Header("Who is speaking?")]
    public CharacterProfile speaker;

    [Header("Content")]
    [TextArea(3, 5)] public string content;

    [Header("Extras")]
    [Tooltip("Để trống nếu muốn dùng voice mặc định của nhân vật")]
    public AudioClip voiceClipOverride;
}

[CreateAssetMenu(fileName = "NewConversation", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> lines;
}