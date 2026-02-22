using UnityEngine;

[System.Serializable]
public class VisualState
{
    [Header("Background")]
    [Tooltip("Để trống nếu không muốn đổi nền")]
    public Sprite BackgroundSprite;

    [Header("Character")]
    [Tooltip("Để trống nếu không đổi nhân vật")]
    public Sprite CharacterSprite;

    [Tooltip("Tên trigger Animation (Ví dụ: 'Frown', 'Shake'). Để trống nếu không có.")]
    public string CharacterAnimTrigger;
}