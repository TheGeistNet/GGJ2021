using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SCR_AudioOptionManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public Canvas mainMenuCanvas;
    public Slider mainVolumeSlider;
    public Slider playerVolumeSlider;
    public Slider backgroundVolumeSlider;
    public Slider musicVolumeSlider;

    Canvas thisCanvas;
    AudioSource thisAudioSource;
    

    // Start is called before the first frame update
    void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        CanvasDisable();
        thisAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeMainVolume()
    {
        mainMixer.SetFloat("MainVolume", Mathf.Log(mainVolumeSlider.value) * 20);
    }
    public void ChangePlayerVolume()
    {
        mainMixer.SetFloat("PlayerVolume", Mathf.Log(playerVolumeSlider.value) * 20);
    }
    public void ChangeBackgroundVolume()
    {
        mainMixer.SetFloat("BackgroundVolume", Mathf.Log(backgroundVolumeSlider.value) * 20);
    }
    public void ChangeMusicVolume()
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log(musicVolumeSlider.value) * 20);
    }

    public void BackButton()
    {
        CanvasDisable();
        if (mainMenuCanvas)
        {
            mainMenuCanvas.enabled = true;
            thisAudioSource.Play();
        }
    }

    public void CanvasDisable()
    {
        PlayerPrefs.SetFloat("MainVolume", mainVolumeSlider.value);
        PlayerPrefs.SetFloat("PlayerVolume", playerVolumeSlider.value);
        PlayerPrefs.SetFloat("BackgroundVolume", backgroundVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        thisCanvas.enabled = false;
    }

    public void CanvasEnable()
    {
        mainVolumeSlider.value = PlayerPrefs.GetFloat("MainVolume");
        playerVolumeSlider.value = PlayerPrefs.GetFloat("PlayerVolume");
        backgroundVolumeSlider.value = PlayerPrefs.GetFloat("BackgroundVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        thisCanvas.enabled = true;
    }
}
