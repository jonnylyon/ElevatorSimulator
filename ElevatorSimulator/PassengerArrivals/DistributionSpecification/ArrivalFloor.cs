using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.PassengerArrivals.DistributionSpecification
{
    class ArrivalFloor
    {
        public int Floor { get; set; }
        private List<DestinationFloor> _DestinationFloors = new List<DestinationFloor>();

        public List<DestinationFloor> DestinationFloors
        {
            get
            {
                return this._DestinationFloors;
            }
        }
    }
}
