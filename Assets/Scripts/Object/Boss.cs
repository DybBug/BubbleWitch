using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boss : MonoBehaviour
{
    #region SerializeField
    [SerializeField] uint m_HP;
    [SerializeField] private List<BubbleSpawner> m_BubbleSpawners;
    #endregion

    #region Property
    public uint HP => m_HP;
    #endregion

    #region Field
    private uint m_CurrentHP;
    private uint m_SpawnEndCount;
    #endregion

    #region Event
    public event UnityAction<uint/*currentHP*/, uint/*maxHP*/> OnChangedHPEvent;
    #endregion

    #region Unity
    private void Start()
    {
        Setup(m_HP);
    }

    private void OnDestroy()
    {
        OnChangedHPEvent = null;
    }
    #endregion

    public void Setup(uint HP)
    {
        m_HP = HP;
        m_CurrentHP = HP;
        m_SpawnEndCount = 0;
        foreach (var spawner in m_BubbleSpawners)
        {
            spawner.OnMoveEndEvent += OnSpawnEnd;
        }
    }

    public void TakeDamage(uint damage)
    {
        var newHP = (uint)Mathf.Max(0, m_CurrentHP - damage);
        if (newHP != m_CurrentHP)
        {
            m_CurrentHP = newHP;
            OnChangedHPEvent?.Invoke(m_CurrentHP, m_HP);
        }
    }

    private void OnSpawnEnd()
    {
        ++m_SpawnEndCount;
        if (m_SpawnEndCount >= m_BubbleSpawners.Count)
        {
            m_SpawnEndCount = 0;
            BubbleSystemState.SetState(EBubbleSystemState.WaitingForShoot);
        }
    }
}