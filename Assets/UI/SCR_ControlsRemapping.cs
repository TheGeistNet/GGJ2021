using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SCR_ControlsRemapping : MonoBehaviour
{
    Canvas thisCanvas;
    AudioSource thisAudioSource;
    public Canvas mainMenuCanvas;
    public GameObject keyboardRemaps;
    public GameObject gamepadRemaps;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisAudioSource = GetComponent<AudioSource>();
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
        if (Gamepad.current != null)
        {
            keyboardRemaps.SetActive(false);
            gamepadRemaps.SetActive(true);
        } else
        {
            keyboardRemaps.SetActive(true);
            gamepadRemaps.SetActive(false);
        }
    }

    public void CanvasDisable()
    {
        thisCanvas.enabled = false;
        if (mainMenuCanvas)
        {
            mainMenuCanvas.enabled = true;
            thisAudioSource.Play();
        }
    }
}
