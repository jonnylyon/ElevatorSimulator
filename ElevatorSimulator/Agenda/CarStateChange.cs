using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Agenda
{
    class CarStateChange : Event
    {
        private CarState newState;

        public CarStateChange(Car owner, DateTime time, CarState newState)
            :base(owner, time)
        {
            this.newState = newState;
        }
    }
}
