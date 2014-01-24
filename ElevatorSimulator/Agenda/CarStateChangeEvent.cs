using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Agenda
{
    /// <summary>
    /// Object representing a car state change, containing the
    /// new car state.
    /// </summary>
    class CarStateChangeEvent : AgendaEvent
    {
        public CarState CarState { get; private set; }

        public CarStateChangeEvent(Car owner, DateTime time, CarState newState)
            :base(owner, time)
        {
            this.CarState = newState;
        }
    }
}
