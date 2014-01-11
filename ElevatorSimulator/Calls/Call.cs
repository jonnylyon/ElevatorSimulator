using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
