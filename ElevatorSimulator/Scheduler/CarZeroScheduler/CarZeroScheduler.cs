using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Scheduler.CarZeroScheduler
{
    class CarZeroScheduler : IScheduler
    {
        public void allocateCall(PassengerGroup group, Building building)
        {
            building.Shafts[0].Cars[0].allocateHallCall(new HallCall(group));

            group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
        }

        public void reallocateCall(PassengerGroup group, Building building, ICar rejectedFrom)
        {
            this.allocateCall(group, building);
        }
    }
}
