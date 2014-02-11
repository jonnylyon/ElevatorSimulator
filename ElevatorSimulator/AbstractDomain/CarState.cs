using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.AbstractDomain
{
    /// <summary>
    /// Class storing a state of an elevator car, consisting of:
    /// Direction, current floor, current action, current speed.
    /// </summary>
    class CarState
    {
        /**
         * The direction in which the car is moving
         **/
        public Direction Direction { get; set; }

        /**
         * The current floor of the car
         */
        public int Floor { get; set; }

        /**
         * The current action of the car
         */
        public CarAction Action { get; set; } 

        /**
         * The speed of the car when it entered this state
         */
        public double InitialSpeed { get; set; }

        public bool DoorsOpen { get; set; }
    }
}
