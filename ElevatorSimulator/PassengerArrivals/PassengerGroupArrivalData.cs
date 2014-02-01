using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.PassengerArrivals
{
    // Could do with a shorter name
    // Also we can maybe just use the existing PassengerGroup class for this
    // but it introduces some coupling that we might not want
    class PassengerGroupArrivalData
    {
        public DateTime ArrivalTime { get; set; }
        public int Size { get; set; }
        public int Origin { get; set; }
        public int Destination { get; set; }
    }
}
