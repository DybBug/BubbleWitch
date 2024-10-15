
using UnityEngine;

[CreateAssetMenu(fileName = "FairyAbilitySO", menuName = "ScriptableObjects/BubbleAbility/FairyAbilitySO", order = 1)]
public class FairyAbilitySO : BubbleAbilitySO
{
    public uint m_Damage;

    public override void Execute(Bubble owner)
    {
        var boss = BubbleSystem.Instance.FindBossOrNull();
        if (boss != null)
        {
            boss.TakeDamage(m_Damage);
        }
    }
}