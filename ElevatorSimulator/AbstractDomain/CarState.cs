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
        // These instance variables store the data for the
        // properties defined below
        private Direction direction;
        private int floor;
        private double initialSpeed;
        private CarAction action;

        /**
         * The direction in which the car is moving
         **/
        public Direction Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }

        /**
         * The current floor of the car
         */
        public int Floor
        {
            get { return this.floor; }
            set { this.floor = value; }
        }

        /**
         * The current action of the car
         */
        public CarAction Action
        {
            get { return this.action; }
            set { this.action = value; }
        }

        /**
         * The speed of the car when it entered this state
         */
        public double InitialSpeed
        {
            get { return this.initialSpeed; }
            set { this.initialSpeed = value; }
        }
    }
}
