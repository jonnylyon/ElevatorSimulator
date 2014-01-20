using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Exceptions
{
    class SimulationAssumptionException : Exception
    {
        public SimulationAssumptionException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
