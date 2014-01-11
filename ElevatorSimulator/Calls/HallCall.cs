using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Calls
{
    class HallCall : Call
    {
        private int destination; // null if no Destination Control
        private Direction direction;

        public Direction getDirection()
        {
            return this.direction;
        }

        public override bool hasDirection()
        {
            return true;
        }
    }
}
