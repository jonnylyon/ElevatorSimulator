using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Scheduler.ClosestCarScheduler
{
    class ClosestCarScheduler : IScheduler
    {
        public void allocateCall(PassengerGroup group, Building building)
        {
            // compile list of all cars (can we do this in one linq expression?)
            List<ICar> cars = new List<ICar>();
            building.Shafts.ForEach(s => s.Cars.ForEach(c => cars.Add(c)));

            // get closest car
            var car = cars.OrderBy(c => Math.Abs(c.State.Floor - group.Origin)).First();

            // allocate call
            car.allocateHallCall(new HallCall(group));

            group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
        }

        public void reallocateCall(PassengerGroup group, Building building, ICar rejectedFrom)
        {
            throw new NotImplementedException();
        }
    }
}
