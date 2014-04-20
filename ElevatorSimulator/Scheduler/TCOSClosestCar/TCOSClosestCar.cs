using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Scheduler.TCOSClosestCar
{
    class TCOSClosestCar : IScheduler
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

            var carPreference = cars.OrderBy(c => Math.Abs(c.State.Floor - group.Origin)).ToList();
            bool allocated = false;

            while (!allocated && carPreference.Any())
            {
                var car = carPreference.First();

                allocated = car.allocateHallCall(new HallCall(group));

                carPreference.Remove(car);
            }

            if (allocated)
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
