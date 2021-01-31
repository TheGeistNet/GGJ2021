using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="SO_LevelData", menuName ="ScriptableObjects/LevelCompletionData")]
public class SCR_ScoreboardEntry : ScriptableObject
{
    public int index;
    public int usedSwaps;
    public int maxSwaps;
    public int amountCollected;
    public int maxCollected;
    [SerializeField]
    public string[] rankingStrings;
    [SerializeField]
    public int[] rankingValues;
}
