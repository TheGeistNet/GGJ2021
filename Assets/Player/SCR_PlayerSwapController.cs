using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SCR_PlayerSwapController : SCR_DimensionSwapObserverBase
{
    private SCR_PlayerController m_PlayerController;

    protected override void Awake()
    {
        base.Awake();
        m_PlayerController = GetComponent<SCR_PlayerController>();
    }

    public override void DoSwap(eSwapType type)
    {
        if(type == eSwapType.SWAP_TYPE_GRAVITY)
        {
            m_PlayerController.InvertGravity();
        }
    }

    public void OnSwapDimensionInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SCR_DimensionSwapManager.Instance.ForceSwap(eSwapType.SWAP_TYPE_GRAVITY);
        }
    }
}
