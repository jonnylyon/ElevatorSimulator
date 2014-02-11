using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Calls;

namespace ElevatorSimulator.DataStructures
{
    class CallAllocationList
    {
        private CallsList p1Calls = new CallsList(); // Pass 1 (current direction)
        private CallsList p2Calls = new CallsList(); // Pass 2 (opposite direction, reverse once)
        private CallsList p3Calls = new CallsList(); // Pass 3 (opposite direction, reverse twice)

        public void addHallCall(HallCall hallCall, CarState state)
        {
            if (state.Direction == Direction.Up)
            {
                if (hallCall.CallDirection == Direction.Down)
                {
                    this.p2Calls.addCall(hallCall);
                }
                else if (hallCall.Passengers.Origin > state.Floor)
                {
                    this.p1Calls.addCall(hallCall);
                }
                else if (hallCall.Passengers.Origin == state.Floor && !(state.Action == CarAction.Moving || state.Action == CarAction.Leaving || state.Action == CarAction.DoorsClosing))
                {
                    // call is for the floor that we're currently stopped at
                    this.p1Calls.addCall(hallCall);
                }
                else
                {
                    this.p3Calls.addCall(hallCall);
                }
            }
            else
            {
                if (hallCall.CallDirection == Direction.Up)
                {
                    this.p2Calls.addCall(hallCall);
                }
                else if (hallCall.Passengers.Origin < state.Floor)
                {
                    this.p1Calls.addCall(hallCall);
                }
                else if (hallCall.Passengers.Origin == state.Floor && !(state.Action == CarAction.Moving || state.Action == CarAction.Leaving || state.Action == CarAction.DoorsClosing))
                {
                    // call is for the floor that we're currently stopped at
                    this.p1Calls.addCall(hallCall);
                }
                else
                {
                    this.p3Calls.addCall(hallCall);
                }
            }

        }

        public void addCarCall(CarCall carCall, CarState state)
        {
            if (state.Direction == Direction.Up && carCall.CallLocation >= state.Floor)
            {
                // if call is in current direction (upwards)
                this.p1Calls.addCall(carCall);
            }
            else if (state.Direction == Direction.Down && carCall.CallLocation <= state.Floor)
            {
                // if call is in current direction (downwards)
                this.p1Calls.addCall(carCall);
            }
            else
            {
                // if call is not in current direction
                this.p2Calls.addCall(carCall);
            }
        }

        public void removeCall(Call c)
        {
            this.p1Calls.removeCall(c);
            this.p2Calls.removeCall(c);
            this.p3Calls.removeCall(c);
        }

        public List<Call> getCarCallsForFloor(int floor)
        {
            return p1Calls.Where(c => c is CarCall && c.Passengers.Destination == floor).ToList();
        }

        public List<Call> getHallCallsForFloor(int floor)
        {
            return p1Calls.Where(c => c is HallCall && c.Passengers.Origin == floor).ToList();
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
    }
}
