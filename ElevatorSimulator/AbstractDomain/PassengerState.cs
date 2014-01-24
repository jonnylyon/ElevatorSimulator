using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.AbstractDomain
{
    /// <summary>
    /// Possible passenger states
    /// </summary>
    enum PassengerState
    {
        Unborn,
        Waiting,
        InTransit,
        Arrived
    }
}
