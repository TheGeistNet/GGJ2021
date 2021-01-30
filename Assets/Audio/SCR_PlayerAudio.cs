using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SCR_PlayerAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource deathAudioSource;
    public AudioSource jumpAudioSource;
    public AudioSource landAudioSource;
    public AudioSource leftFootAudioSource;
    public AudioSource rightFootAudioSource;
    public AudioSource swapAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AudioDeath()
    {

    }

    public void AudioJump()
    {
        jumpAudioSource.Play();
    }

    public void AudioLand()
    {
        landAudioSource.Play();
    }

    public void LeftFoot()
    {
        leftFootAudioSource.Play();
    }

    public void RightFoot()
    {
        rightFootAudioSource.Play();
    }

    public void Swap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            swapAudioSource.Play();
        }
    }
}
