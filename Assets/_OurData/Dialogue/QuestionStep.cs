using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestionStep", menuName = "GameData/Steps/Question Step")]
public class QuestionStep : ScenarioStep
{
    [TextArea(3, 5)] public string questionText;
    public Option optionA;
    public Option optionB;
    public Option optionC;
}