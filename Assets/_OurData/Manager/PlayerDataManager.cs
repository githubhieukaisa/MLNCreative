using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : TeamBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public event Action<StatType, int> OnStatChanged;

    // Dictionary lưu trữ giá trị: Key là Enum, Value là số nguyên
    private Dictionary<StatType, int> statsDict = new Dictionary<StatType, int>();

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    protected override void ResetValue()
    {
        SetStat(StatType.Capital, 100);
        SetStat(StatType.Brand, 50);
        SetStat(StatType.Tech, 10);
    }

    public int GetStat(StatType type)
    {
        if (statsDict.ContainsKey(type)) return statsDict[type];
        return 0;
    }

    private void SetStat(StatType type, int value)
    {
        if (statsDict.ContainsKey(type))
        {
            statsDict[type] = value;
        }
        else
        {
            statsDict.Add(type, value);
        }

        OnStatChanged?.Invoke(type, value);
    }

    public void ModifyStat(StatType type, int amount)
    {
        int currentValue = GetStat(type);
        int newValue = currentValue + amount;

        // Logic chặn không cho chỉ số âm (ngoại trừ Capital có thể âm để game over)
        if (type != StatType.Capital && newValue < 0) newValue = 0;

        SetStat(type, newValue);

        Debug.Log($"Stat Modified: {type} changed by {amount}. New Value: {newValue}");
    }
}
