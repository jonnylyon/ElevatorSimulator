using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.AbstractDomain
{
    class CarAttributes
    {
        public readonly int Capacity = 18;

        public readonly double MaxSpeed = 5; // in metres per second
        public readonly double Acceleration = 2; // in metres per second squared; assumes linear acc'n
        public readonly double Deceleration = 4; // in positive metres per second squared; assumes linear dec'n

        public readonly double DirectionChangeTime = 1; // in seconds
        public readonly double PassengerBoardTime = 10; // in seconds
        public readonly double PassengerAlightTime = 10; // in seconds

        public readonly double DoorsCloseTime = 3; // in seconds
        public readonly double DoorsOpenTime = 3; // in seconds

        public CarAttributes()
        {
        }
    }
}
