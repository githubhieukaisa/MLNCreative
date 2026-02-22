using UnityEngine;

[CreateAssetMenu(fileName = "NewProfile", menuName = "Dialogue/Character Profile")]
public class CharacterProfileSO : ScriptableObject
{
    public string characterName;
    public Sprite defaultIcon;
    public AudioClip defaultVoice; // Voice mặc định (nếu có)
}