using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "GameData/Level Sequence")]
public class LevelData : ScriptableObject
{
    public string levelName;
    // Điểm mấu chốt: List này chứa cả DialogueStep lẫn QuestionStep
    public List<ScenarioStep> steps;
}