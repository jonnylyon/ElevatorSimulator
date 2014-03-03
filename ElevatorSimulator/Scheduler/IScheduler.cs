using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator.Scheduler
{
    interface IScheduler
    {
        /// <summary>
        /// Allocates a given group of passengers to a specific car within
        /// the given building, based on the relevant allocation method
        /// </summary>
        /// <param name="group">The PassengerGroup to be allocated</param>
        /// <param name="building">The Building holding the domain model</param>
        void AllocateCall(PassengerGroup group, Building building);
    }
}
