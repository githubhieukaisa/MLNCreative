using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueStep", menuName = "GameData/Steps/Dialogue Step")]
public class DialogueStep : ScenarioStep
{
    public DialogueData dialogueData;
    [Header("Khởi tạo Hình ảnh lúc bắt đầu")]
    public VisualState startingVisual;
}