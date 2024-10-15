using UnityEngine;

public class Map : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private GameObject m_TopLine;
    [SerializeField] private GameObject m_BottomLine;
    [SerializeField] private GameObject m_Grid;
    [SerializeField] private GameObject m_BeginPoint;
    #endregion

    #region Property
    public Vector2 BottomLinePosition => m_BottomLine.transform.position;
    #endregion

    public void Start()
    {
        
    }


    public bool CheckTopLineOverlap(Bubble bubble)
    {
        return bubble.Position.y >= m_TopLine.transform.position.y;
    }

    public void AddBubble(Bubble bubble)
    {
        if (bubble.transform.parent == m_BeginPoint.transform)
            return;

        bubble.transform.parent = m_BeginPoint.transform;
        var pos = bubble.transform.localPosition;
        pos.z = 0;
        bubble.transform.localPosition = pos;
    }

    public void ShiftGridUp(uint shiftCount = 1)
    {
        var gridPos = m_Grid.transform.position;
        gridPos.y += 2.0f * Bubble.Radius * shiftCount;
        m_Grid.transform.position = gridPos;
    }

    public void ShiftGridDown(uint shiftCount = 1)
    {
        var gridPos = m_Grid.transform.position;
        gridPos.y -= 2.0f *Bubble.Radius * shiftCount;
        m_Grid.transform.position = gridPos;
    }

    public void ResetGridPosition()
    {
        m_Grid.transform.position = Vector3.zero;
    }

}
