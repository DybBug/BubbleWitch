public class BubbleState_Idle : BubbleState
{
    public override EBubbleStateType StateType => EBubbleStateType.Idle;

    public BubbleState_Idle(BubbleStateMachine bubbleStateMachine) : base(bubbleStateMachine)
    {
    }

    protected override void OnEnter()
    {

    }

    protected override void OnUpdate(float deltaTime)
    {

    }

    protected override void OnExit()
    {

    }
}