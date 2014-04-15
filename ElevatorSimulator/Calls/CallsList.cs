using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using System.Collections;
using ElevatorSimulator.Tools;

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

        /// <summary>
        /// This only includes the immediate location of the call (i.e.
        /// for hall calls, only the origin and not the destination)
        /// </summary>
        /// <returns>The location of the lowest call</returns>
        public Call getLowestCall()
        {
            return this.OrderBy(a => a.CallLocation).First();
        }

        public int? getLowestCallLocation()
        {
            var lc = getLowestCall();

            if (object.ReferenceEquals(lc, null))
            {
                return null;
            }

            return lc.CallLocation;
        }

        /// <summary>
        /// The furthest location in the destination specified including
        /// both the origin and destination of hall calls
        /// </summary>
        /// <param name="direction">The direction specified</param>
        /// <returns>The furthest location</returns>
        public int? getFurthestLocation(Direction direction)
        {
            Call call;

            if (direction == Direction.Down)
            {
                call = this.OrderBy(a => Math.Min(a.Passengers.Origin, a.Passengers.Destination)).First();
                if (object.ReferenceEquals(call, null))
                {
                    return null;
                }
                return GeneralTools.min(call.Passengers.Origin, call.Passengers.Destination);
            }

            call = this.OrderBy(a => Math.Max(a.Passengers.Origin, a.Passengers.Destination)).Last();
            if (object.ReferenceEquals(call, null))
            {
                return null;
            }
            return GeneralTools.max(call.Passengers.Origin, call.Passengers.Destination);
        }

        /// <summary>
        /// This only includes the immediate location of the call (i.e.
        /// for hall calls, only the origin and not the destination)
        /// </summary>
        /// <returns>The location of the highest call</returns>
        public Call getHighestCall()
        {
            return this.OrderBy(a => a.CallLocation).Last();
        }

        public int? getHighestCallLocation()
        {
            var hc = getHighestCall();

            if (object.ReferenceEquals(hc, null))
            {
                return null;
            }

            return hc.CallLocation;
        }
    }
}
