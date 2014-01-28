using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using System.Collections;

namespace ElevatorSimulator.Calls
{
    class CallsList : List<Call>
    {
        public void addCall(Call call)
        {
            this.Add(call);
        }
        
        public bool isEmpty()
        {
            return this.Count == 0;
        }

        public Call getNextCallInCurrentDirection(int currentFloor, Direction direction)
        {
            if (isEmpty())
            {
                //TODO
                return null;
            }

            if (direction == Direction.Up)
            {
                var filtered = this.Where(a => a.getElevatorDestination() >= currentFloor);
                return  filtered.Count() == 0 ? null : filtered.OrderBy(a => a.getElevatorDestination()).First();                
            }

            if (direction == Direction.Down)
            {
                var filtered = this.Where(a => a.getElevatorDestination() <= currentFloor);
                return filtered.Count() == 0 ? null : filtered.OrderBy(a => a.getElevatorDestination()).Last();
            }

            // TODO If idle?
            return null;
        }

        public Call getLowestCall()
        {
            return this.OrderBy(a => a.getElevatorDestination()).First();
        }

        public Call getHighestCall()
        {
            return this.OrderBy(a => a.getElevatorDestination()).Last();
        }

        internal void removeCall(Call call)
        {
            this.Remove(call);
        }
    }
}
