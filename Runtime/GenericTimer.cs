using System;
using UnityEngine;

public class Timer
{
    public event Action OnTimerComplete; // Event fired when timer reaches 0

    private float _startValue, _startTime;
    private float _currentValue;
    private bool _isRunning = false;

    public void SetStartValue(float startValue)
    {
        _startValue = startValue;
    }

    public void Start()
    {
        if (_isRunning)
        {
            Debug.LogWarning("Timer is already running!");
            
            return;
        }
        
        _currentValue = _startValue;
        _startTime = Time.realtimeSinceStartup;
        _isRunning = true;
    }

    public void Stop()
    {
        _isRunning = false;
    }

    public void Update()
    {
        if (!_isRunning)
        {
            return;
        }

        var elapsedTime = Time.realtimeSinceStartup - _startTime;
        _currentValue = _startValue - elapsedTime;
        
        // Check if timer reached 0 and fire event
        if (_currentValue <= 0)
        {
            _currentValue = 0;
            _isRunning = false;
            OnTimerComplete?.Invoke();
            Debug.Log($"Timer Finished!");
        }
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    public float GetCurrentTime()
    {
        return Math.Max(0, _currentValue);
    }
}