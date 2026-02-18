using UnityEngine;

namespace Core.Audio
{
    public class GameMusicController : TeamBehaviour
    {
        // [SerializeField] private float _pollutionThresholdForMusic = 0.5f; // 50% ô nhiễm đổi nhạc
        // private bool _isUiOpen = false;
        // private float _currentPollutionPercent = 0f;

        // private void Start()
        // {
        //     if (EconomyManager.Instance != null)
        //     {
        //         EconomyManager.Instance.OnPollutionChanged += HandlePollutionChanged;
        //         EconomyManager.Instance.OnBankrupt += HandleGameOver;
        //         EconomyManager.Instance.OnEnvironmentCollapse += HandleGameOver;
        //     }

        //     if (UpgradePanelManager.Instance != null)
        //     {
        //         UpgradePanelManager.Instance.OnPanelOpened += HandlePanelOpened;
        //         UpgradePanelManager.Instance.OnPanelClosed += HandlePanelClosed;
        //     }

        //     CheckAndPlayGameplayMusic();
        // }

        // private void OnDestroy()
        // {
        //     if (EconomyManager.Instance != null)
        //     {
        //         EconomyManager.Instance.OnPollutionChanged -= HandlePollutionChanged;
        //         EconomyManager.Instance.OnBankrupt -= HandleGameOver;
        //         EconomyManager.Instance.OnEnvironmentCollapse -= HandleGameOver;
        //     }

        //     if (UpgradePanelManager.Instance != null)
        //     {
        //         UpgradePanelManager.Instance.OnPanelOpened -= HandlePanelOpened;
        //         UpgradePanelManager.Instance.OnPanelClosed -= HandlePanelClosed;
        //     }
        // }

        // private void HandlePollutionChanged(float current, float percentage)
        // {
        //     _currentPollutionPercent = percentage;
        //     AudioManager.Instance.UpdateMusicEffect(percentage);
        //     if (_isUiOpen) return;
        //     CheckAndPlayGameplayMusic();
        // }

        // private void HandleGameOver()
        // {
        //     AudioManager.Instance.PlayMusic(MusicType.GameOver);
        // }

        // private void HandlePanelOpened()
        // {
        //     _isUiOpen = true;
        //     AudioManager.Instance.PlayMusic(MusicType.MainMenu);
        // }

        // private void HandlePanelClosed()
        // {
        //     _isUiOpen = false;
        //     CheckAndPlayGameplayMusic();
        // }

        // private void CheckAndPlayGameplayMusic()
        // {
        //     if (_currentPollutionPercent >= _pollutionThresholdForMusic)
        //     {
        //         AudioManager.Instance.PlayMusic(MusicType.PollutedGameplay);
        //     }
        //     else
        //     {
        //         AudioManager.Instance.PlayMusic(MusicType.NormalGameplay);
        //     }
        // }
    }
}