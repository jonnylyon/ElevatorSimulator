using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Agenda
{
    abstract class AgendaEvent
    {
        public DateTime TimeOccurred { get; private set; }

        public IEventOwner Owner { get; private set; }

        protected AgendaEvent(IEventOwner owner, DateTime time)
        {
            this.Owner = owner;
            this.TimeOccurred = time;
        }
    }
}
