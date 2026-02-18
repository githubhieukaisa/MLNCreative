using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUIManager : TeamBehaviour
{
    [System.Serializable]
    public class StatUIConfig
    {
        public StatType type;
        public TextMeshProUGUI textComponent;
        public string prefixLabel;
    }

    [Header("UI Configuration")]
    [SerializeField] private List<StatUIConfig> statConfigs;

    private Dictionary<StatType, StatUIConfig> uiLookup = new();

    protected override void LoadComponents()
    {
        base.LoadComponents();
        foreach (var config in statConfigs)
        {
            if (!uiLookup.ContainsKey(config.type))
            {
                uiLookup.Add(config.type, config);
            }
        }
    }

    private void Start()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.OnStatChanged += UpdateStatUI;
            RefreshAllStats();
        }
    }

    private void OnDestroy()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.OnStatChanged -= UpdateStatUI;
        }
    }

    private void UpdateStatUI(StatType type, int newValue)
    {
        if (uiLookup.TryGetValue(type, out StatUIConfig config))
        {
            config.textComponent.SetText($"{config.prefixLabel} {newValue}");
        }
    }

    private void RefreshAllStats()
    {
        foreach (var config in statConfigs)
        {
            int val = PlayerDataManager.Instance.GetStat(config.type);
            UpdateStatUI(config.type, val);
        }
    }
}
