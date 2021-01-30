using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_MainMenuButtons : MonoBehaviour
{
    Canvas thisCanvas;
    AudioSource thisAudioSource;
    public SCR_AudioOptionManager audioOptionsScript;
    public SCR_ControlsRemapping controlsScript;
    // Start is called before the first frame update
    void Start()
    {
        thisCanvas = GetComponent<Canvas>();
        thisAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButton()
    {
        thisAudioSource.Play();
        SceneManager.LoadScene(1);
    }
    
    public void ControlsButton()
    {
        thisCanvas.enabled = false;
        controlsScript.CanvasEnable();
        thisAudioSource.Play();
    }

    public void AudioOptionsButton()
    {
        thisCanvas.enabled = false;

        audioOptionsScript.CanvasEnable();
        thisAudioSource.Play();
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
