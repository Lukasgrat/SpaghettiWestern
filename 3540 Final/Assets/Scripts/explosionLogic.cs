using Cinemachine;
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
    public GameObject runText;
    float sliderTime;
    float curTime;
    bool finishedArming = false;
    public CinemachineDollyCart locomotive;
    public CinemachineDollyCart car;
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
            curTime = Mathf.Min(curTime + Time.deltaTime, sliderTime);
        }
        else
        {
            curTime = Math.Max(curTime - Time.deltaTime / 2, 0);
        }
        explosionSlider.value = curTime;
        if (sliderTime == curTime) 
        {
            explosives.SetActive(true);
            explosionUI.SetActive(false);
            finishedArming = true;
            car.enabled = true;
            locomotive.enabled = true;
            runText.SetActive(true);
        }
    }
}
