using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Agenda
{
    abstract class AgendaEvent
    {
        public DateTime TimeOccurred { get; private set; }

        private IEventOwner owner;

        protected AgendaEvent(IEventOwner owner, DateTime time)
        {
            this.owner = owner;
            this.TimeOccurred = time;
        }
    }
}
