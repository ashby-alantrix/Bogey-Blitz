using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSystem
{
    private float timeRem;
    private float maxTimeAvail;

    private Action onTimerComplete = null;
    private Action onTimerInProgress = null;
    public bool IsTimerComplete
    {
        get;
        private set;
    }

    public void Init(float maxTimeAvail, Action onComplete = null, Action inProgress = null)
    {
        timeRem = 0;
        IsTimerComplete = false;

        this.maxTimeAvail = maxTimeAvail;
        this.onTimerComplete = onComplete;
        this.onTimerInProgress = inProgress;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (timeRem < maxTimeAvail)
        {
            timeRem += deltaTime;
            onTimerInProgress?.Invoke();
        }
        else
        {
            timeRem = 0;
            onTimerComplete?.Invoke();
            IsTimerComplete = true;
        }
    }
}
