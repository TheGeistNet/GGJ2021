using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCR_DimensionSwapObserverBase : MonoBehaviour, SCR_ISwapObserver
{
    public bool m_StartSwapped = false;
    public abstract void DoSwap(eSwapType type);

    //DO NOT OVERRIDE IN BASE CLASS WITHOUT CALLING PARENT FUNCTIONALITY
    protected virtual void Awake() 
    {
        SCR_DimensionSwapManager.Instance.RegisterSwapCallback(this.DoSwap);
    }

    //DO NOT OVERRIDE IN BASE CLASS WITHOUT CALLING PARENT FUNCTIONALITY
    // MAKE SURE EVERYTHING NEEDED FOR A SWAP IS DONE IN AWAKE OR BEFORE CALLING base.Start()
    protected virtual void Start()
    {
        if (m_StartSwapped)
        {
            DoSwap(eSwapType.SWAP_TYPE_CURRENT);
        }
    }

    //DO NOT OVERRIDE IN BASE CLASS WITHOUT CALLING PARENT FUNCTIONALITY
    protected virtual void OnDisable()
    {
        SCR_DimensionSwapManager.Instance.RegisterSwapCallback(this.DoSwap);
    }
}
