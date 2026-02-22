using UnityEngine;
using System;

// 1. Class cha: Đại diện cho MỌI bưới đi trong game
public abstract class ScenarioStep : ScriptableObject
{
    // Không cần code gì ở đây, chỉ dùng để làm Type
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

    [Header("Rẽ Nhánh Cốt Truyện (Branching)")]
    [Tooltip("Kéo file LevelData của nhánh truyện mới vào đây. Để trống nếu chỉ muốn đi tiếp bước tiếp theo của danh sách hiện tại.")]
    public LevelData nextBranch;
}
