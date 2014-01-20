using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator.Calls
{
    abstract class Call
    {
        protected PassengerGroup passengerGroup;

        public Call(PassengerGroup passengerGroup)
        {
            this.passengerGroup = passengerGroup;
        }

        public abstract int getFloor();

        public abstract bool hasDirection();

        public abstract Direction getDirection();
    }
}
