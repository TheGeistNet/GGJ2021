using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_InGameMenuButtons : MonoBehaviour
{
    Canvas thisCanvas;
    public Canvas hudCanvas;
    public SCR_ControlsRemapping controlsScript;
    public SCR_AudioOptionManager audioOptionsScript;

    // Start is called before the first frame update
    void Start()
    {
        thisCanvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResumeButton()
    {
        thisCanvas.enabled = false;
        hudCanvas.enabled = true;
    }

    public void ControlsButton()
    {
        thisCanvas.enabled = false;
        controlsScript.CanvasEnable();
    }

    public void AudioOptionsButton()
    {
        thisCanvas.enabled = false;

        audioOptionsScript.CanvasEnable();
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
