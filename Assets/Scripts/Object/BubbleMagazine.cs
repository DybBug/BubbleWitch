using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BubbleMagazine : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private int m_Count;
    [SerializeField] private List<BubbleSO> m_BubbleSOs;
    #endregion

    #region Field
    private readonly LinkedList<BubbleSO> m_BubbleSOList = new();
    public event UnityAction<int> OnChangedCountEvent;
    #endregion

    #region Unity
    private void OnDestroy()
    {
        OnChangedCountEvent = null;
    }
    #endregion
    public void Initialize()
    {
        Setup(m_Count, m_BubbleSOs);
    }

    public void Setup(int count, List<BubbleSO> bubbleSOs)
    {
        m_BubbleSOList.Clear();
        for (var i = 0; i < count; ++i)
        {
            var bubbleSO = bubbleSOs[Random.Range(0, bubbleSOs.Count)];
            m_BubbleSOList.AddLast(BubbleTable.Instance.GetItemByKey(bubbleSO.name));
        }
    }

    public int GetRemainCount()
    {
        return m_BubbleSOList.Count;
    }

    public void AddToFront(BubbleSO bubbleSO)
    {
        m_BubbleSOList.AddFirst(bubbleSO);
    }

    public void AddToFrontWithNotify(BubbleSO bubbleSO)
    {
        AddToFront(bubbleSO);
        OnChangedCountEvent?.Invoke(GetRemainCount());
    }

    public BubbleSO GetFromFrontOrNull()
    {
        var bubbleSO = m_BubbleSOList.First?.Value ?? null;
        if (bubbleSO != null)
        {
            m_BubbleSOList.RemoveFirst();
        }
        return bubbleSO;
    }

    public BubbleSO GetFromFrontOrNullWithNotify()
    {
        var bubbleSO = GetFromFrontOrNull();
        if (bubbleSO != null)
        {
            OnChangedCountEvent?.Invoke(GetRemainCount());
        }
        return bubbleSO;
    }

    public BubbleSO Peek(int index)
    {
        if (index >= m_BubbleSOList.Count)
            return null;

        var node = m_BubbleSOList.First;
        for (int i = 1; i <= index; ++i)
        {
            node = node.Next;
        }
        return node.Value;
    }

    public IReadOnlyList<BubbleSO> Peek(int startIndex, int endIndex)
    {
        var result = new List<BubbleSO>();
        if (startIndex >= m_BubbleSOList.Count)
            return result;

        var startNode = m_BubbleSOList.First;
        for (int i = 1; i <= startIndex; ++i)
        {
            startNode = startNode.Next;
        }

        var node = startNode;
        result.Add(startNode.Value);
        for (int i = startIndex + 1; i <= endIndex; ++i)
        {
            node = node.Next;
            if (node == null)
                break;

            result.Add(node.Value);
        }

        return result;
    }
}
