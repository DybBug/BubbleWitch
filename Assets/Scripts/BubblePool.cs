using System.Collections.Generic;
using UnityEngine;

public class BubblePool : MonoBehaviour
{
    #region Instance
    private static BubblePool _instance;
    public static BubblePool Instance
    {
        get
        {
            if (_instance == null)
            {
                var gameObject = new GameObject("BubblePool");
                _instance = gameObject.AddComponent<BubblePool>();
            }
            return _instance;
        }
    }
    #endregion


    #region Field
    private List<Bubble> m_Bubbles = new();
    #endregion

    #region Unity
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    public Bubble SpawnBubble(BubbleSO bubbleSO = null)
    {
        Bubble bubble = null;
        foreach (var bubbleInPool in m_Bubbles)
        {
            if (bubbleInPool.IsSpawnable)
            {
                bubble = bubbleInPool;
                break;
            }
        }

        if (bubble == null)
        {
            bubble = Instantiate(bubbleSO.BubblePrefab).GetComponent<Bubble>();
            m_Bubbles.Add(bubble);
        }

        bubble.Spawn(bubbleSO);
        return bubble;
    }

    public Bubble SpawnBubble(Vector2 position, BubbleSO bubbleSO = null)
    {
        var bubble = SpawnBubble(bubbleSO);
        bubble.transform.position = position;
        return bubble;
    }

}
