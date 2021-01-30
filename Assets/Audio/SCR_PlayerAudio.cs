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
    public AudioSource moveAudioSource;
    public AudioSource swapAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Death()
    {

    }

    public void Jump()
    {

    }

    public void Land()
    {

    }

    public void Move()
    {
    }

    public void Swap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            swapAudioSource.Play();
        }
    }
}
