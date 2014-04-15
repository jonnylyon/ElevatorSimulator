using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.DataStructures;
using ElevatorSimulator.Tools;

namespace ElevatorSimulator.Scheduler.TCOSETABasic
{
    class TCOSETABasic : IScheduler
    {
        private class ExpandedAllocationList
        {
            public CallsList P1Calls = new CallsList();
            public CallsList P2Calls = new CallsList();
            public CallsList P3Calls = new CallsList();

            public CallsList GetPassCallsList(Pass pass)
            {
                switch (pass)
                {
                    case Pass.P1:
                        return P1Calls;
                    case Pass.P2:
                        return P2Calls;
                    case Pass.P3:
                        return P3Calls;
                    default:
                        return null;
                }
            }

            public void AddGroupCallsToPass(PassengerGroup group, Pass pass)
            {
                var passList = GetPassCallsList(pass);

                passList.Add(new HallCall(group));
                passList.Add(new CarCall(group));
            }

            public List<Call> GetOrderedListOfAllCalls(Direction currentDirection)
            {
                int orderCoefficient = currentDirection == Direction.Up ? 1 : -1;
                var resultList = new List<Call>();

                foreach (Call c in P1Calls.OrderBy(a => a.CallLocation * orderCoefficient))
                {
                    resultList.Add(c);
                }

                foreach (Call c in P2Calls.OrderBy(a => -1 * a.CallLocation * orderCoefficient))
                {
                    resultList.Add(c);
                }

                foreach (Call c in P3Calls.OrderBy(a => a.CallLocation * orderCoefficient))
                {
                    resultList.Add(c);
                }

                return resultList;
            }

            public ExpandedAllocationList(CallAllocationList original)
            {
                foreach (Call c in original.P1List)
                {
                    P1Calls.Add(c);
                    if (c is HallCall)
                    {
                        P1Calls.Add(new CarCall(c.Passengers));
                    }
                }

                foreach (Call c in original.P2List)
                {
                    P2Calls.Add(c);
                    if (c is HallCall)
                    {
                        P2Calls.Add(new CarCall(c.Passengers));
                    }
                }

                foreach (Call c in original.P3List)
                {
                    P3Calls.Add(c);
                    if (c is HallCall)
                    {
                        P3Calls.Add(new CarCall(c.Passengers));
                    }
                }
            }
        }

        private enum Pass
        {
            P1,
            P2,
            P3
        }

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
            
            Pass pass = CalculatePassOfCall(car, group);

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
                        if (object.ReferenceEquals(call.Passengers, groupUnderAllocation))
                        {
                            systemCost += currentTime;
                        }
                        else
                        {
                            groupCost += currentTime;
                        }
                        
                        currentTime += (LoadPersonTimeSeconds * call.Passengers.Size);
                    }
                    if (call is CarCall)
                    {
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
                }
            }

            return systemCost + groupCost;
        }

        private int? GetFinalDestinationOfPass(TCOSCar car, ExpandedAllocationList calls, Pass pass)
        {
            if (car.State.Direction == Direction.Down)
            {
                if (pass == Pass.P1)
                {
                    return GeneralTools.min(calls.P1Calls.getLowestCallLocation(), calls.P2Calls.getLowestCallLocation());
                }
                if (pass == Pass.P2)
                {
                    return GeneralTools.min(calls.P2Calls.getHighestCallLocation(), calls.P3Calls.getHighestCallLocation());
                }
                return calls.P3Calls.getLowestCallLocation();
            }

            if (pass == Pass.P1)
            {
                return GeneralTools.min(calls.P1Calls.getHighestCallLocation(), calls.P2Calls.getHighestCallLocation());
            }
            if (pass == Pass.P2)
            {
                return GeneralTools.min(calls.P2Calls.getLowestCallLocation(), calls.P3Calls.getLowestCallLocation());
            }
            return calls.P3Calls.getHighestCallLocation();
        }

        private Pass CalculatePassOfCall(TCOSCar car, PassengerGroup group)
        {
            if (group.Direction != car.State.Direction)
            {
                return Pass.P2;
            }

            if (group.Direction == Direction.Down && group.Origin < car.State.Floor)
            {
                return Pass.P1;
            }

            if (group.Direction == Direction.Up && group.Origin > car.State.Floor)
            {
                return Pass.P1;
            }

            if (group.Origin == car.State.Floor && car.State.Action != CarAction.Leaving && car.State.Action != CarAction.Moving)
            {
                return Pass.P1;
            }

            return Pass.P3;
        }
    }
}
