using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.PhysicalDomain
{
    class Shaft
    {
        private readonly ShaftData shaftData;

        public List<ICar> Cars { get; private set; }

        public Shaft(ShaftData data)
        {
            Cars = new List<ICar>();
            shaftData = data;
        }

        internal void addCar(CarAttributes attributes, int startFloor, CarType type)
        {
            switch (type)
            {
                case CarType.Single:
                    this.Cars.Add(new Car(shaftData, attributes, startFloor));
                    break;
                case CarType.Double:
                    //TODO
                    break;
            }
        }
    }
}
