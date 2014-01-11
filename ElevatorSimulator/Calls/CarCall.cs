using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Calls
{
    class CarCall : Call
    {
        public override bool hasDirection()
        {
            return false;
        }
    }
}
