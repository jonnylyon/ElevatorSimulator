using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator.Agenda
{
    class PassengerArrival : Event
    {
        public PassengerArrival(PassengerGroup owner, DateTime time)
            :base(owner, time)
        {
            // do nothing
        }
    }
}
