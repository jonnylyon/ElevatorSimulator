using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Tools;

namespace ElevatorSimulator.Scheduler.TCOSMinimalOverlap
{
    class TCOSMinimalOverlap : IScheduler
    {
        private TCOSETAETD.TCOSETAETD etaCalculator;

        public TCOSMinimalOverlap()
        {
            this.etaCalculator = new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.WaitingTime, false, 1, false);
        }

        public void AllocateCall(PassengerGroup group, Building building)
        {
            // compile list of all cars (can we do this in one linq expression?)
            List<TCOSCar> cars = new List<TCOSCar>();
            building.Shafts.ForEach(s => s.Cars.ForEach(c => cars.Add((TCOSCar)c)));

            var carPreference = cars.OrderBy(c => CalculateOverlapExtension(c, group)).ThenBy(c => etaCalculator.CalculateCost(c, group)).ToList();
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

        private int CalculateOverlapExtension(TCOSCar car, PassengerGroup group)
        {
            TCOSCar otherCar = (TCOSCar)car.shaft.Cars.Where(c => !object.ReferenceEquals(c, car)).First();

            var otherCarCalls = new ExpandedAllocationList(otherCar.CallAllocationList);
            var carCallsOriginal = new ExpandedAllocationList(car.CallAllocationList);
            var carCallsModified = new ExpandedAllocationList(car.CallAllocationList);

            Pass pass = Tools.CalculatePassOfCall(car, group);

            carCallsModified.AddGroupCallsToPass(group, pass);

            var otherCarRange = CalculateRange(otherCarCalls, otherCar);
            var carRangeOriginal = CalculateRange(carCallsOriginal, car);
            var carRangeModified = CalculateRange(carCallsModified, car);

            var overlapRangeOriginal = otherCarRange.Intersect(carRangeOriginal);
            var overlapRangeModified = otherCarRange.Intersect(carRangeModified);

            return overlapRangeModified.Count() - overlapRangeOriginal.Count();
        }

        private List<int> CalculateRange(ExpandedAllocationList calls, TCOSCar car)
        {
            var min = GeneralTools.min(calls.LowestCallLocation, car.State.Floor);
            min = GeneralTools.min(min, car.ParkFloor);

            var max = GeneralTools.max(calls.HighestCallLocation, car.State.Floor);
            max = GeneralTools.max(max, car.ParkFloor);

            return GeneralTools.getRange(min, max);
        }
    }
}
