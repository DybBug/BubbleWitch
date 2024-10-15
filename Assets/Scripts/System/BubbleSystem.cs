using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BubbleSystem : MonoBehaviour
{
    #region Instance
    private static BubbleSystem _instance = null;
    public static BubbleSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                var gameObject = new GameObject("BubbleSystem");
                _instance = gameObject.AddComponent<BubbleSystem>();

            }

            return _instance;
        }
    }
    #endregion

    #region Property
    public uint DropBubbleCount { get; set; }
    #endregion

    #region Field
    private readonly HashSet<Bubble> m_BubbleSet = new();
    private readonly HashSet<Bubble> m_MatchBubbleSet = new();
    private readonly HashSet<Bubble> m_ConnectedBubbleSet = new();
    private float m_DropBoundaryY;
    private Map m_Map;
    private Boss m_Boss;
    private uint m_GridShiftUpCount = 0;

    private DropPoint m_DropPoint;
    #endregion


    #region Unity
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Initialize();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void OnDestroy()
    {
        
    }

    private void Update()
    {
        BubbleSystemState.Update();
    }
    #endregion

    public void AttachBubbleWithNotify(Bubble bubble)
    {
        var position = bubble.transform.position;
        m_BubbleSet.Add( bubble);

        m_DropBoundaryY = float.MinValue;

        m_MatchBubbleSet.Add(bubble);
        MatchBubblesBFS(bubble);

        if (m_MatchBubbleSet.Any(e => (e.AbilitySO is BombAbilitySO)) || m_MatchBubbleSet.Count >= 3)
        {
            BurstBubblesWithNotify();
            DropUnconnectedBubblesWithNotify(m_DropBoundaryY);

            if (m_GridShiftUpCount > 0)
            {
                var minPosY = m_ConnectedBubbleSet.Min(e => e.Position.y);
                var diffCount = Mathf.Ceil(Mathf.Abs(minPosY - m_Map.BottomLinePosition.y) / Bubble.Radius);
                var shiftCount = Mathf.Min(diffCount, m_GridShiftUpCount);
                m_Map.ShiftGridDown((uint)shiftCount);
                m_GridShiftUpCount -= (uint)shiftCount;
            }
        }
        else
        {
            if (m_Map.BottomLinePosition.y >= bubble.Position.y)
            {
                m_Map.ShiftGridUp();
                ++m_GridShiftUpCount;
            }
            BubbleSystemState.SetState(EBubbleSystemState.WaitingForShoot);
        }

        m_MatchBubbleSet.Clear();
    }

    public Boss FindBossOrNull()
    {
        return m_Boss;
    }

    public Bubble GetBubbleOrNull(Vector2 pos)
    {
        return m_BubbleSet.FirstOrDefault(e => e.Position == pos);
    }

    public Bubble Spawn(Vector2 position, BubbleSO bubbleSO)
    {
        var bubble = BubblePool.Instance.SpawnBubble(position, bubbleSO);
        bubble.OnBurstEvent += OnBubbleBurst;
        bubble.OnDropEvent += OnBubbleDrop;
  
        m_BubbleSet.Add(bubble);
        m_Map.AddBubble(bubble);
        return bubble;
    }

    private void Initialize()
    {
        m_Map = FindObjectOfType<Map>();
        m_Boss = FindObjectOfType<Boss>();
        m_DropPoint = FindObjectOfType<DropPoint>();
        m_GridShiftUpCount = 0;
    }

    private void BurstBubblesWithNotify()
    {
        BubbleSystemState.SetState(EBubbleSystemState.BurstingBubbles);

        foreach (var matchBubble in m_MatchBubbleSet)
        {
            if (!m_BubbleSet.Contains(matchBubble))
                continue;
            matchBubble.BurstWithNotify();
        }
    }

    private void MatchBubblesBFS(Bubble shootBubble)
    {
        var closeSet = new HashSet<Bubble>();
        var bubbleQueue = new Queue<Bubble>();

        bubbleQueue.Enqueue(shootBubble);
        closeSet.Add(shootBubble);

        while (bubbleQueue.Count > 0)
        {
            var currentBubble = bubbleQueue.Dequeue();
            var neighborBubbles = currentBubble.GetNeighborBubbles();

            foreach (var neighborBubble in neighborBubbles)
            {
                if (!closeSet.Add(neighborBubble))
                    continue;

                if ((neighborBubble.MathFlag & shootBubble.MathFlag) != 0)
                {
                    if ((neighborBubble.AbilitySO is BombAbilitySO))
                    {
                        if (currentBubble != shootBubble)
                        {
                            continue;
                        }
                    }

                    if (m_DropBoundaryY <= neighborBubble.transform.position.y)
                    {
                        m_DropBoundaryY = neighborBubble.transform.position.y;
                    }
                    m_MatchBubbleSet.Add(neighborBubble);
                    bubbleQueue.Enqueue(neighborBubble);
                }
            }
        }
    }
    public void DropUnconnectedBubblesWithNotify(float dropBoundary)
    {
        DropUnconnectedBubbles(dropBoundary);
        if (DropBubbleCount > 0)
        {
            BubbleSystemState.SetState(EBubbleSystemState.DroppingUnconnectedBubbles);
        }
        else
        {
            BubbleSystemState.SetState(EBubbleSystemState.SpawningNewBubbles);
        }
    }

    public void DropUnconnectedBubbles(float dropBoundary)
    {
        m_ConnectedBubbleSet.RemoveWhere(e => e.Position.y <= dropBoundary);

        var pendingBubbleSet = new HashSet<Bubble>();
        FilterBubblesByBoundary(m_ConnectedBubbleSet, pendingBubbleSet);
        SearchConnectedBubblesBelowBoundary(m_ConnectedBubbleSet, pendingBubbleSet);
        DropUnconnectedBubbles(m_ConnectedBubbleSet);
    }


    private void FilterBubblesByBoundary(HashSet<Bubble> connectedBubbleSet, HashSet<Bubble> pendingBubbleSet)
    {
        foreach (var bubble in m_BubbleSet)
        {
            if (m_MatchBubbleSet.Contains(bubble))
                continue;

            if (m_DropBoundaryY < bubble.transform.position.y)
            {
                connectedBubbleSet.Add(bubble);
            }
            else
            {
                pendingBubbleSet.Add(bubble);
            }
        }
    }

    private void SearchConnectedBubblesBelowBoundary(HashSet<Bubble> connectedBubbleSet, HashSet<Bubble> pendingBubbleSet)
    {
        var closeBubbleSet = new HashSet<Bubble>();
        foreach(var pendingBubble in pendingBubbleSet)
        {
            if (!closeBubbleSet.Add(pendingBubble))
                continue;

            if (connectedBubbleSet.Contains(pendingBubble))
                continue;

            if (CheckConnectNeighborBubbleBFS(pendingBubble, connectedBubbleSet, closeBubbleSet))
            {
                connectedBubbleSet.Add(pendingBubble);
            }
        }
    }

    private bool CheckConnectNeighborBubbleBFS(Bubble bubble, HashSet<Bubble> connectedBubbleSet, HashSet<Bubble> closeBubbleSet)
    {
        var close = new HashSet<Bubble>();
        var queue = new Queue<Bubble>();
        queue.Enqueue(bubble);

        while(queue.Count > 0)
        {
            var currBubble = queue.Dequeue();
            if (!close.Add(currBubble))
                continue;

            if (m_Map.CheckTopLineOverlap(bubble))
            {
                connectedBubbleSet.Add(bubble);
                return true;
            }

            foreach (var neighborBubble in currBubble.GetNeighborBubbles())
            {
                if (m_MatchBubbleSet.Contains(neighborBubble))
                    continue;

                if (connectedBubbleSet.Contains(neighborBubble))
                {
                    connectedBubbleSet.Add(bubble);
                    continue;
                }

                if (close.Contains(neighborBubble))
                    continue;

                queue.Enqueue(neighborBubble);
            }
        }

        return false;
    }

    private void DropUnconnectedBubbles(HashSet<Bubble> connectedBubbleSet)
    {
        foreach (var bubble in m_BubbleSet.ToList())
        {
            if (!connectedBubbleSet.Contains(bubble))
            {
                ++DropBubbleCount;
                bubble.Drop(m_DropPoint.transform.position);
            }
        }
    }


    private void CheckConnectNeighborBubble(Bubble bubble, HashSet<Bubble> connectedBubbleSet, HashSet<Bubble> closeBubbleSet)
    {
        if (!closeBubbleSet.Add(bubble))
            return;

        foreach (var neighbor in bubble.GetNeighborBubbles())
        {
            if (closeBubbleSet.Contains(neighbor))
                continue;

            var isConnected = connectedBubbleSet.Contains(neighbor);
            if (isConnected)
            {
                connectedBubbleSet.Add(bubble);
                return;
            }
            CheckConnectNeighborBubble(neighbor, connectedBubbleSet, closeBubbleSet);

        }
        return;
    }

    private void OnBubbleBurst(Bubble bubble)
    {
        bubble.OnBurstEvent -= OnBubbleBurst;
        m_BubbleSet.Remove(bubble);
    }

    private void OnBubbleDrop(Bubble bubble)
    {
        bubble.OnDropEvent -= OnBubbleDrop;
        m_BubbleSet.Remove(bubble);
    }
}
