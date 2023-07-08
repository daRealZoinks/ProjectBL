using System;
using UnityEngine;
using UnityEngine.Events;

public class CountdownTimer : MonoBehaviour
{
    [Tooltip("The number of minutes to count down from.")]
    [SerializeField]
    private int minutes;

    [Tooltip("The number of seconds to count down from.")]
    [SerializeField]
    [Range(0, 59)]
    private int seconds;

    /// <summary>
    ///     The number of minutes remaining on the timer.
    /// </summary>
    public int Minutes => Mathf.FloorToInt(_currentTime / 60f);

    /// <summary>
    ///     The number of seconds remaining on the timer.
    /// </summary>
    public int Seconds => Mathf.FloorToInt(_currentTime % 60f);

    /// <summary>
    ///     If the timer is paused.
    /// </summary>
    public bool IsPaused => _paused;

    private float _currentTime;
    private bool _paused = true;

    private void Start()
    {
        _currentTime = minutes * 60f + seconds;
    }

    /// <summary>
    ///     Invoked when the timer expires.
    /// </summary>
    public event UnityAction OnTimerExpired;

    /// <summary>
    ///     Pauses the timer.
    /// </summary>
    public void Pause()
    {
        _paused = true;
        _currentTime = Mathf.Ceil(_currentTime);
    }

    /// <summary>
    ///     Stops the timer.
    /// </summary>
    public void Resume()
    {
        _paused = false;
    }

    /// <summary>
    ///     Tick the timer.
    /// </summary>
    /// <param name="deltaTime">The time in seconds since the last tick.</param>
    public void Tick(float deltaTime)
    {
        if (_paused) return;

        _currentTime -= deltaTime;

        if (_currentTime <= 0f)
        {
            _currentTime = 0f;
            _paused = true;
            OnTimerExpired?.Invoke();
        }
    }
}
