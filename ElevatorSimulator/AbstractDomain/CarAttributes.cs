using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.AbstractDomain
{
    class CarAttributes
    {
        public int Capacity { get; set; }

        public double MaxSpeed { get; set; } // in metres per second
        public double Acceleration { get; set; } // in metres per second squared; assumes linear acc'n
        public double Deceleration { get; set; } // in positive metres per second squared; assumes linear dec'n

        public double DirectionChangeTime { get; set; } // in seconds
        public double PassengerBoardTime { get; set; } // in seconds
        public double PassengerAlightTime { get; set; } // in seconds

        public double DoorsCloseTime { get; set; } // in seconds
        public double DoorsOpenTime { get; set; } // in seconds

        public CarAttributes()
        {
        }
    }
}
