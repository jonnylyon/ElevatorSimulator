using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Calls
{
    class CarCall : Call
    {
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
