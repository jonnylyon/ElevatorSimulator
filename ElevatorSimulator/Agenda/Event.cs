using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Agenda
{
    abstract class Event
    {
        private IEventOwner owner;
        private DateTime time;

        protected Event(IEventOwner owner, DateTime time)
        {
            this.owner = owner;
            this.time = time;
        }

        public DateTime getTime()
        {
            return time;
        }
    }
}
