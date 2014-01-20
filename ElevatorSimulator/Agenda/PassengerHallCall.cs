using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator.Agenda
{
    class PassengerHallCall : Event
    {
        public PassengerHallCall(PassengerGroup owner, DateTime time)
            :base(owner, time)
        {
            // do nothing
        }
    }
}
