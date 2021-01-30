using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SCR_PlayerUI : MonoBehaviour
{
    SCR_HUDButtons hudButtons;
    SCR_HUDSwapCounter hudSwapCounter;
    bool runMenuSwitch = true;
    PlayerInput player;
    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {
        hudButtons = FindObjectOfType<SCR_HUDButtons>();
        hudSwapCounter = hudButtons.gameObject.GetComponent<SCR_HUDSwapCounter>();
        player = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (context.started && runMenuSwitch)
        {
            hudButtons.OpenInGameMenu();
            runMenuSwitch = false;
            player.enabled = false;
        }
    }
    public void ReturnToGame()
    {
        runMenuSwitch = true;
        player.enabled = true;
    }

    public void AddToSwapCounter(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            hudSwapCounter.AddToCounter();
        }
    }
}
