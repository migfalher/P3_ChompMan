using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    // components
    private TMP_Text timeCounterTMP;
    // variables
    private float timeCounter;
    private string timeCounterText;
    private bool timeCounterIsOn;

    // getters and setters
    public void setTimeCounterTMP(TMP_Text _timeCounter) { timeCounterTMP = _timeCounter; }
    public bool getTimeCounterIsOn() { return timeCounterIsOn; }
    public void setTimeCounterIsOn(bool _timeCounterIsOn) { timeCounterIsOn = _timeCounterIsOn; }
    public string getTimeCounterText() { return timeCounterText; }
    public void resetTimeCounter() { timeCounter = 0; }

    // Awake
    private void Awake()
    {
        timeCounter = 0;
        timeCounterIsOn = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        if (timeCounterIsOn)
        {
            timeCounter += Time.deltaTime;
            timeCounterText = TimeSpan.FromSeconds(timeCounter).ToString("hh':'mm':'ss");
            timeCounterTMP.text = timeCounterText;
        }
    }

}
