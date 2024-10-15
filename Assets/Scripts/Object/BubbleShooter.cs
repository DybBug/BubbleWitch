using System.Collections.Generic;
using UnityEngine;

public class BubbleShooter : Spawner
{

    [SerializeField] private Vector3 m_CollisionObjet;
    #region SerializeField
    [SerializeField] private GameObject m_Muzzle;
    [SerializeField] private GameObject m_Arrow;
    [SerializeField] private GameObject m_SnapView;

    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] private float m_FirePower = 20.0f;

    [SerializeField] private BubbleMagazine m_Magazine;
    #endregion


    #region Property
    public Vector2 MuzzleDirection => m_Muzzle.transform.up;
    public Vector2 MuzzlePosition => m_Muzzle.transform.position;
    #endregion

    #region Field
    private const float LIMIT_ANGLE_DEGREES = 85.0f;
    private List<Vector2> m_Path = new();
    private bool m_IsFireable = true;
    #endregion



    #region Unity
    private void Awake()
    {
        BubbleSystemState.Register(this);
    }
    // Start is called before the first frame update
    void Start()
    { 
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_IsFireable)
        {
            UpdateRotation();
            UpdatePath();
            DrawLine();


            if (Input.GetKeyDown(KeyCode.A))
            {
                Fire();
            }
        }
    }
    #endregion 

    public void Fire()
    {
        var bubbleSO = m_Magazine.GetFromFrontOrNullWithNotify();
        if (bubbleSO == null)
            return;

        var bubble = BubbleSystem.Instance.Spawn(MuzzlePosition, bubbleSO);
        bubble.Move(m_Path, m_FirePower, m_SnapView.transform.position);
        bubble.Spawner = this;

        Deactivate();
    }

    public void Activate()
    {
        m_IsFireable = true;
        m_LineRenderer.enabled = true;
        m_SnapView.SetActive(true);
    }

    public void Deactivate()
    {
        m_IsFireable = false;
        m_LineRenderer.enabled = false;
        m_SnapView.SetActive(false);
    }
    private void UpdateRotation()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        var dirToMouseWorldPos = (mouseWorldPos - transform.position).normalized;
        RotateArrow(dirToMouseWorldPos);
    }

    private void RotateArrow(Vector2 dir)
    {
        var forward = new Vector2(0.0f, 1.0f);
        float dot = Vector2.Dot(forward, dir);

        float forwardLength = forward.magnitude;
        float dirLength = dir.magnitude;

        float angleRadians = Mathf.Acos(dot / (forwardLength * dirLength));

        float angleDegrees = angleRadians * Mathf.Rad2Deg;
        if (angleDegrees > LIMIT_ANGLE_DEGREES)
            return;

        float cross = forward.x * dir.y - forward.y * dir.x;
        float sign = cross < 0.0f ? -1 : 1;
        m_Arrow.transform.rotation = Quaternion.Euler(0.0f, 0.0f, sign * angleDegrees);
    }

    private void UpdatePath()
    {
        m_Path.Clear();
        GameObject prevObject = null;
        SearchPath(MuzzlePosition, MuzzleDirection, ref m_Path, prevObject);
    }

    private void SearchPath(Vector2 position, Vector2 direction, ref List<Vector2> path, GameObject prevObject)
    {
        m_startCapsulePos = position;
        m_endCapsulePos = position + (direction * 50);
        var diffVector = (m_endCapsulePos - m_startCapsulePos);

        m_capsuleDirection = diffVector.normalized;
        m_capsuleSize = new Vector2(Bubble.Radius, Bubble.Radius);
        m_castDistance = diffVector.magnitude;

        var hits = Physics2D.CapsuleCastAll(
                m_startCapsulePos,
                m_capsuleSize,
                CapsuleDirection2D.Vertical,
                Helper.CalculateAngle(diffVector.normalized),
                m_capsuleDirection,
                m_castDistance,
                LayerMask.Bubble | LayerMask.Wall);


        if (hits.Length > 0)
        {
            GameObject nearestObject = null;
            Vector2 nearestHitPoint = Vector2.zero;
            var nearestDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                if (prevObject == hit.collider.gameObject)
                    continue;

                var distance = (hit.point - m_startCapsulePos).sqrMagnitude;
                if (distance < nearestDistance)
                {
                    nearestObject = hit.collider.gameObject;
                    nearestHitPoint = hit.point;
                    nearestDistance = distance;
                }
            }

            prevObject = nearestObject;

            if (nearestObject != null)
            {
                if (nearestObject.CompareTag(Tag.Wall))
                {
                    var wallNormal = (Vector2)nearestObject.transform.right;

                    var isTopWall = wallNormal.y < 0.0f;
                    if (isTopWall)
                    {
                        path.Add(nearestHitPoint);
                    }
                    else
                    {
                        Vector2 pos = nearestHitPoint;
                        pos.x = nearestHitPoint.x + (wallNormal.x * m_capsuleSize.x);
                        pos.y = nearestHitPoint.y;

                        path.Add(pos);
                        var newDirection = Helper.CalculateReflection(wallNormal, direction);
                        SearchPath(pos, newDirection, ref path, prevObject);
                    }
                    return;
                }

                if (nearestObject.CompareTag(Tag.Bubble))
                {
                    m_CollisionObjet = nearestObject.transform.position;
                    var objectPos = new Vector2(nearestObject.transform.position.x, nearestObject.transform.position.y);

                    var toHitPointDir = (nearestHitPoint - objectPos).normalized;

                    objectPos = SnapPosition(nearestHitPoint, objectPos);

                    m_SnapView.transform.position = objectPos;
                    path.Add(nearestHitPoint);
                    return;
                }
            }
        }
    }

    private void DrawLine()
    {
        if (m_Path.Count <= 0)
            return;

        m_LineRenderer.positionCount = m_Path.Count + 1;
        m_LineRenderer.SetPosition(0, MuzzlePosition);
        for (int i = 0; i < m_Path.Count; ++i)
        {
            m_LineRenderer.SetPosition(i + 1, m_Path[i]);
        }
    }


    private Vector2 m_capsuleSize;
    private Vector2 m_capsuleDirection;
    private float m_castDistance;
    private Vector2 m_startCapsulePos;
    private Vector2 m_endCapsulePos;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 start = new Vector3(m_startCapsulePos.x, m_startCapsulePos.y, 0);
        Vector3 end = new Vector3(m_endCapsulePos.x, m_endCapsulePos.y, 0);

        // 캡슐의 시작점과 끝점을 시각적으로 표시
        Gizmos.DrawWireSphere(m_startCapsulePos, m_capsuleSize.x / 2);
        Gizmos.DrawWireSphere(end, m_capsuleSize.x / 2);
        Gizmos.DrawLine(start + Vector3.up * m_capsuleSize.x / 2, end + Vector3.up * m_capsuleSize.x / 2);
        Gizmos.DrawLine(start - Vector3.up * m_capsuleSize.x / 2, end - Vector3.up * m_capsuleSize.x / 2);
    }

    private Vector2 SnapPosition(Vector2 hitPoint, Vector2 bubblePosition)
    {
        var toHitPoint = (hitPoint - bubblePosition);

        if (Mathf.Abs(toHitPoint.x) > Mathf.Abs(toHitPoint.y))
        {
            var newPosition = SnapX(toHitPoint, bubblePosition);
            return newPosition;
        }
        else
        {
            var newPosition = SnapY(toHitPoint, bubblePosition);
            return newPosition;
        }
    }

    private Vector2 SnapX(Vector2 toHitPoint, Vector2 bubblePosition)
    {
        var newPosition = bubblePosition;
        if (toHitPoint.x > 0)
        {
            newPosition = bubblePosition + new Vector2(+2.0f * Bubble.Radius, 0);
            if (BubbleSystem.Instance.GetBubbleOrNull(newPosition) != null)
            {
                if (toHitPoint.y < 0)
                {
                    newPosition = bubblePosition + new Vector2(+Bubble.Radius, -2.0f * Bubble.Radius);
                }
                else
                {
                    newPosition = bubblePosition + new Vector2(+Bubble.Radius, +2.0f * Bubble.Radius);
                }
            }
        }
        else
        {
            newPosition = bubblePosition + new Vector2(-2.0f * Bubble.Radius, 0);
            if (BubbleSystem.Instance.GetBubbleOrNull(newPosition) != null)
            {
                if (toHitPoint.y < 0)
                {
                    newPosition = bubblePosition + new Vector2(-Bubble.Radius, -2.0f * Bubble.Radius);
                }
                else
                {
                    newPosition = bubblePosition + new Vector2(-Bubble.Radius, +2.0f * Bubble.Radius);
                }
            }
        }
        return newPosition;
    }
    private Vector2 SnapY(Vector2 toHitPoint, Vector2 bubblePosition)
    {
        var newPosition = bubblePosition;
        if(toHitPoint.y > 0)
        {
            if (toHitPoint.x > 0)
            {
                newPosition = bubblePosition + new Vector2(+Bubble.Radius, +2.0f * Bubble.Radius);
                if (BubbleSystem.Instance.GetBubbleOrNull(newPosition) != null)
                {
                    newPosition = bubblePosition + new Vector2(+2.0f * Bubble.Radius, 0);
                }
            }
            else if (toHitPoint.x < 0)
            {
                newPosition = bubblePosition + new Vector2(-Bubble.Radius, +2.0f * Bubble.Radius);
                if (BubbleSystem.Instance.GetBubbleOrNull(newPosition) != null)
                {
                    newPosition = bubblePosition + new Vector2(-2.0f * Bubble.Radius, 0);
                }
            }
        }
        else
        {
            if (toHitPoint.x > 0)
            {
                newPosition = bubblePosition + new Vector2(+Bubble.Radius, -2.0f * Bubble.Radius);
                if (BubbleSystem.Instance.GetBubbleOrNull(newPosition) != null)
                {
                    newPosition = bubblePosition + new Vector2(+2.0f * Bubble.Radius, 0);
                }
            }
            else if (toHitPoint.x < 0)
            {
                newPosition = bubblePosition + new Vector2(-Bubble.Radius, -2.0f * Bubble.Radius);
                if (BubbleSystem.Instance.GetBubbleOrNull(newPosition) != null)
                {
                    newPosition = bubblePosition + new Vector2(-2.0f * Bubble.Radius, 0);
                }
            }
        }
        return newPosition;
    }
}