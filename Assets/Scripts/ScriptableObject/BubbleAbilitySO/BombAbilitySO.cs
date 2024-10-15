using UnityEngine;

[CreateAssetMenu(fileName = "BombAbilitySO", menuName = "ScriptableObjects/BubbleAbility/BombAbilitySO", order = 2)]
public class BombAbilitySO : BubbleAbilitySO
{
    public float m_Range;

    public override void Execute(Bubble owner)
    {
        var hits = Physics2D.OverlapCircleAll(owner.Position, m_Range, LayerMask.Bubble);
        foreach (var hit in hits)
        {
            if (hit.gameObject == owner.gameObject)
                continue;

            var bubble = hit.GetComponent<Bubble>();
            if (bubble.StateType == EBubbleStateType.Burst)
                continue;

            bubble.BurstWithNotify();
        }
        BubbleSystem.Instance.DropUnconnectedBubbles(owner.Position.y);
    }
}