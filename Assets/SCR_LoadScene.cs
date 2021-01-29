using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_LoadScene : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadSceneAsync(SCR_LoadData.sceneToLoad);
    }
}
