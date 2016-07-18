using System;
using System.Timers;

namespace Pentathanerd.When
{
    public class TimerExtension : Timer
    {
        private DateTime _endTime;

        public double SecondsLeft
        {
            get { return (_endTime - DateTime.Now).TotalSeconds; }
        }

        public TimerExtension()
        {
            Elapsed += OnElapsed;
        }

        public TimerExtension(double interval) : this()
        {
            Interval = interval;
        }

        private void OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (AutoReset)
            {
                _endTime = DateTime.Now.AddMilliseconds(Interval);
            }
        }
        public new void Dispose()
        {
            Elapsed -= OnElapsed;
            base.Dispose();
        }

        public new void Start()
        {
            _endTime = DateTime.Now.AddMilliseconds(Interval);
            base.Start();
        }
    }
}