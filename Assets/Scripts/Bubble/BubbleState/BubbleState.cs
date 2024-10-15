using System;
using UnityEngine.Events;

public enum EBubbleStateType
{
    Idle,
    Move,
    Drop,
    Burst
}

public abstract class BubbleState
{
    public abstract EBubbleStateType StateType { get; }
    private BubbleStateMachine m_StateMachine;

    public BubbleState(BubbleStateMachine stateMachine)
    {
        m_StateMachine = stateMachine;
    }

    public void Enter() => OnEnter();
    public void Update(float deltaTime) => OnUpdate(deltaTime);
    public void Exit() => OnExit();

    protected abstract void OnEnter();
    protected abstract void OnUpdate(float deltaTime);
    protected abstract void OnExit();
    protected void Transition(EBubbleStateType stateType) => m_StateMachine.Transition(stateType);
    protected Bubble Owner => m_StateMachine.Owner;
}

public class BubbleStateMachine
{
    public Bubble Owner { get; private set; }
    public EBubbleStateType CurrentStateType { get; private set; } = EBubbleStateType.Idle;
    private BubbleState[] m_BubbleStates = new BubbleState[Enum.GetValues(typeof(EBubbleStateType)).Length];

    public BubbleStateMachine(Bubble owner)
    {
        Owner = owner;
        m_BubbleStates[(int)EBubbleStateType.Idle] = new BubbleState_Idle(this);
        m_BubbleStates[(int)EBubbleStateType.Move] = new BubbleState_Move(this);
        m_BubbleStates[(int)EBubbleStateType.Drop] = new BubbleState_Drop(this);
        m_BubbleStates[(int)EBubbleStateType.Burst] = new BubbleState_Burst(this);
    }


    public void Transition(EBubbleStateType stateType, UnityAction<BubbleState> preEnterCB = null)
    {
        m_BubbleStates[(int)CurrentStateType].Exit();
        CurrentStateType = stateType;

        var currState = m_BubbleStates[(int)CurrentStateType];
        preEnterCB?.Invoke(currState);
        currState.Enter();
    }

    public void Update(float deltaTime)
    {
        m_BubbleStates[(int)CurrentStateType].Update(deltaTime);
    }
}
