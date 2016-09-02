using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Security.Permissions;
using System;
using JetBrains.Annotations;

public class HotPotatoTimer : MonoBehaviour
{
    public bool debugging = true;
    // The default maximum time a user has to decide which next hop to take, otherwise a random hop will be performed.
    public float standardHopTime = 5.0f;
    // The time the user has to decide which next hop to take after a new run has started.
    public float hopTimeOnNewRun = 20.0f;
    //Standard interval for updating the gui timer.
    public float standardIntervalGUITimer = 1.0f;

    // Keeps the selected GUI update interval for the current timer run.
    private float intervalGUITimer = 1.0f;

    // Event to notify about failure to complete the action in the given time.
    UnityEvent e_timeUp;
    // Event to notify about GUI Changes to the timer
    UnityEvent e_updateTimerGUI;

    #region Properties

    // Indicates the current time of the timer run.
    public float CurrentTime
    {
        get;
        set;
    }

    #endregion Properties

    // Use this for initialization
    void Awake()
    {
        if (debugging)
            Debug.Log("Awake the timer class.");

        if (e_timeUp == null)
            e_timeUp = new UnityEvent();
        if (e_updateTimerGUI == null)
            e_updateTimerGUI = new UnityEvent();
        
    }

    /// <summary>
    /// Calls a new timer with the given threshold time in seconds. After the time has passed the
    /// threshold time, the timer will fire the timeUp Event and thus call the specified
    /// callback function.
    /// </summary>
    /// <param name="callbackFunc">The callback function that is called if the timer passes the threshold time.</param>
    /// <param name="thresholdTime">The threshold time in seconds.</param>
    public void CallNewTimer(UnityAction callbackFunc, float thresholdTime)
    {
        if (e_timeUp != null)
        {
            e_timeUp.AddListener(callbackFunc);
        }

        // Check whether provided threshold time value.
        if (thresholdTime <= 0)
            thresholdTime = this.standardHopTime;   // Take standard value if invalid threshold time. 

        if (debugging)
        {
            Debug.Log("Event e_timeUp: " + e_timeUp + " | timerMaximum: " + thresholdTime);
        }

        CurrentTime = thresholdTime;
        Invoke("FireTimeUpEvent", thresholdTime);

    }

    /// <summary>
    /// Cancels the current timer run.
    /// </summary>
    /// <param name="callbackFunc">Callback func.</param>
    public void CancelTimerRun(UnityAction callbackFunc)
    {
        if (e_timeUp != null)
            e_timeUp.RemoveListener(callbackFunc);

        CancelInvoke("FireTimeUpEvent");
    }

    /// <summary>
    /// Subscribes to GUI timer updates with the given interval in seconds. The Timer will fire
    /// GUI updates using the specified callback function at the specified interval.
    /// </summary>
    /// <param name="callbackFunc">Callback function that will be called at the specified interval.</param>
    /// <param name="intervalGUITimer">The interval value in seconds.</param>
    public void SubscribeToGuiTimer(UnityAction callbackFunc, float intervalGUITimer)
    {
        if (e_updateTimerGUI != null)
        {
            e_updateTimerGUI.AddListener(callbackFunc);
        }

        // Check provided interval value.
        if (intervalGUITimer <= 0)
            intervalGUITimer = standardIntervalGUITimer;        // Take default value if invalid interval value is provided.

        this.intervalGUITimer = intervalGUITimer;

        InvokeRepeating("FireUpdateTimerGUI", intervalGUITimer, intervalGUITimer);
    }

    /// <summary>
    /// Unsubscribes from GUI timer updates.
    /// </summary>
    /// <param name="callbackFunc">Callback func.</param>
    public void UnsubscribeFromGuiTimer(UnityAction callbackFunc)
    {
        if (e_updateTimerGUI != null)
            e_updateTimerGUI.RemoveListener(callbackFunc);

        CancelInvoke("FireUpdateTimerGUI");
    }

    /// <summary>
    /// Fires the time up event.
    /// </summary>
    void FireTimeUpEvent()
    {
        if (debugging)
        {
            Debug.Log("Firing time up event. " + DateTime.Now.ToString("HH:mm:ss:fff"));
        }
        e_timeUp.Invoke();
    }

    /// <summary>
    /// Fires the updatetimerGUIEvent.
    /// </summary>
    void FireUpdateTimerGUI()
    {
        CurrentTime -= this.intervalGUITimer;

        if (debugging)
        {
            Debug.Log("Firing updatetimerGUIEvent. CurrentTime: " + CurrentTime);
        }

        e_updateTimerGUI.Invoke();
    }

    /// <summary>
    /// Pauses the timer.
    /// </summary>
    public void PauseTimer()
    {
        PauseTimeUpEvent();
        PauseUpdateTimerGUI();
    }

    /// <summary>
    /// Resumes the timer.
    /// </summary>
    public void ResumeTimer()
    {
        ResumeTimeUpEvent();
        ResumeUpdateTimerGUI();
    }

    void PauseUpdateTimerGUI()
    {
        if (debugging)
        {
            Debug.Log("Pausing updatetimerGUIEvent. CurrentTime: " + CurrentTime);
        }
        CancelInvoke("FireUpdateTimerGUI");
    }

    void PauseTimeUpEvent()
    {
        if (debugging)
        {
            Debug.Log("Pausing updateTimeUpEvent. CurrentTime: " + CurrentTime);
        }
        CancelInvoke("FireTimeUpEvent");
    }

    void ResumeUpdateTimerGUI()
    {
        if (debugging)
        {
            Debug.Log("Resuming updatetimerGUIEvent. CurrentTime: " + CurrentTime);
        }
        InvokeRepeating("FireUpdateTimerGUI", standardIntervalGUITimer, standardIntervalGUITimer);
    }

    void ResumeTimeUpEvent()
    {
        if (debugging)
        {
            Debug.Log("Resuming updateTimeUpEvent. CurrentTime: " + CurrentTime);
        }
        Invoke("FireTimeUpEvent", CurrentTime);
    }
}
