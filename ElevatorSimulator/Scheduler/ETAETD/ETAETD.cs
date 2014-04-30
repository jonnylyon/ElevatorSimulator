using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Tools;

namespace ElevatorSimulator.Scheduler.ETAETD
{
    enum OptimizationType
    {
        WaitingTime,
        SystemTime
    }

    class ETAETD : IScheduler
    {
        private OptimizationType OptimizationType;
        private int EstimationTimePower;
        private bool EnforcePenalties;

        private double StopTimeSeconds = 5;
        private double StartTimeSeconds = 5;
        private double ReverseTimeSeconds = 1;
        private double UnloadPersonTimeSeconds = 2;
        private double LoadPersonTimeSeconds = 2;
        private double FloorTravelTimeSeconds = 1;
        private double NoCapacityAllocationPenaltySeconds = 1000;

        public ETAETD(OptimizationType OptimizationType, int EstimationTimePower, bool EnforcePenalties)
        {
            this.OptimizationType = OptimizationType;
            this.EstimationTimePower = EstimationTimePower;
            this.EnforcePenalties = EnforcePenalties;
        }

        public void AllocateCall(PassengerGroup group, Building building)
        {
            // compile list of all cars (can we do this in one linq expression?)
            List<Car> cars = new List<Car>();
            building.Shafts.ForEach(s => s.Cars.ForEach(c => cars.Add((Car)c)));

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

        public double CalculateCost(Car car, PassengerGroup group)
        {
            var originalCalls = new ExpandedAllocationList(car.CallAllocationList);
            var modifiedCalls = new ExpandedAllocationList(car.CallAllocationList);

            Pass pass = Tools.CalculatePassOfCall(car, group);

            modifiedCalls.AddGroupCallsToPass(group, pass);

            double costExcludingAllocation;
            double costIncludingAllocation;

            var originalCarRunResult = PerformCarRun(originalCalls, car);
            var modifiedCarRunResult = PerformCarRun(modifiedCalls, car);

            costExcludingAllocation = originalCarRunResult.TotalCost;
            costIncludingAllocation = modifiedCarRunResult.TotalCost;

            return costIncludingAllocation - costExcludingAllocation;
        }

        class CarRunResult
        {
            public List<double> systemCosts = new List<double>();
            public List<double> groupCosts = new List<double>();
            public List<double> penaltyCosts = new List<double>();

            private int EstimationTimePower;

            public CarRunResult(int EstimationTimePower)
            {
                this.EstimationTimePower = EstimationTimePower;
            }

            public double TotalCost
            {
                get
                {
                    return systemCosts.Sum(c => Math.Pow(c, EstimationTimePower)) + groupCosts.Sum(c => Math.Pow(c, EstimationTimePower)) + penaltyCosts.Sum(c => Math.Pow(c, EstimationTimePower));
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
            }
        }

        private CarRunResult PerformCarRun(ExpandedAllocationList calls, Car car, PassengerGroup groupUnderAllocation = null)
        {
            var orderedCalls = calls.GetOrderedListOfAllCalls(car.State.Direction);

            CarRunResult result = new CarRunResult(EstimationTimePower);

            double currentTime = 0;

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
                        if (this.OptimizationType == OptimizationType.WaitingTime)
                        {
                            if (!object.ReferenceEquals(call.Passengers, groupUnderAllocation))
                            {
                                result.systemCosts.Add(currentTime);
                            }
                            else
                            {
                                result.groupCosts.Add(currentTime);
                            }
                        }

                        if (currentLoad + call.Passengers.Size > car.TotalCapacity && EnforcePenalties)
                        {
                            result.penaltyCosts.Add(NoCapacityAllocationPenaltySeconds);
                        }

                        currentTime += (LoadPersonTimeSeconds * call.Passengers.Size);
                        currentLoad += call.Passengers.Size;
                    }
                    if (call is CarCall)
                    {
                        if (this.OptimizationType == OptimizationType.SystemTime)
                        {
                            if (!object.ReferenceEquals(call.Passengers, groupUnderAllocation))
                            {
                                result.systemCosts.Add(currentTime);
                            }
                            else
                            {
                                result.groupCosts.Add(currentTime);
                            }
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

            return result;
        }
    }
}
