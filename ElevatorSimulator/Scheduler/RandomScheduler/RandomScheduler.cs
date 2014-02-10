using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Scheduler.RandomScheduler
{
    class RandomScheduler : IScheduler
    {
        private Random random = new Random();

        public void allocateCall(PassengerGroup group, Building building)
        {
            // choose shaft at random
            var shaft = building.Shafts[random.Next(building.Shafts.Count)];

            // choose car with shaft at random
            var car = shaft.Cars[random.Next(shaft.Cars.Count)];

            // allocate call
            car.allocateHallCall(new HallCall(group));

            group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
        }
    }
}
