using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator.Scheduler
{
    interface IScheduler
    {
        void allocateCall(PassengerGroup group, Building building);
    }
}
