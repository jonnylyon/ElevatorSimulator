using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.PhysicalDomain
{
    class SingleDeckCollisionDetector
    {
        private readonly Car upper;
        private readonly Car lower;
        private Car c;
        private ICar newCar;

        public bool CollisionOccurred
        {
            get
            {
                if (upper.State.Floor <= lower.State.Floor)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public SingleDeckCollisionDetector(Car upper, Car lower)
        {
            this.upper = upper;
            this.lower = lower;
        }
    }
}
