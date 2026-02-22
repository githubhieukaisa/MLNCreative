using Core.Audio;
using UnityEngine;

public class GameMusicController : TeamBehaviour
{
    [Header("Crisis Thresholds")]
    [Tooltip("Nếu Vốn dưới mức này, nhạc sẽ chuyển sang dồn dập và bị nghẹt")]
    [SerializeField] private int _crisisCapitalThreshold = 30;

    private void Start()
    {
        // Đăng ký lắng nghe sự kiện từ GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged += HandleStatsChanged;
            GameManager.Instance.OnGameEnded += HandleGameEnded;
        }

        // Bắt đầu game với nhạc bình thường
        AudioManager.Instance.PlayMusic(MusicType.Gameplay);
    }

    private void OnDestroy()
    {
        // Hủy đăng ký để tránh lỗi Memory Leak khi chuyển Scene
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged -= HandleStatsChanged;
            GameManager.Instance.OnGameEnded -= HandleGameEnded;
        }
    }

    private void HandleStatsChanged(int capital, int brand, int tech)
    {
        // Nếu Vốn rơi vào vùng nguy hiểm -> Bật nhạc Crisis và tăng độ nghẹt thở
        if (capital <= _crisisCapitalThreshold && capital > 0)
        {
            AudioManager.Instance.PlayMusic(MusicType.Crisis);

            // Tính toán % căng thẳng (Từ 0.0 -> 1.0). 
            // Vốn càng gần 0, intensity càng gần 1 (nhạc càng nghẹt)
            float stressIntensity = 1f - ((float)capital / _crisisCapitalThreshold);
            AudioManager.Instance.SetStressLevel(stressIntensity);
        }
        else if (capital > _crisisCapitalThreshold)
        {
            // Vốn an toàn -> Trở lại nhạc bình thường
            AudioManager.Instance.PlayMusic(MusicType.Gameplay);
            AudioManager.Instance.SetStressLevel(0f);
        }
    }

    private void HandleGameEnded(bool isVictory)
    {
        // Trả lại âm thanh trong trẻo trước khi phát nhạc kết thúc
        AudioManager.Instance.SetStressLevel(0f);

        if (isVictory)
        {
            AudioManager.Instance.PlayMusic(MusicType.Victory);
        }
        else
        {
            AudioManager.Instance.PlayMusic(MusicType.Defeat);
        }
    }
}