using UnityEngine;

public class BubbleState_Drop : BubbleState
{
    public override EBubbleStateType StateType => EBubbleStateType.Drop;

    private Vector2 m_DropPointPosition;
    private Vector2 m_Velocity;
    private float m_DropSpeed = 10.0f;

    public BubbleState_Drop(BubbleStateMachine stateMachine) : base(stateMachine)
    {
    }

    public void Setup(Vector2 dropPointPosition)
    {
        m_DropPointPosition = dropPointPosition;
        m_Velocity = (m_DropPointPosition - Owner.Position).normalized * m_DropSpeed;
    }

    protected override void OnEnter()
    {
        
    }

    protected override void OnExit()
    {
        --BubbleSystem.Instance.DropBubbleCount;
        if (BubbleSystem.Instance.DropBubbleCount <= 0)
        {
            BubbleSystem.Instance.DropBubbleCount = 0;
            BubbleSystemState.SetState(EBubbleSystemState.SpawningNewBubbles);
        }
    }
    protected override void OnUpdate(float deltaTime)
    {
        Owner.Position += m_Velocity * deltaTime;
    }
}