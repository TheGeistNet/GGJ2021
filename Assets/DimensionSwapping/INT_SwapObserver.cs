public enum eSwapType
{
    SWAP_TYPE_GRAVITY,
}

public delegate void OnSwapCallbackDelegate(eSwapType type);

public interface INT_SwapObserver
{
    void DoSwap(eSwapType type);
}
