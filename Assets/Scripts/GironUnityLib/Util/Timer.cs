/**
* Timer.cs written by Andreas Mikko, 2016
*/

namespace Util
{
    public class Timer
    {
        private float time;
        private bool tick;

        /// <summary>
        /// The current time.
        /// </summary>
        public float Time {
            get { return time; }
        }

        /// <summary>
        /// Check if the timer is ticking.
        /// </summary>
        public bool IsTicking {
            get { return tick; }
        }

        public Timer()
        {
            time = 0.0f;
            tick = false;
        }

        /// <summary>
        /// Update the timer with one tick of delta time
        /// </summary>
        /// <param name="deltaTime">The delta time</param>
        public void Tick(float deltaTime)
        {
            if (tick)
            {
                time += deltaTime;
            }
        }

        /// <summary>
        /// Start the timer.
        /// </summary>
        public void Start()
        {
            tick = true;
        }

        /// <summary>
        /// Stop the timer. Time will reset to zero.
        /// </summary>
        public void Stop()
        {
            tick = false;
            time = 0.0f;
        }

        /// <summary>
        /// Pause the timer
        /// </summary>
        public void Pause()
        {
            tick = false;
        }

        /// <summary>
        /// Reset the time to zero.
        /// </summary>
        public void Reset()
        {
            time = 0.0f;
        }
    }
}
