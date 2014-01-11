using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Calls
{
    class CallsList
    {
        List<Call> calls = new List<Call>();

        public void addCall(Call call)
        {
            calls.Add(call);
        }

        public int count()
        {
            return calls.Count;
        }

        public bool isEmpty()
        {
            return calls.Count == 0;
        }

        public Call getNextCall(int currentFloor, Direction direction)
        {
            Call nextCall = null;

            foreach (Call call in calls)
            {
                if (direction == Direction.Up && call.getFloor() >= currentFloor && (nextCall == null || call.getFloor() < nextCall.getFloor()))
                {
                    nextCall = call;
                }
                else if (direction == Direction.Down && call.getFloor() <= currentFloor && (nextCall == null || call.getFloor() > nextCall.getFloor()))
                {
                    nextCall = call;
                }
            }

            return nextCall;
        }

        public Call getLowestCall()
        {
            Call nextCall = null;

            foreach (Call call in calls)
            {
                if (nextCall == null || call.getFloor() < nextCall.getFloor())
                {
                    nextCall = call;
                }
            }

            return nextCall;
        }

        public Call getHighestCall()
        {
            Call nextCall = null;

            foreach (Call call in calls)
            {
                if (nextCall == null || call.getFloor() > nextCall.getFloor())
                {
                    nextCall = call;
                }
            }

            return nextCall;
        }
    }
}
