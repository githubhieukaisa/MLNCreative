using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Header("Who is speaking?")]
    public CharacterProfileSO speaker;

    [Header("Content")]
    [TextArea(3, 5)] public string content;

    [Header("Extras")]
    [Tooltip("Để trống nếu muốn dùng voice mặc định của nhân vật")]
    public AudioClip voiceClipOverride;

    [Header("Hình ảnh & Hoạt ảnh (Visual & Anim)")]
    [Tooltip("Trạng thái visual KHI CÂU THOẠI NÀY HIỆN LÊN")]
    public VisualState lineVisualState;
}

[CreateAssetMenu(fileName = "NewConversation", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> lines;
}