using UnityEngine.SceneManagement;

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

    private const int NUM_USES_DEFAULT = 3;
    private int m_NumUses;

    private event OnSwapCallbackDelegate OnSwap;

    SCR_DimensionSwapManager()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        ForceReset();
    }

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
        //if(m_NumUses > 0)
        //{
        //    --m_NumUses;
            OnSwap?.Invoke(type);
        //}
    }

    public void OnSceneUnloaded(Scene current)
    {
        ForceReset();
    }

    public void ForceReset()
    {
        if (OnSwap != null)
        {
            foreach (OnSwapCallbackDelegate del in OnSwap?.GetInvocationList())
            {
                OnSwap -= del;
            }
        }
        m_NumUses = NUM_USES_DEFAULT;
    }

    public void StartLevel(int uses)
    {
        m_NumUses = uses;
    }
}
