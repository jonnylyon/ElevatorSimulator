using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Tools;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Calls;

namespace ElevatorSimulator.Scheduler.TCOSETDAdvanced
{
    class TCOSETDAdvanced : IScheduler
    {
        private int DestinationTimePower;
        private bool EnforcePenalties;

        private double StopTimeSeconds = 5;
        private double StartTimeSeconds = 5;
        private double ReverseTimeSeconds = 1;
        private double UnloadPersonTimeSeconds = 2;
        private double LoadPersonTimeSeconds = 2;
        private double FloorTravelTimeSeconds = 1;
        private double NoCapacityAllocationPenaltySeconds = 1000;

        public TCOSETDAdvanced(int WaitingTimePower, bool EnforcePenalties)
        {
            this.DestinationTimePower = WaitingTimePower;
            this.EnforcePenalties = EnforcePenalties;
        }

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
            TCOSCar otherCar = (TCOSCar)car.shaft.Cars.Where(c => !object.ReferenceEquals(c, car)).First();

            var otherCarCalls = new ExpandedAllocationList(otherCar.CallAllocationList);
            var originalCalls = new ExpandedAllocationList(car.CallAllocationList);
            var modifiedCalls = new ExpandedAllocationList(car.CallAllocationList);

            Pass pass = Tools.CalculatePassOfCall(car, group);

            modifiedCalls.AddGroupCallsToPass(group, pass);

            var otherCarRange = CalculateRange(otherCarCalls, otherCar);
            var carRangeOriginal = CalculateRange(originalCalls, car);
            var carRangeModified = CalculateRange(modifiedCalls, car);

            var originalOverlap = carRangeOriginal.Intersect(otherCarRange).ToList();
            var modifiedOverlap = carRangeModified.Intersect(otherCarRange).ToList();

            var originalCarRunResult = PerformCarRun(originalCalls, car, originalOverlap);
            var modifiedCarRunResult = PerformCarRun(modifiedCalls, car, modifiedOverlap);
            var originalOtherCarRunResult = PerformCarRun(otherCarCalls, otherCar, originalOverlap);
            var modifiedOtherCarRunResult = PerformCarRun(otherCarCalls, otherCar, modifiedOverlap);

            double costExcludingAllocation = ReconcileCarRuns(originalCarRunResult, originalOtherCarRunResult);
            double costIncludingAllocation = ReconcileCarRuns(modifiedCarRunResult, modifiedOtherCarRunResult);
            return costIncludingAllocation - costExcludingAllocation;
        }

        private double ReconcileCarRuns(CarRunResult result1, CarRunResult result2)
        {
            List<CarRunResult> owners = new List<CarRunResult>() { result1, result2 };

            List<OverlapEntryExitMapped> oees = new List<OverlapEntryExitMapped>();

            foreach (var owner in owners)
            {
                foreach (var oee in owner.overlapEntries)
                {
                    oees.Add(new OverlapEntryExitMapped() { overlapEntryExit = oee, owner = owner });
                }
            }

            while (oees.Any())
            {
                var thisOee = oees.OrderBy(o => o.overlapEntryExit.entry).First();

                while (oees.Except(new List<OverlapEntryExitMapped>() { thisOee })
                           .Where(o => o.overlapEntryExit.entry < thisOee.overlapEntryExit.exit)
                           .Any())
                {
                    var conflict = oees.Except(new List<OverlapEntryExitMapped>() { thisOee })
                                       .Where(o => o.overlapEntryExit.entry < thisOee.overlapEntryExit.exit)
                                       .First();

                    double waitTime = (double)thisOee.overlapEntryExit.exit - (double)conflict.overlapEntryExit.entry;
                    double waitStartTime = (double)conflict.overlapEntryExit.entry;

                    conflict.owner.forceWait(waitStartTime, waitTime);
                }

                oees.Remove(thisOee);
            }

            return result1.TotalCost + result2.TotalCost;
        }

        struct OverlapEntryExitMapped
        {
            public CarRunResult.OverlapEntryExit overlapEntryExit;
            public CarRunResult owner;
        }

        class CarRunResult
        {
            public class OverlapEntryExit
            {
                public double? entry, exit;

                public OverlapEntryExit(double? entry = null, double? exit = null)
                {
                    this.entry = entry;
                    this.exit = exit;
                }

                public void forceWait(double startTime, double waitTime)
                {
                    if (entry >= startTime)
                    {
                        entry += waitTime;
                    }

                    if (exit >= startTime)
                    {
                        exit += waitTime;
                    }
                }
            }

            public List<double> systemCosts = new List<double>();
            public List<double> groupCosts = new List<double>();
            public List<double> penaltyCosts = new List<double>();
            public List<OverlapEntryExit> overlapEntries = new List<OverlapEntryExit>();

            private int DestinationTimePower;

            public CarRunResult(int DestinationTimePower)
            {
                this.DestinationTimePower = DestinationTimePower;
            }

            public double TotalCost
            {
                get
                {
                    return systemCosts.Sum(c => Math.Pow(c, DestinationTimePower)) + groupCosts.Sum(c => Math.Pow(c, DestinationTimePower)) + penaltyCosts.Sum(c => Math.Pow(c, DestinationTimePower));
                }
            }

            public void forceWait(double startTime, double waitTime)
            {
                for (int i = 0; i < systemCosts.Count(); i++)
                {
                    if (systemCosts[i] >= startTime)
                    {
                        systemCosts[i] += waitTime;
                    }
                }

                for (int i = 0; i < groupCosts.Count(); i++)
                {
                    if (groupCosts[i] >= startTime)
                    {
                        groupCosts[i] += waitTime;
                    }
                }

                for (int i = 0; i < overlapEntries.Count(); i++)
                {
                    overlapEntries[i].forceWait(startTime, waitTime);
                }
            }
        }

        private CarRunResult PerformCarRun(ExpandedAllocationList calls, TCOSCar car, List<int> overlap, PassengerGroup groupUnderAllocation = null)
        {
            var orderedCalls = calls.GetOrderedListOfAllCalls(car.State.Direction);

            CarRunResult result = new CarRunResult(DestinationTimePower);

            double currentTime = 0;
            CarRunResult.OverlapEntryExit thisOverlapEntry = new CarRunResult.OverlapEntryExit();

            if (overlap.Contains(car.State.Floor))
            {
                if (GeneralTools.getDirectionFromHereToThere(car.State.Floor, car.ParkFloor) != car.State.Direction)
                {
                    thisOverlapEntry.entry = 0;
                }
            }

            int currentLoad = car.NumberOfPassengers;
            int currentFloor = car.State.Floor;
            Direction currentDirection = car.State.Direction;

            while (orderedCalls.Any())
            {
                Call call = orderedCalls.First();

                if (call.CallLocation == currentFloor
                    && (call.CallDirection == currentDirection || call.CallDirection == Direction.None))
                {
                    // can serve call
                    if (call is HallCall)
                    {
                        if (currentLoad + call.Passengers.Size > car.TotalCapacity && EnforcePenalties)
                        {
                            result.penaltyCosts.Add(NoCapacityAllocationPenaltySeconds);
                        }

                        currentTime += (LoadPersonTimeSeconds * call.Passengers.Size);
                        currentLoad += call.Passengers.Size;
                    }
                    if (call is CarCall)
                    {
                        if (!object.ReferenceEquals(call.Passengers, groupUnderAllocation))
                        {
                            result.systemCosts.Add(currentTime);
                        }
                        else
                        {
                            result.groupCosts.Add(currentTime);
                        }

                        currentTime += (UnloadPersonTimeSeconds * call.Passengers.Size);
                        currentLoad -= call.Passengers.Size;
                    }

                    orderedCalls.Remove(call);
                }
                else if (call.CallLocation == currentFloor)
                {
                    // can serve call at this floor but must reverse
                    currentDirection = currentDirection == Direction.Down ? Direction.Up : Direction.Down;
                    currentTime += ReverseTimeSeconds;
                    if (thisOverlapEntry.entry != null)
                    {
                        thisOverlapEntry.exit = currentTime;
                        result.overlapEntries.Add(thisOverlapEntry);
                        thisOverlapEntry = new CarRunResult.OverlapEntryExit();
                    }
                }
                else if (GeneralTools.getDirectionFromHereToThere(currentFloor, call.CallLocation) != currentDirection)
                {
                    // reverse in order to move towards call
                    currentDirection = currentDirection == Direction.Down ? Direction.Up : Direction.Down;
                    currentTime += ReverseTimeSeconds;
                    if (thisOverlapEntry.entry != null)
                    {
                        thisOverlapEntry.exit = currentTime;
                        result.overlapEntries.Add(thisOverlapEntry);
                        thisOverlapEntry = new CarRunResult.OverlapEntryExit();
                    }
                }
                else
                {
                    // don't need to reverse, move to call floor
                    currentTime += StartTimeSeconds;
                    currentTime += Math.Abs(currentFloor - call.CallLocation) * FloorTravelTimeSeconds;
                    currentTime += StopTimeSeconds;
                    currentFloor = call.CallLocation;
                    if (thisOverlapEntry.entry != null)
                    {
                        if (overlap.Contains(currentFloor))
                        {
                            thisOverlapEntry.exit = currentTime;
                        }
                    }
                }
            }

            return result;
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
