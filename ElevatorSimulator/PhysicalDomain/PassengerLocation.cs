using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.PhysicalDomain
{
    class PassengerLocation
    {
        protected List<PassengerGroup> passengers;

        public bool addPassengers(PassengerGroup newPassengers)
        {
            this.passengers.Add(newPassengers);

            return true;
        }

        public int getNumberOfPassengers()
        {
            int count = 0;

            foreach (PassengerGroup p in this.passengers)
            {
                count += p.getSize();
            }

            return count;
        }
    }
}
