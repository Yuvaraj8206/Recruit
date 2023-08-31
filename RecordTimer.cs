using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RecordTimer : MonoBehaviour
{
    public TMP_Text timerText;
    private float startTime;
    private bool timerIsRunning = false;

    void Start()
    {
        // set the timer text to an initial value
        timerText.text = "00:00";
    }

    void Update()
    {
        if (timerIsRunning)
        {
            // calculate the time elapsed since the timer started
            float timeElapsed = Time.time - startTime;

            // format the time as a string in the format HH:MM:SS
            string minutes = ((int)timeElapsed / 60).ToString("00");
            string seconds = (timeElapsed % 60).ToString("00");
            /*  string milliseconds = ((int)(timeElapsed * 1000f) % 1000).ToString("000");*/
            string timerFormatted = string.Format("{0}:{1}", minutes, seconds);

            // update the timer text
            timerText.text = timerFormatted;
        }
    }

    public void StartTimer()
    {
        // set the timer start time and start the timer
        startTime = Time.time;
        timerIsRunning = true;

        // make the timer text visible
        timerText.gameObject.SetActive(true);
    }

    public void EndTimer()
    {
        // stop the timer and hide the timer text
        timerIsRunning = false;
        timerText.gameObject.SetActive(false);
    }
}




