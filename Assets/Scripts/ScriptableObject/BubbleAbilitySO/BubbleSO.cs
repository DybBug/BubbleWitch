using System;
using UnityEngine;

[Flags]
public enum EMatchFlag
{
    None = 0,
    Red = 1 << 0,
    Green = 1 << 1,
    Blue = 1 << 2,
}

[CreateAssetMenu(fileName = "BubbleSO_", menuName = "ScriptableObjects/BubbleSO", order = 3)]
public class BubbleSO : ScriptableObject
{
    public EMatchFlag MatchFlag;
    public GameObject BubblePrefab;
    public BubbleAbilitySO BubbleAbilitySO;

    public Sprite BubbleSprite;
    public Color BubbleColor;

    public Sprite InnerSprite;
    public Color InnerColor;

}