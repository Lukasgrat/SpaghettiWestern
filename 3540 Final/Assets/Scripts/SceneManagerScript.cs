using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public GameObject eventHandler;
    public void LoadScene(string sceneName)
    {
        eventHandler.SetActive(false);
        SceneManager.LoadScene(sceneName);
        
    }

    public void LoadScene(int sceneIndex)
    {
        eventHandler.SetActive(false);
        SceneManager.LoadScene(sceneIndex);
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}

