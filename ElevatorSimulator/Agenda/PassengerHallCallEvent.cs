using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator.Agenda
{
    class PassengerHallCallEvent : AgendaEvent
    {
        public PassengerHallCallEvent(PassengerGroup owner, DateTime time)
            :base(owner, time)
        {
            // TODO
        }
    }
}
