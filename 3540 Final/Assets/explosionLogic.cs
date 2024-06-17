using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class explosionLogic : MonoBehaviour
{
    public GameObject explosives;
    public GameObject explosionUI;
    public Slider explosionSlider;
    float sliderTime;
    float curTime;
    bool finishedArming = false;
    // Start is called before the first frame update
    void Start()
    {
        sliderTime = explosionSlider.maxValue;
        curTime = explosionSlider.minValue;
        explosionUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (finishedArming) return;
        explosionUI.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        explosionSlider.value = 0;
        curTime = 0;
        explosionUI.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.E))
        {
            curTime = Mathf.Max(curTime + Time.deltaTime, sliderTime);
        }
        else
        {
            curTime -= Math.Min(curTime - Time.deltaTime / 2, 0);
        }
        if (sliderTime == curTime) 
        {
            explosives.SetActive(true);
            explosionUI.SetActive(false);
            finishedArming = true;
        }
    }
}
