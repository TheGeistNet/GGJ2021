using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_HUDSwapCounter : MonoBehaviour
{
    public Text counterText;
    public Text collectibleText;
    int counterTally = 0;
    int collectibleTally = 0;

    // Start is called before the first frame update
    private void Start()
    {
        ResetCounts();
        counterText.text = counterTally.ToString();
        collectibleText.text = collectibleTally.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToCounter()
    {
        counterTally = counterTally + 1;
        counterText.text = counterTally.ToString();
    }

    public void AddToCollectible(int points)
    {
        collectibleTally = collectibleTally + points;
        collectibleText.text = collectibleTally.ToString();
    }

    public void ResetCounts()
    {
        counterTally = 0;
        collectibleTally = 0;
    }
}
