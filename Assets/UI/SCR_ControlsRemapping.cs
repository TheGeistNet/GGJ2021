using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_ControlsRemapping : MonoBehaviour
{
    Canvas thisCanvas;
    public Canvas mainMenuCanvas;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
    }
    // Start is called before the first frame update
    void Start()
    {
        thisCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CanvasEnable()
    {
        thisCanvas.enabled = true;
        if (mainMenuCanvas)
        {
            mainMenuCanvas.enabled = false;
        }
    }

    public void CanvasDisable()
    {
        thisCanvas.enabled = false;
        if (mainMenuCanvas)
        {
            mainMenuCanvas.enabled = true;
        }
    }
}
