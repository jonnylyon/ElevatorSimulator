using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.Tools;

namespace ElevatorSimulator.DataStructures
{
    class CallAllocationList
    {
        private CallsList p1Calls = new CallsList(); // Pass 1 (current direction)
        private CallsList p2Calls = new CallsList(); // Pass 2 (opposite direction, reverse once)
        private CallsList p3Calls = new CallsList(); // Pass 3 (current direction, reverse twice)

        public CallsList P1List { get { return p1Calls; } }
        public CallsList P2List { get { return p2Calls; } }
        public CallsList P3List { get { return p3Calls; } }

        private List<Call> allCalls
        {
            get
            {
                return p1Calls.Union(p2Calls).Union(p3Calls).ToList();
            }
        }

        public void addHallCall(HallCall hallCall, CarState state)
        {
            if (state.Direction == Direction.Up)
            {
                if (hallCall.CallDirection == Direction.Down)
                {
                    this.p2Calls.Add(hallCall);
                }
                else if (hallCall.Passengers.Origin > state.Floor)
                {
                    this.p1Calls.Add(hallCall);
                }
                else if (hallCall.Passengers.Origin == state.Floor && state.Action != CarAction.Leaving && state.Action != CarAction.Moving)
                {
                    // call is for the floor that we're currently stopped at
                    this.p1Calls.Add(hallCall);
                }
                else
                {
                    this.p3Calls.Add(hallCall);
                }
            }
            else
            {
                if (hallCall.CallDirection == Direction.Up)
                {
                    this.p2Calls.Add(hallCall);
                }
                else if (hallCall.Passengers.Origin < state.Floor)
                {
                    this.p1Calls.Add(hallCall);
                }
                else if (hallCall.Passengers.Origin == state.Floor && state.Action != CarAction.Leaving && state.Action != CarAction.Moving)
                {
                    // call is for the floor that we're currently stopped at
                    this.p1Calls.Add(hallCall);
                }
                else
                {
                    this.p3Calls.Add(hallCall);
                }
            }

        }

        public void addCarCall(CarCall carCall, CarState state)
        {
            if (state.Direction == Direction.Up && carCall.CallLocation >= state.Floor)
            {
                // if call is in current direction (upwards)
                this.p1Calls.Add(carCall);
            }
            else if (state.Direction == Direction.Down && carCall.CallLocation <= state.Floor)
            {
                // if call is in current direction (downwards)
                this.p1Calls.Add(carCall);
            }
            else
            {
                // if call is not in current direction
                this.p2Calls.Add(carCall);
            }
        }

        public void removeCall(Call c)
        {
            this.p1Calls.Remove(c);
            this.p2Calls.Remove(c);
            this.p3Calls.Remove(c);
        }

        public List<Call> getCarCallsForFloor(int floor)
        {
            return p1Calls.Where(c => c is CarCall && c.Passengers.Destination == floor).ToList();
        }

        public List<Call> getHallCallsForFloor(int floor, Direction direction)
        {
            return p1Calls.Where(c => c is HallCall && c.Passengers.Origin == floor && c.Passengers.Direction == direction).ToList();
        }

        public bool isEmpty()
        {
            if (p1Calls.isEmpty() && p2Calls.isEmpty() && p3Calls.isEmpty())
            {
                return true;
            }

            return false;
        }

        public Call getNextCall(CarState state)
        {
            if (this.p1Calls.Count > 0)
            {
                return this.p1Calls.getNextCallInCurrentDirection(state.Floor, state.Direction);
            }

            if (this.p2Calls.Count > 0)
            {
                if (state.Direction == Direction.Up)
                {
                    return this.p2Calls.getHighestCall();
                }

                return this.p2Calls.getLowestCall();
            }

            if (this.p3Calls.Count > 0)
            {
                if (state.Direction == Direction.Up)
                {
                    return this.p3Calls.getLowestCall();
                }

                return this.p3Calls.getHighestCall();
            }

            return null;
        }

        public void reverseListsDirection()
        {
            if (!p1Calls.isEmpty())
            {
                //todo throw exception
            }

            this.p1Calls = this.p2Calls;
            this.p2Calls = this.p3Calls;
            this.p3Calls = new CallsList();
        }

        public void postponeHallCall(HallCall c)
        {
            if (p1Calls.Contains(c))
            {
                p1Calls.Remove(c);
                p3Calls.Add(c);
            }
        }

        public List<Call> getOrderedCalls(CarState state)
        {
            List<Call> calls;

            if (state.Direction == Direction.Down)
            {
                calls = p1Calls.OrderBy(c => c.CallLocation).ToList();
                calls.AddRange(p2Calls.OrderByDescending(c => c.CallLocation).ToList());
                calls.AddRange(p3Calls.OrderBy(c => c.CallLocation).ToList());
            }
            else
            {
                calls = p1Calls.OrderByDescending(c => c.CallLocation).ToList();
                calls.AddRange(p2Calls.OrderBy(c => c.CallLocation).ToList());
                calls.AddRange(p3Calls.OrderByDescending(c => c.CallLocation).ToList());
            }

            return calls;            
        }

        internal List<int> getCurrentCallsZone(CarState state)
        {
            int? lowest = null;
            int? highest = null;

            if (state.Direction == Direction.Up)
            {
                int? highestP1 = null;
                int? highestP2 = null;

                if (!p1Calls.isEmpty())
                {
                    lowest = p1Calls.Min(c => c is HallCall ? Math.Min(c.Passengers.Origin, c.Passengers.Destination) : c.Passengers.Destination);
                    highestP1 = p1Calls.Max(c => c is HallCall ? Math.Max(c.Passengers.Origin, c.Passengers.Destination) : c.Passengers.Destination);
                }

                if (!p2Calls.isEmpty())
                {
                    highestP2 = p2Calls.Max(c => Math.Max(c.Passengers.Origin, c.Passengers.Destination));
                }

                lowest = GeneralTools.notNullOption(lowest, highestP2);
                highest = GeneralTools.max(highestP1, highestP2);
            }

            if (state.Direction == Direction.Down)
            {
                int? lowestP1 = null;
                int? lowestP2 = null;

                if (!p1Calls.isEmpty())
                {
                    highest = p1Calls.Max(c => c is HallCall ? Math.Max(c.Passengers.Origin, c.Passengers.Destination) : c.Passengers.Destination);
                    lowestP1 = p1Calls.Min(c => c is HallCall ? Math.Min(c.Passengers.Origin, c.Passengers.Destination) : c.Passengers.Destination);
                }

                if (!p2Calls.isEmpty())
                {
                    lowestP2 = p2Calls.Min(c => Math.Min(c.Passengers.Origin, c.Passengers.Destination));
                }

                lowest = GeneralTools.min(lowestP1, lowestP2);
                highest = GeneralTools.notNullOption(highest, lowestP2);
            }

            return GeneralTools.getRange(lowest, highest);
        }

        internal List<int> getCallsZoneIfReversed(CarState state)
        {
            int? lowest = null;
            int? highest = null;

            if (state.Direction == Direction.Down)
            {
                int? highestP2 = null;
                int? highestP3 = null;

                if (!p2Calls.isEmpty())
                {
                    lowest = p2Calls.Min(c => Math.Min(c.Passengers.Origin, c.Passengers.Destination));
                    highestP2 = p2Calls.Max(c => Math.Max(c.Passengers.Origin, c.Passengers.Destination));
                }

                if (!p3Calls.isEmpty())
                {
                    highestP3 = p3Calls.Max(c => Math.Max(c.Passengers.Origin, c.Passengers.Destination));
                }

                lowest = GeneralTools.min(lowest, highestP3);
                highest = GeneralTools.max(highestP2, highestP3);
            }

            if (state.Direction == Direction.Up)
            {
                int? lowestP2 = null;
                int? lowestP3 = null;

                if (!p2Calls.isEmpty())
                {
                    highest = p2Calls.Min(c => Math.Min(c.Passengers.Origin, c.Passengers.Destination));
                    lowestP2 = p2Calls.Min(c => Math.Min(c.Passengers.Origin, c.Passengers.Destination));
                }

                if (!p3Calls.isEmpty())
                {
                    lowestP3 = p3Calls.Min(c => Math.Min(c.Passengers.Origin, c.Passengers.Destination));
                }

                lowest = GeneralTools.min(lowestP2, lowestP3);
                highest = GeneralTools.max(highest, lowestP3);
            }

            return GeneralTools.getRange(lowest, highest);
        }
    }
}
