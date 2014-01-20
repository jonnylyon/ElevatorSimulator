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

        public override int getFloor()
        {
            return this.passengerGroup.getDestination();
        }

        public override bool hasDirection()
        {
            return false;
        }

        public override Direction getDirection()
        {
            return Direction.None;
        }
    }
}
