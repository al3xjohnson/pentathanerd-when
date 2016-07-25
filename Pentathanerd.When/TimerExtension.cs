using System;
using System.Timers;

namespace Pentathanerd.When
{
    internal class TimerExtension : Timer
    {
        private DateTime _endTime;
        private DateTime _stopTime;

        public double SecondsLeft
        {
            get { return (_endTime - DateTime.Now).TotalSeconds; }
        }

        public double IntervalInSeconds
        {
            get { return Interval / 1000; }
        }

        public double TimeRemainingWhenStopped
        {
            get { return (_endTime - _stopTime).TotalSeconds; }
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
            _stopTime = DateTime.Now;
            base.Start();
        }

        public new void Stop()
        {
            _stopTime = DateTime.Now;
            base.Stop();
        }
    }
}