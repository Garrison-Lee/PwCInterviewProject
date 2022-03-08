using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script controlling and providing methods for controlling the scoreboard.
/// </summary>
public class Scoreboard : MonoBehaviour
{
    //References to the dynamic text fields
    private Text score;
    private Text time;

    //For the timer
    private bool timerRunning = false;
    private float timeRemaining = 0f;

    //Audio for the start and end of the time trial
    private SimpleAudioEmitter audioEmitter;

    private void Awake()
    {
        score = transform.Find("Score_DynamicText").GetComponent<Text>();
        time = transform.Find("Time_DynamicText").GetComponent<Text>();

        audioEmitter = GetComponent<SimpleAudioEmitter>();
    }

    private void Update()
    {
        //If the timer is started and time remaining, handle the logic for updating that
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;

            SetTimer((int)timeRemaining);

            if (timeRemaining <= 0f) //Timer just ran out, we can stop timer and play buzzer noise
            {
                audioEmitter.PlayNoise("End");
                timerRunning = false;
            }
        }
    }

    /// <summary>
    /// Helper method for adding points to the score. Primarily called by targets.
    /// </summary>
    public void AddScore(int points)
    {
        int curScore = int.Parse(score.text);
        curScore += points;
        score.text = curScore.ToString();
    }

    /// <summary>
    /// Resets the score back to 0
    /// </summary>
    public void ResetScore()
    {
        score.text = "0";
    }

    /// <summary>
    /// Starts the timer
    /// </summary>
    public void StartTimer(float seconds)
    {
        timeRemaining = seconds;
        timerRunning = true;

        audioEmitter.PlayNoise("Start");
    }

    /// <summary>
    /// This method takes a number of seconds as input and formats it into a time string and puts it on the board.
    /// </summary>
    public void SetTimer(int seconds)
    {
        int minutes = 0;

        while (seconds >= 60)
        {
            minutes++;
            seconds -= 60;
        }

        string minString = minutes > 9 ? minutes.ToString() : "0" + minutes.ToString();
        string secString = seconds > 9 ? seconds.ToString() : "0" + minutes.ToString();

        time.text = minString + ":" + secString;
    }
}
