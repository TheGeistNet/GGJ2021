using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_HUDButtons : MonoBehaviour
{
    Canvas thisCanvas;
    public Canvas inGameMenuCanvas;

    // Start is called before the first frame update
    void Start()
    {
        thisCanvas = GetComponent<Canvas>();
        inGameMenuCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenInGameMenu()
    {
        inGameMenuCanvas.enabled = true;
        thisCanvas.enabled = false;
    }
}
