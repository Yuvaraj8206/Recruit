using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float totalTime = 600.0f; // Total time in seconds (10 minutes)
    private float timeRemaining;
    private bool isTimerRunning = false;
    public DisplayQuestion displayQuestion;
    [SerializeField]
    public TextMeshProUGUI timerText;
    public GameObject submitPanel, verbalPanel, speakingPanel, audioPanel, headerText, timerPanel, totalqnsPanel;

    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        ResetTimer();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerText();
            }
            else
            {
                isTimerRunning = false;
                timeRemaining = 0;
                UpdateTimerText();
                Debug.Log("Timer finished!");
                displayQuestion.ShowResult();
                submitPanel.SetActive(true);
                verbalPanel.SetActive(false);
                speakingPanel.SetActive(false);
                audioPanel.SetActive(false);
                headerText.SetActive(false);
                timerPanel.SetActive(false);
                totalqnsPanel.SetActive(false);
            }
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void ResetTimer()
    {
        timeRemaining = totalTime;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60.0f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60.0f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
