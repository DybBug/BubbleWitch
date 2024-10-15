using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BubbleMagazineView : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private BubbleMagazine m_BubbleMagazine;

    [SerializeField] private Button m_SwapButton;
    [SerializeField] private TMP_Text m_RemainCountText;
    [SerializeField] private List<Image> m_BubbleImages;
    #endregion

    #region Unity
    private void Awake()
    {
        m_BubbleMagazine.OnChangedCountEvent += OnChangedCount;
        m_SwapButton.onClick.AddListener(OnClickedSwapButton);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_BubbleMagazine.Initialize();
        SetCountText(m_BubbleMagazine.GetRemainCount());
        ModifyBubbleImages();

    }
    #endregion

    private void ModifyBubbleImages()
    {
        var bubbleSOs = m_BubbleMagazine.Peek(0, 1);
        for (var i = 0; i < m_BubbleImages.Count; ++i)
        {
            if (i < bubbleSOs.Count)
            {
                m_BubbleImages[i].gameObject.SetActive(true);
                m_BubbleImages[i].sprite = bubbleSOs[i].BubbleSprite;
                m_BubbleImages[i].color = bubbleSOs[i].BubbleColor;
            }
            else
            {
                m_BubbleImages[i].gameObject.SetActive(false);
            }
        }
    }

    private void SetCountText(int count)
    {
        m_RemainCountText.text = $"{count}";
    }

    private void OnChangedCount(int remainCount)
    {
        SetCountText(remainCount);
        ModifyBubbleImages();
    }

    private void OnClickedSwapButton()
    {
        if (m_BubbleMagazine.GetRemainCount() <= 0)
            return;

        var first = m_BubbleMagazine.GetFromFrontOrNull();
        var second = m_BubbleMagazine.GetFromFrontOrNull();

        m_BubbleMagazine.AddToFront(first);
        m_BubbleMagazine.AddToFront(second);

        ModifyBubbleImages();
    }
}
