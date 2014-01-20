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

        public override int getFloor()
        {
            return this.passengerGroup.getOrigin();
        }

        public int getDestination()
        {
            return this.passengerGroup.getDestination();
        }

        public override Direction getDirection()
        {
            return this.passengerGroup.getDestination() > this.passengerGroup.getOrigin() ? Direction.Up : Direction.Down;
        }

        public override bool hasDirection()
        {
            return true;
        }
    }
}
