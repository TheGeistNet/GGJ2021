using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_NextSceneTrigger : MonoBehaviour
{
    [SerializeField] string nextSceneName = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SCR_PlayerController>())
        {
            if (nextSceneName != "")
            {
                SCR_LoadData.sceneToLoad = nextSceneName;
                SceneManager.LoadSceneAsync("SCN_LoadingScreen");
            }
        }
    }
}
