using UnityEngine;
using UnityEngine.UI;

public class SceneVisualManager : TeamBehaviour
{
    public static SceneVisualManager Instance { get; private set; }
    [Header("UI References")]
    [SerializeField] private Image imgBackground;
    [SerializeField] private Image imgCharacter;
    [SerializeField] private Animator characterAnimator;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void ApplyVisualState(VisualState state)
    {
        if (state == null) return;

        // 1. Xử lý Đổi Nền (Background)
        if (state.BackgroundSprite != null)
        {
            imgBackground.sprite = state.BackgroundSprite;
        }

        // 2. Xử lý Đổi Nhân vật / Cảm xúc
        if (state.CharacterSprite != null)
        {
            imgCharacter.gameObject.SetActive(true);
            imgCharacter.sprite = state.CharacterSprite;
            imgCharacter.SetNativeSize();
        }

        // 3. Xử lý Cử động nhỏ (Nhíu mày, nảy lên, rung lắc...)
        if (!string.IsNullOrEmpty(state.CharacterAnimTrigger) && characterAnimator != null)
        {
            characterAnimator.SetTrigger(state.CharacterAnimTrigger);
        }
    }

    public void HideCharacter()
    {
        imgCharacter.gameObject.SetActive(false);
    }
}