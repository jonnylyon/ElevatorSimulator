using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.PhysicalDomain
{
    class PassengerGroup : Agenda.IEventOwner
    {
        private int size;
        private int origin;
        private int destination;
        private PassengerLocation currentLocation;

        public int getSize()
        {
            return this.size;
        }
    }
}
