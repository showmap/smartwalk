using System;

namespace SmartWalk.Client.iOS.Utils
{
    public class AnimationDelay
    {
        private static readonly TimeSpan _delay = TimeSpan.FromSeconds(0.5); 
        private DateTime _resetTime;

        public bool Animate
        {
            get
            {
                var result = DateTime.Now - _resetTime > _delay;
                return result;
            }
        }

        public void Reset()
        {
            _resetTime = DateTime.Now;
        }
    }
}