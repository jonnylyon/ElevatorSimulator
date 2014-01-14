using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Calls
{
    abstract class Call
    {
        protected int floor;

        public int getFloor()
        {
            return this.floor;
        }

        public abstract bool hasDirection();

        public abstract Direction getDirection();
    }
}
