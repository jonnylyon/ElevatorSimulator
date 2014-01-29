using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator.Calls
{
    class HallCall : Call
    {
        public HallCall(PassengerGroup passengerGroup)
            : base(passengerGroup)
        {
        }

        public override Direction CallDirection
        {
            get 
            {
                return this.Passengers.Direction;
            }
        }

        public override int getElevatorDestination()
        {
            return Passengers.Origin;
        }
    }
}
