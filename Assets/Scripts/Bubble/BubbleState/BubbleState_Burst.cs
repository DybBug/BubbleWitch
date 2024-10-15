public class BubbleState_Burst : BubbleState
{
    public override EBubbleStateType StateType => EBubbleStateType.Burst;

    public BubbleState_Burst(BubbleStateMachine bubbleStateMachine) : base(bubbleStateMachine)
    {
    }

    protected override void OnEnter()
    {
        Owner.AbilitySO?.Execute(Owner);
    }

    protected override void OnUpdate(float deltaTime)
    {

    }

    protected override void OnExit()
    {
    }
}