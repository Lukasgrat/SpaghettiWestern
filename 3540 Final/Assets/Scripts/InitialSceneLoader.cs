using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialSceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        if (PlayerPrefs.GetInt("Bypass", 0) == 1)
        {
            PlayerPrefs.SetInt("Bypass", 0);
            SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.SetInt("Bypass", 1);
            Debug.Log("Loading scene");
            SceneManager.LoadScene("World");
        }
    }
    void Bypass() 
    {
        
    }
}