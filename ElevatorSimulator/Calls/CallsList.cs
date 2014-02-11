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
        public bool isEmpty()
        {
            return this.Count == 0;
        }

        // If there are two calls for the same floor, this is going to return an arbitrary one
        // (probably the one that was added first?).  Be aware of this when using the method.
        public Call getNextCallInCurrentDirection(int currentFloor, Direction direction)
        {
            if (isEmpty())
            {
                //TODO
                return null;
            }

            if (direction == Direction.Up)
            {
                var filtered = this.Where(a => a.CallLocation >= currentFloor);
                return  filtered.Count() == 0 ? null : filtered.OrderBy(a => a.CallLocation).First();                
            }

            if (direction == Direction.Down)
            {
                var filtered = this.Where(a => a.CallLocation <= currentFloor);
                return filtered.Count() == 0 ? null : filtered.OrderBy(a => a.CallLocation).Last();
            }

            // TODO If idle?
            return null;
        }

        public Call getLowestCall()
        {
            return this.OrderBy(a => a.CallLocation).First();
        }

        public Call getHighestCall()
        {
            return this.OrderBy(a => a.CallLocation).Last();
        }
    }
}
