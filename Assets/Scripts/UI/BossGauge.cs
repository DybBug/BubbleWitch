using UnityEngine;
using UnityEngine.UI;

public class BossGauge : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private Slider m_Slider;
    #endregion

    #region Unity
    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    #endregion

    public void Setup()
    {
        var boss = BubbleSystem.Instance.FindBossOrNull();
        gameObject.SetActive(boss != null);
        if (boss != null)
        {
            boss.OnChangedHPEvent += UpdateSliderValue;
            UpdateSliderValue(boss.HP, boss.HP);
        }
    }

    private void UpdateSliderValue(uint currHP, uint maxHP)
    {
        Debug.Assert(maxHP > 0);
        m_Slider.value = (float)currHP / (float)maxHP;
    }
}
