using UnityEngine;
using UnityEngine.Events;

namespace Networking
{
    public class CountdownTimer : MonoBehaviour
    {
        [Tooltip("The number of minutes to count down from.")] [SerializeField]
        private int minutes;

        [Tooltip("The number of seconds to count down from.")] [SerializeField] [Range(0, 59)]
        private int seconds;

        private float _currentTime;

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
        private bool IsPaused { get; set; } = true;

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
            IsPaused = true;
            _currentTime = Mathf.Ceil(_currentTime);
        }

        /// <summary>
        ///     Stops the timer.
        /// </summary>
        public void Resume()
        {
            IsPaused = false;
        }

        /// <summary>
        ///     Tick the timer.
        /// </summary>
        /// <param name="deltaTime">The time in seconds since the last tick.</param>
        public void Tick(float deltaTime)
        {
            if (IsPaused) return;

            _currentTime -= deltaTime;

            if (!(_currentTime <= 0f)) return;

            _currentTime = 0f;
            IsPaused = true;
            OnTimerExpired?.Invoke();
        }
    }
}