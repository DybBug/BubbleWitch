using System.Collections.Generic;

public enum EBubbleSystemState
{
    Waiting,
    Start,
    WaitingForShoot,
    BurstingBubbles,
    DroppingUnconnectedBubbles,
    SpawningNewBubbles,
}

public class BubbleSystemState
{
    public static BubbleSystemState Instance = new();

    private EBubbleSystemState m_CurrentState = EBubbleSystemState.Waiting;
    private BubbleShooter m_Shooter;
    private readonly List<BubbleSpawner> m_BubbleSpawners = new();

    public static void Register(BubbleShooter bubbleShooter)
    {
        Instance.m_Shooter = bubbleShooter;
    }

    public static void Register(BubbleSpawner bubbleSpawner)
    {
        Instance.m_BubbleSpawners.Add(bubbleSpawner);
    }

    public static void SetState(EBubbleSystemState state)
    {
        Instance.m_CurrentState = state;
        switch (state)
        {
            case EBubbleSystemState.Waiting:
            {
                Instance.m_Shooter.Deactivate();
                break;
            }
            case EBubbleSystemState.Start:
            {
                foreach (var spawner in Instance.m_BubbleSpawners)
                {
                    spawner.SpawnBubbles();
                    spawner.MoveBubbleSequentially();
                }
                break;
            }
            case EBubbleSystemState.WaitingForShoot:
            {
                Instance.m_Shooter.Activate();
                break;
            }
            case EBubbleSystemState.BurstingBubbles:
            {
                Instance.m_Shooter.Deactivate();
                break;
            }
            case EBubbleSystemState.DroppingUnconnectedBubbles:
            {
                Instance.m_Shooter.Deactivate();
                break;
            }
            case EBubbleSystemState.SpawningNewBubbles:
            {
                Instance.m_Shooter.Deactivate();
                foreach (var spawner in Instance.m_BubbleSpawners)
                {
                    spawner.SpawnBubbles();
                    spawner.MoveBubbles();
                }
                break;
            }
        }
    }

    public static void Update()
    {
        if (Instance.m_CurrentState == EBubbleSystemState.Waiting)
            SetState(EBubbleSystemState.Start);
    }

}