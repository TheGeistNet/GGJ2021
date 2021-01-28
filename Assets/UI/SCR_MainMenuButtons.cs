using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_MainMenuButtons : MonoBehaviour
{
    Canvas thisCanvas;
    public SCR_AudioOptionManager audioOptionsScript;
    public SCR_ControlsRemapping controlsScript;
    // Start is called before the first frame update
    void Start()
    {
        thisCanvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButton()
    {
        SceneManager.LoadScene(1);
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
