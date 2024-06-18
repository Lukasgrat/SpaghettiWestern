using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMouseSensitivity : MonoBehaviour
{
    public Slider mouseSensitivity;
    // Start is called before the first frame update
    void Start()
    {
        mouseSensitivity.value = PlayerPrefs.GetInt("mouseSensitivity", 100);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetInt("mouseSensitivity", (int)mouseSensitivity.value);
    }
}
