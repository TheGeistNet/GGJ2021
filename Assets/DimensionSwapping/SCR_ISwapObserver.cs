public enum eSwapType
{
    SWAP_TYPE_GRAVITY,
}

public delegate void OnSwapCallbackDelegate(eSwapType type);

public interface SCR_ISwapObserver
{
    void DoSwap(eSwapType type);
}
