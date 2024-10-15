using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bubble : MonoBehaviour
{
    public static float Radius = 0.5f;

    #region SerializeField
    [SerializeField] private BubbleSO m_BubbleSO;
    [SerializeField] private SpriteRenderer m_BubbleSpriteRenderer;
    [SerializeField] private SpriteRenderer m_InnerSpriteRenderer;
    [SerializeField] private Rigidbody2D m_Rigidbody;
    #endregion

    #region Property
    public Vector2 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public EMatchFlag MathFlag => m_BubbleSO?.MatchFlag ?? EMatchFlag.None;
    public bool IsSpawnable { get; private set; }
    public Spawner Spawner { get; set; }
    public BubbleAbilitySO AbilitySO => m_BubbleSO.BubbleAbilitySO;
    public EBubbleStateType StateType => m_StateMachine.CurrentStateType;
    #endregion

    #region Field
    private BubbleStateMachine m_StateMachine;
    #endregion

    #region Events
    public event UnityAction<Bubble> OnBurstEvent;
    public event UnityAction<Bubble> OnDropEvent;
    #endregion

    #region Unity

    private void Awake()
    {
        m_StateMachine = new BubbleStateMachine(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_BubbleSO != null)
        {
            ApplyBubbleSO(m_BubbleSO);
        }
    }

    private void FixedUpdate()
    {
        m_StateMachine.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        OnBurstEvent = null;
        OnDropEvent = null;
    }
    #endregion

    public List<Bubble> GetNeighborBubbles()
    {
        var neighborBubbles = new List<Bubble>();

        var hits = Physics2D.OverlapCircleAll(Position, Radius * 1.5f, LayerMask.Bubble);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            var bubble = hit.GetComponent<Bubble>();
            neighborBubbles.Add(bubble);
        }
        return neighborBubbles;
    }


    public void BurstWithNotify()
    {
        OnBurstEvent?.Invoke(this);
        Despawn();
        OnBurst();
    }

    public void Spawn(BubbleSO bubbleSO)
    {
        m_StateMachine.Transition(EBubbleStateType.Idle);

        ApplyBubbleSO(bubbleSO);
        IsSpawnable = false;
        gameObject.SetActive(true);
    }

    public void Despawn()
    {
        m_StateMachine.Transition(EBubbleStateType.Idle);
        IsSpawnable = true;
        gameObject.SetActive(false);
        Spawner = null;
        OnBurstEvent = null;
    }

    public void Drop(Vector2 dropPointPosition)
    {
        OnDropEvent?.Invoke(this);
        m_StateMachine.Transition(EBubbleStateType.Drop, (state) =>
        {
            if (state is BubbleState_Drop dropState)
            {
                dropState.Setup(dropPointPosition);
            }
        });
    }

    public void Move(List<Vector2> path, float power, Vector2 attachPosition, bool isWithNotify = true)
    {
        m_StateMachine.Transition(EBubbleStateType.Move, (state) =>
        {
            if (state is BubbleState_Move moveState)
            {
                moveState.Setup(path, power, attachPosition, isWithNotify);
            }
        });
    }


    private void ApplyBubbleSO(BubbleSO bubbleSO)
    {
        if (bubbleSO != null)
        {
            m_BubbleSO = bubbleSO;

            m_BubbleSpriteRenderer.sprite = bubbleSO.BubbleSprite;
            m_BubbleSpriteRenderer.color = bubbleSO.BubbleColor;

            m_InnerSpriteRenderer.sprite = bubbleSO.InnerSprite;
            m_InnerSpriteRenderer.color = bubbleSO.InnerColor;
        }
    }

    protected void OnBurst()
    {
        m_StateMachine.Transition(EBubbleStateType.Burst);
    }
}
