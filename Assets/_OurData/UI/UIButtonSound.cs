using UnityEngine;
using UnityEngine.EventSystems;
using Core.Audio;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Sound Settings")]
    [SerializeField] private SFXType _hoverSound = SFXType.UI_Hover;
    [SerializeField] private SFXType _clickSound = SFXType.UI_Click;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverSound != SFXType.None && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(_hoverSound);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_clickSound != SFXType.None && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(_clickSound);
        }
    }
}