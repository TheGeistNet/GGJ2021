using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCR_SwapObserverBase : MonoBehaviour, INT_SwapObserver
{
    public abstract void DoSwap(eSwapType type);

    //DO NOT OVERRIDE IN BASE CLASS WITHOUT CALLING PARENT FUNCTIONALITY
    protected virtual void Awake() 
    {
        SCR_DimensionSwapManager.Instance.RegisterSwapCallback(this.DoSwap);
    }

    //DO NOT OVERRIDE IN BASE CLASS WITHOUT CALLING PARENT FUNCTIONALITY
    protected virtual void OnDisable()
    {
        SCR_DimensionSwapManager.Instance.RegisterSwapCallback(this.DoSwap);
    }
}
