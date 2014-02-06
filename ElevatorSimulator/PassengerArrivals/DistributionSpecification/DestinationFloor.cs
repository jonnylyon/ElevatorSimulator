using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.PassengerArrivals.DistributionSpecification
{
    class DestinationFloor
    {
        public int Floor { get; set; }
        public double ArrivalsPerMinuteMean { get; set; }
        public double GroupSizeMean { get; set; }
    }
}
