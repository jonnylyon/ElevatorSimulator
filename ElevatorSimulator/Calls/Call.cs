using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator.Calls
{
    /// <summary>
    /// Abstract class representing calls from passengers.
    /// </summary>
    abstract class Call
    {
        public PassengerGroup Passengers { get; private set; }

        public Call(PassengerGroup passengerGroup)
        {
            this.Passengers = passengerGroup;
        }
        
        /// <summary>
        /// Direction of the Call
        /// </summary>
        public abstract Direction CallDirection { get; }

        /// <summary>
        /// The relative destination for the elevator to visit,
        /// i.e if hall call, then the origin of the call,
        /// if car call, the destination of the call.
        /// </summary>
        /// <returns>The number of the floor the elevator should visit</returns>
        public abstract int CallLocation { get; } // TODO im not convinced this should be in this class? It's more elevator logic than anything to do with the call?
    }
}
