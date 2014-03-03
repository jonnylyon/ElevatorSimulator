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
        /// <summary>
        /// Allocates a given group of passengers to a specific car within
        /// the given building, based on the ClosestCar allocation method
        /// </summary>
        /// <param name="group">The PassengerGroup to be allocated</param>
        /// <param name="building">The Building holding the domain model</param>
        public void AllocateCall(PassengerGroup group, Building building)
        {
            // compile list of all cars
            List<ICar> cars = new List<ICar>();
            building.Shafts.ForEach(s => s.Cars.ForEach(c => cars.Add(c)));

            // get closest car
            var car = cars.OrderBy(c => Math.Abs(c.State.Floor - group.Origin)).First();

            // allocate call
            car.allocateHallCall(new HallCall(group));

            // update the PassengerGroup data with the time at which
            // the hall call was made
            group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
        }
    }
}
