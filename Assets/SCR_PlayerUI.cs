using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SCR_PlayerUI : MonoBehaviour
{
    SCR_HUDButtons hudButtons;
    bool runMenuSwitch = true;
    PlayerInput player;
    public InputActionAsset playerInputSettings;
    // Start is called before the first frame update
    void Start()
    {
        hudButtons = FindObjectOfType<SCR_HUDButtons>();
        player = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (context.performed && runMenuSwitch)
        {
            hudButtons.OpenInGameMenu();
            //player.SwitchCurrentActionMap("UI");
            runMenuSwitch = false;
            print(player.currentActionMap);
        }
    }
    public void ReturnToGame()
    {
        //player.SwitchCurrentActionMap("Gameplay");
        runMenuSwitch = true;
        print(player.currentActionMap);
    }
}
