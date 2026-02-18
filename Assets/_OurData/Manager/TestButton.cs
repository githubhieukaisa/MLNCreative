using UnityEngine;

public class TestButton : MonoBehaviour
{
    public void OnClickTest()
    {
        // Thử trừ 10 vốn
        PlayerDataManager.Instance.ModifyStat(StatType.Capital, -10);
    }
}
