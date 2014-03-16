using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Scheduler.TCOSTwoZones
{
    class TCOSTwoZones : IScheduler
    {
        public void AllocateCall(PassengerGroup group, Building building)
        {
            ICar preferredCar;
            ICar otherCar;

            if ((group.Origin + group.Destination) / 2 >= 4.5)
            {
                preferredCar = building.Shafts[0].Cars[1];
                otherCar = building.Shafts[0].Cars[0];
            }
            else
            {
                preferredCar = building.Shafts[0].Cars[0];
                otherCar = building.Shafts[0].Cars[1];
            }

            if (!preferredCar.allocateHallCall(new HallCall(group)))
            {
                otherCar.allocateHallCall(new HallCall(group));
            }

            group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
        }
    }
}
