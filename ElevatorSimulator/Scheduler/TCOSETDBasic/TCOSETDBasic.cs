using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.Tools;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.DataStructures;

namespace ElevatorSimulator.Scheduler.TCOSETDBasic
{
    class TCOSETDBasic : IScheduler
    {
        private double StopTimeSeconds = 5;
        private double StartTimeSeconds = 5;
        private double ReverseTimeSeconds = 1;
        private double UnloadPersonTimeSeconds = 2;
        private double LoadPersonTimeSeconds = 2;
        private double FloorTravelTimeSeconds = 1;

        public void AllocateCall(PassengerGroup group, Building building)
        {
            // compile list of all cars (can we do this in one linq expression?)
            List<TCOSCar> cars = new List<TCOSCar>();
            building.Shafts.ForEach(s => s.Cars.ForEach(c => cars.Add((TCOSCar)c)));

            var carPreference = cars.OrderBy(c => CalculateCost(c, group)).ToList();
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

        private double CalculateCost(TCOSCar car, PassengerGroup group)
        {
            var originalCalls = new ExpandedAllocationList(car.CallAllocationList);
            var modifiedCalls = new ExpandedAllocationList(car.CallAllocationList);

            Pass pass = Tools.CalculatePassOfCall(car, group);

            modifiedCalls.AddGroupCallsToPass(group, pass);

            double costExcludingAllocation = CalculateSystemCost(originalCalls, car);
            double costIncludingAllocation = CalculateSystemCost(modifiedCalls, car, group);

            return costIncludingAllocation - costExcludingAllocation;
        }

        private double CalculateSystemCost(ExpandedAllocationList calls, TCOSCar car, PassengerGroup groupUnderAllocation = null)
        {
            var orderedCalls = calls.GetOrderedListOfAllCalls(car.State.Direction);

            int currentFloor = car.State.Floor;
            Direction currentDirection = car.State.Direction;

            double currentTime = 0;
            double systemCost = 0;
            double groupCost = 0;

            while (orderedCalls.Any())
            {
                Call call = orderedCalls.First();

                if (call.CallLocation == currentFloor
                    && (call.CallDirection == currentDirection || call.CallDirection == Direction.None))
                {
                    // can serve call
                    if (call is HallCall)
                    {
                        currentTime += (LoadPersonTimeSeconds * call.Passengers.Size);
                    }
                    if (call is CarCall)
                    {
                        if (!object.ReferenceEquals(call.Passengers, groupUnderAllocation))
                        {
                            systemCost += currentTime;
                        }
                        else
                        {
                            groupCost += currentTime;
                        }

                        currentTime += (UnloadPersonTimeSeconds * call.Passengers.Size);
                    }

                    orderedCalls.Remove(call);
                }
                else if (call.CallLocation == currentFloor)
                {
                    // can serve call at this floor but must reverse
                    currentDirection = currentDirection == Direction.Down ? Direction.Up : Direction.Down;
                    currentTime += ReverseTimeSeconds;
                }
                else if (GeneralTools.getDirectionFromHereToThere(currentFloor, call.CallLocation) != currentDirection)
                {
                    // reverse in order to move towards call
                    currentDirection = currentDirection == Direction.Down ? Direction.Up : Direction.Down;
                    currentTime += ReverseTimeSeconds;
                }
                else
                {
                    // don't need to reverse, move to call floor
                    currentTime += StartTimeSeconds;
                    currentTime += Math.Abs(currentFloor - call.CallLocation) * FloorTravelTimeSeconds;
                    currentTime += StopTimeSeconds;
                    currentFloor = call.CallLocation;
                }
            }

            return systemCost + groupCost;
        }
    }
}
