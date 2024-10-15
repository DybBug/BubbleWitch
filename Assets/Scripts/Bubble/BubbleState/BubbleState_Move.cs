// Move
using System.Collections.Generic;
using UnityEngine;

public class BubbleState_Move : BubbleState
{
    private readonly Queue<Vector2> m_PathQueue = new();
    private float m_Power;
    private Vector2 m_TargetPosition;
    private Vector2 m_attachPosition;
    private bool m_IsWithNotify;

    public override EBubbleStateType StateType => EBubbleStateType.Move;
    public BubbleState_Move(BubbleStateMachine bubbleStateMachine) : base(bubbleStateMachine)
    {
    }

    public void Setup(List<Vector2> path, float power, Vector2 attachPosition, bool isWithNotify)
    {
        if (path.Count == 0)
            Transition(EBubbleStateType.Idle);

        m_PathQueue.Clear();
        foreach (var target in path)
        {
            m_PathQueue.Enqueue(target);
        }

        m_Power = power;
        m_attachPosition = attachPosition;
        m_IsWithNotify = isWithNotify;
        m_TargetPosition = m_PathQueue.Dequeue();
    }


    protected override void OnEnter()
    {

    }

    protected override void OnUpdate(float deltaTime)
    {
        var diff = (m_TargetPosition - Owner.Position);
        var deltaPos = diff.normalized * m_Power * deltaTime;
        if (diff.magnitude > Bubble.Radius && deltaPos.magnitude < diff.magnitude)
        {
            Owner.Position = Owner.Position + deltaPos;
        }
        else
        {
            if (m_PathQueue.Count <= 0)
            {
                Owner.transform.position = m_attachPosition;
                if (m_IsWithNotify)
                {
                    BubbleSystem.Instance.AttachBubbleWithNotify(Owner);
                }
                Transition(EBubbleStateType.Idle);
                return;
            }

            Owner.Position = m_TargetPosition;
            m_TargetPosition = m_PathQueue.Dequeue();
            return;

        }
    }

    protected override void OnExit()
    {
        m_PathQueue.Clear();
    }
}