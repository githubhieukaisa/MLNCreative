using UnityEngine;
using System.Collections.Generic;
using System;

// 1. Class cha: Đại diện cho MỌI bưới đi trong game
public abstract class ScenarioStep : ScriptableObject
{
    // Không cần code gì ở đây, chỉ dùng để làm Type
}

// 2. Bước Hội Thoại (Chỉ chạy chữ, không dừng lại hỏi)
[CreateAssetMenu(fileName = "NewDialogueStep", menuName = "GameData/Steps/Dialogue Step")]
public class DialogueStep : ScenarioStep
{
    public DialogueData dialogueData;
    public Sprite backgroundChange; // (Optional) Đổi nền nếu cần
}

// 3. Bước Câu Hỏi (Dừng lại chờ người chơi chọn)
[CreateAssetMenu(fileName = "NewQuestionStep", menuName = "GameData/Steps/Question Step")]
public class QuestionStep : ScenarioStep
{
    [TextArea(3, 5)] public string questionText;
    public Option optionA;
    public Option optionB;
    public Option optionC;
}

// 4. Level Data: Chứa một DANH SÁCH các bước
[CreateAssetMenu(fileName = "NewLevel", menuName = "GameData/Level Sequence")]
public class LevelData : ScriptableObject
{
    public string levelName;
    // Điểm mấu chốt: List này chứa cả DialogueStep lẫn QuestionStep
    public List<ScenarioStep> steps;
}

[Serializable]
public class Option
{
    [Header("Nội dung lựa chọn")]
    [TextArea(2, 3)] public string text; // Chữ hiển thị trên nút bấm

    [Header("Thay đổi chỉ số")]
    public int capitalChange; // Thay đổi vốn (VD: -10, +20)
    public int brandChange;   // Thay đổi uy tín
    public int techChange;    // Thay đổi công nghệ

    [Header("Kết quả/Bài học")]
    [TextArea(3, 5)] public string feedback;
}
