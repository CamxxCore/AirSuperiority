using GTA;

namespace AirSuperiority.Script
{
    public class Timer
    {
        private bool enabled;
        private int interval;
        private int waiter;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public int Waiter
        {
            get { return waiter; }
            set { waiter = value; }
        }

        public Timer(int interval)
        {
            this.interval = interval;
            waiter = 0;
            enabled = false;
        }

        public Timer()
        {
            interval = 0;
            waiter = 0;
            enabled = false;
        }

        public void Start()
        {
            waiter = Game.GameTime + interval;
            enabled = true;
        }

        public void Reset()
        {
            waiter = Game.GameTime + interval;
        }
    }
}
