using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator.Calls
{
    class CarCall : Call
    {
        public CarCall(PassengerGroup passengerGroup)
            :base(passengerGroup)
        {
        }

        public override Direction CallDirection
        {
            get 
            {
                return Direction.None;
            }
        }

        public override int CallLocation
        {
            get
            {
                return Passengers.Destination;
            }
        }
    }
}
