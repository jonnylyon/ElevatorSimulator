using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Scheduler.TCOSAlternateSingleShaft
{
    class TCOSAlternateSingleShaft : IScheduler
    {
        private ICar lastAllocation;

        public void AllocateCall(PassengerGroup group, Building building)
        {
            var shaft0Cars = building.Shafts[0].Cars;

            var nextCar = shaft0Cars.Where(c => !object.ReferenceEquals(c, lastAllocation)).First();

            if (nextCar.allocateHallCall(new HallCall(group)))
            {
                lastAllocation = nextCar;

                group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
            }
            else if (lastAllocation.allocateHallCall(new HallCall(group)))
            {
                group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
            }
            else
            {
                Simulation.logger.logLine("NB: Call has failed allocation");
            }
        }
    }
}
