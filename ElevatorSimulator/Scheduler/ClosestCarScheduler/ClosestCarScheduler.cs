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
        public void allocateCall(PhysicalDomain.PassengerGroup group, PhysicalDomain.Building building)
        {
            // compile list of all cars (can we do this in one linq expression?)
            List<Car> cars = new List<Car>();
            building.Shafts.ForEach(s => s.Cars.ForEach(c => cars.Add(c)));

            // get closest car
            var car = cars.OrderBy(c => Math.Abs(c.State.Floor - group.Origin)).First();

            // allocate call
            car.allocateHallCall(new HallCall(group));

            group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
        }

        public void reallocateCall(PhysicalDomain.PassengerGroup group, PhysicalDomain.Building building, PhysicalDomain.Car rejectedFrom)
        {
            throw new NotImplementedException();
        }
    }
}
