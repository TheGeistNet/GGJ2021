using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_ScoreboardInLevelIdentifier : MonoBehaviour
{
    public SCR_ScoreboardEntry entry;

    private void Awake()
    {
        entry = Instantiate(entry);
    }
}
