using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_InGameMenuButtons : MonoBehaviour
{
    Canvas thisCanvas;
    AudioSource thisAudioSource;
    public Canvas hudCanvas;
    public SCR_ControlsRemapping controlsScript;
    public SCR_AudioOptionManager audioOptionsScript;
    SCR_PlayerUI player;

    // Start is called before the first frame update
    void Start()
    {
        thisCanvas = GetComponent<Canvas>();
        thisAudioSource = GetComponent<AudioSource>();
        player = FindObjectOfType<SCR_PlayerUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResumeButton()
    {
        thisCanvas.enabled = false;
        hudCanvas.enabled = true;
        thisAudioSource.Play();
        player.ReturnToGame();
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
