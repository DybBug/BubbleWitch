using System;
using System.Collections.Generic;
using UnityEngine;


[Flags]
public enum EReversePosition
{
    X = 1 << 0,
    Y = 1 << 1,
}

[CreateAssetMenu(fileName = "SpawnPatternSO_", menuName = "ScriptableObjects/SpawnPatternSO", order = 2)]
public class SpawnPatternSO : ScriptableObject
{
    public List<Vector2> Positions;
    public List<BubbleSO> BubbleSOs;
    public EReversePosition ReversePosition;

}