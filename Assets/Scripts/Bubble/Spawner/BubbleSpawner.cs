using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{

}

public class BubbleSpawner : Spawner
{
    public class SpawnNode
    {
        public Bubble Bubble;
        public int PositionIndex;

        public SpawnNode(Bubble bubble, int positionIndex)
        {
            Bubble = bubble;
            PositionIndex = positionIndex;
        }
    }

    #region SerializeField
    [SerializeField] private SpawnPatternSO m_SpawnPatternSO;
    #endregion

    #region Event
    public event UnityAction OnMoveEndEvent;
    #endregion

    private readonly List<SpawnNode> m_Nodes = new();
    private const float SPAWN_TIME = 0.1f;
    private WaitForSeconds m_WaitForSeconds = new(SPAWN_TIME);
    private int m_SpawnCount;

    #region Unity
    private void Awake()
    {
        BubbleSystemState.Register(this);
    }
    private void Start()
    {
        Setup(m_SpawnPatternSO);

    }

    private void OnDestroy()
    {
        OnMoveEndEvent = null;
    }
    #endregion

    public void Setup(SpawnPatternSO patternSO)
    {
        m_SpawnPatternSO = patternSO;
        m_SpawnCount = m_SpawnPatternSO.Positions.Count;
    }

    public void MoveBubbleSequentially()
    {
        StartCoroutine("MoveBubbleSequentiallyCoroutine");
    }


    public IEnumerator MoveBubbleSequentiallyCoroutine()
    {
        m_SpawnCount = m_SpawnPatternSO.Positions.Count;
        for(var i = 0; i < m_Nodes.Count; ++i)
        {
            MoveBubble(i, m_SpawnCount);
            --m_SpawnCount;
            yield return m_WaitForSeconds;
        }
        OnMoveEndEvent?.Invoke();
    }

    public void MoveBubbles()
    {
        for (var i = 0; i < m_Nodes.Count; ++i)
        {
            var prevMovePosition = m_Nodes[i].PositionIndex;
            MoveBubble(i, m_SpawnCount);
            if (prevMovePosition < 0)
            {
                --m_SpawnCount;
            }
        }
        OnMoveEndEvent?.Invoke();
    }

    private void MoveBubble(int nodeIndex, int moveCount)
    {
        var path = FindMovePath(m_Nodes[nodeIndex], moveCount);
        if (path.Count <= 0)
            return;

        m_Nodes[nodeIndex].Bubble.Move(path, SPAWN_TIME * 100, path[path.Count - 1], false);
    }

    private List<Vector2> FindMovePath(SpawnNode node, int moveCount)
    {
        var path = new List<Vector2>();

        var beginIndex = node.PositionIndex < 0 ? 0 : node.PositionIndex;
        var endPositionIndex = moveCount + beginIndex - (node.PositionIndex < 0 ? 1 : 0);
        if (endPositionIndex > m_SpawnPatternSO.Positions.Count - 1)
            return path;

        for (var i = beginIndex; i <= endPositionIndex; ++i)
        {
            var offsetPos = m_SpawnPatternSO.Positions[i];

            var signX = (m_SpawnPatternSO.ReversePosition & EReversePosition.X) != 0 ? -1 : +1;
            var signY = (m_SpawnPatternSO.ReversePosition & EReversePosition.Y) != 0 ? -1 : +1;

            path.Add(new Vector2()
            {
                x = transform.position.x + (signX * offsetPos.x) - (signX * ((offsetPos.y % 2) * Bubble.Radius)),
                y = transform.position.y + (signY * offsetPos.y)
            });
        }
        node.PositionIndex = endPositionIndex;
        return path;
    }

    public void SpawnBubbles()
    {
        for (int i = 0; i < m_SpawnCount; ++i)
        {
            var randomBubbleSO = m_SpawnPatternSO.BubbleSOs[Random.Range(0, m_SpawnPatternSO.BubbleSOs.Count)];
            var bubble = BubbleSystem.Instance.Spawn(transform.position, randomBubbleSO);
            bubble.Spawner = this;
            bubble.OnBurstEvent += OnBubbleBurst;
            bubble.OnDropEvent += OnBubbleDrop;
            m_Nodes.Add(new SpawnNode(bubble, -1));
        }
    }

    private void OnBubbleBurst(Bubble bubble)
    {
        bubble.OnBurstEvent -= OnBubbleBurst;

        var removeIndex = -1;
        for (var i = 0; i < m_Nodes.Count; ++i)
        {
            if (m_Nodes[i].Bubble == bubble)
            {
                removeIndex = i;
                break;
            }
        }
        if (removeIndex != -1)
        {
            m_Nodes.RemoveAt(removeIndex);
            ++m_SpawnCount;
        }

    }

    private void OnBubbleDrop(Bubble bubble)
    {
        bubble.OnBurstEvent -= OnBubbleDrop;
        var removeIndex = -1;
        for (var i = 0; i < m_Nodes.Count; ++i)
        {
            if (m_Nodes[i].Bubble == bubble)
            {
                removeIndex = i;
                break;
            }
        }

        if (removeIndex != -1)
        {
            m_Nodes.RemoveAt(removeIndex);
            ++m_SpawnCount;
        }
    }
}