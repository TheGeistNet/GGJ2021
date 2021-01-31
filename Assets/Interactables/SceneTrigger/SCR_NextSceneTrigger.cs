using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_NextSceneTrigger : MonoBehaviour
{
    [SerializeField] string nextSceneName = "";

    private bool m_WaitingForSceneTransition = false;

    // Update is called once per frame
    void Update()
    {
        if(m_WaitingForSceneTransition)
        {
            if(SCR_ScoreboardManager.Instance.ScoreboardFinished())
            {
                SCR_LoadData.sceneToLoad = nextSceneName;
                SceneManager.LoadSceneAsync("SCN_LoadingScreen");
                m_WaitingForSceneTransition = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SCR_PlayerController>())
        {
            if (nextSceneName != "")
            {

                if(SCR_ScoreboardManager.Instance.CanShowScoreboard())
                {
                    m_WaitingForSceneTransition = true;
                    return;
                }
                SCR_LoadData.sceneToLoad = nextSceneName;
                SceneManager.LoadSceneAsync("SCN_LoadingScreen");
            }
        }
    }
}
