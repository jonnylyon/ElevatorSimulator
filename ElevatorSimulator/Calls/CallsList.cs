using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using System.Collections;

namespace ElevatorSimulator.Calls
{
    class CallsList : IEnumerable
    {
        private List<Call> calls = new List<Call>();

        public IEnumerator GetEnumerator()
        {
            return calls.GetEnumerator();
        }

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

        public Call getNextCallInCurrentDirection(int currentFloor, Direction direction)
        {
            if (calls.Count == 0)
            {
                //TODO
                return null;
            }

            if (direction == Direction.Up)
            {
                var filtered = calls.Where(a => a.getElevatorDestination() >= currentFloor);
                return  filtered.Count() == 0 ? null : filtered.OrderBy(a => a.getElevatorDestination()).First();                
            }

            if (direction == Direction.Down)
            {
                var filtered = calls.Where(a => a.getElevatorDestination() <= currentFloor);
                return filtered.Count() == 0 ? null : filtered.OrderBy(a => a.getElevatorDestination()).Last();
            }

            // TODO If idle?
            return null;
        }

        public Call getLowestCall()
        {
            return calls.OrderBy(a => a.getElevatorDestination()).First();
        }

        public Call getHighestCall()
        {
            return calls.OrderBy(a => a.getElevatorDestination()).Last();
        }

        internal void removeCall(Call call)
        {
            calls.Remove(call);
        }
    }
}
