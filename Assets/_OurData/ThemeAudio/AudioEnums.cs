namespace Core.Audio
{
    public enum MusicType
    {
        None,
        MainMenu,
        Gameplay,
        Crisis,
        Defeat,
        Victory
    }

    public enum SFXType
    {
        None,
        UI_Click,       // Tiếng bấm nút
        UI_Hover,       // Tiếng lướt chuột
        Cash_In,        // Tiếng tiền vào (bán được hàng)
        Cash_Out,       // Tiếng tiền ra (trả lương/nhập hàng)
        Correct_Action, // Tiếng chọn đúng/tăng uy tín
        Wrong_Action,   // Tiếng chọn sai/giảm uy tín
        Typewriter      // Tiếng gõ máy chữ khi hiện text
    }
}
