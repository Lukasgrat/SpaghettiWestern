using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core;
using Unity.VisualScripting;
using UnityEngine;

public class CalculateTimeSpent : MonoBehaviour
{
    DateTime startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelDone()
    {
        TimeSpan timePlayed = DateTime.Now.Subtract(startTime);
        int totalSecondsPlayed = (int)timePlayed.TotalSeconds;

    int totalDays = totalSecondsPlayed / (3600 * 24);
    int remainingSeconds = totalSecondsPlayed % (3600 * 24);

    int totalHours = remainingSeconds / 3600;
    remainingSeconds %= 3600;

    int totalMins = remainingSeconds / 60;
    int totalSecs = remainingSeconds % 60;

    // Update PlayerPrefs with total values
    PlayerPrefs.SetInt("DaysPlayed", totalDays + PlayerPrefs.GetInt("DaysPlayed", 0));
    PlayerPrefs.SetInt("HoursPlayed", totalHours + PlayerPrefs.GetInt("HoursPlayed", 0));
    PlayerPrefs.SetInt("MinsPlayed", totalMins + PlayerPrefs.GetInt("MinsPlayed", 0));
    PlayerPrefs.SetInt("SecsPlayed", totalSecs + PlayerPrefs.GetInt("SecsPlayed", 0));
    }
}
