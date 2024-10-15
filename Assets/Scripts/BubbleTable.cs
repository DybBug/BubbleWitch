using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BubbleTable : MonoBehaviour
{
    #region Instance
    private static BubbleTable _instance;
    public static BubbleTable Instance
    {
        get
        {
            if (_instance == null)
            {
                var gameObject = new GameObject("BubbleTable");
                _instance = gameObject.AddComponent<BubbleTable>();
            }
            return _instance;
        }
    }
    #endregion

    #region SerializeField
    [SerializeField] private List<BubbleSO> m_BubbleSOs;
    #endregion

    #region Field
    private readonly Dictionary<string/*key*/, BubbleSO> m_Items = new();
    #endregion

    #region Unity
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            if (m_BubbleSOs != null)
            {
                GenerateItems();
            }
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void GenerateItems()
    {
        foreach (var so in m_BubbleSOs)
        {
            m_Items.Add(so.name, so);
        }
    }

    public List<string> GetKeys()
    {
        return m_Items.Keys.ToList();
    }

    public BubbleSO GetItemByKey(string key)
    {
        var isValid = m_Items.TryGetValue(key, out var value);
        Debug.Assert(isValid);

        return value;
    }
    #endregion
}