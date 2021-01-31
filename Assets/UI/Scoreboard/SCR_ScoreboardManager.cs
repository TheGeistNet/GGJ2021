using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SCR_ScoreboardManager : MonoBehaviour
{
    public static SCR_ScoreboardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject managerObject = GameObject.Find("ManagerObject");
                if (managerObject == null)
                {
                    managerObject = new GameObject();
                    managerObject.name = "ManagerObject";
                    managerObject.AddComponent<SCR_ScoreboardManager>();
                    DontDestroyOnLoad(managerObject);
                }
                _instance = managerObject.GetComponent<SCR_ScoreboardManager>();
            }
            return _instance;
        }
    }
    private static SCR_ScoreboardManager _instance;

    public List<SCR_ScoreboardEntry> m_Entries;

    private SCR_ScoreboardHUD m_ScoreboardHUD;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;

        Initialize();
    }

    void Initialize()
    {
        m_Entries = new List<SCR_ScoreboardEntry>();
        m_ScoreboardHUD = GameObject.FindObjectOfType<SCR_ScoreboardHUD>();
    }

    public bool CanShowScoreboard()
    {
        SCR_ScoreboardInLevelIdentifier identifier = GameObject.FindObjectOfType<SCR_ScoreboardInLevelIdentifier>();
        if (identifier != null)
        {
            SCR_HUDSwapCounter hud = GameObject.FindObjectOfType<SCR_HUDSwapCounter>();
            if (hud != null)
            {
                SCR_ScoreboardEntry entry = identifier.entry;
                entry.usedSwaps = hud.GetSwapCount();
                entry.amountCollected = hud.GetCollectibleCount();
                PopulateScoreboard(entry);
                return true;
            }
        }
        return false;
    }

    public bool ScoreboardFinished()
    {
        if(m_ScoreboardHUD == null)
        {
            return true;
        }
        return m_ScoreboardHUD.Finished();
    }

    void PopulateScoreboard(SCR_ScoreboardEntry entry)
    {
        if (m_ScoreboardHUD == null)
        {
            m_ScoreboardHUD = GameObject.FindObjectOfType<SCR_ScoreboardHUD>();
        }
        if (m_ScoreboardHUD != null)
        {
            m_ScoreboardHUD.gameObject.SetActive(true);
            m_ScoreboardHUD.StartScoreboard(entry);
        }
    }



}
