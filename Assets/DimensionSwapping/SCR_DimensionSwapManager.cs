public class SCR_DimensionSwapManager
{
    public static SCR_DimensionSwapManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new SCR_DimensionSwapManager();
            }
            return _instance;
        }
    }
    private static SCR_DimensionSwapManager _instance;

    private event OnSwapCallbackDelegate OnSwap;

    public void RegisterSwapCallback(OnSwapCallbackDelegate callback)
    {
        OnSwap += callback;
    }
    public void DeregisterSwapCallback(OnSwapCallbackDelegate callback)
    {
        OnSwap -= callback;
    }

    public void ForceSwap(eSwapType type)
    {
        OnSwap?.Invoke(type);
    }

    public void ForceReset()
    {
        foreach(OnSwapCallbackDelegate del in OnSwap.GetInvocationList())
        {
            OnSwap -= del;
        }
    }
}
