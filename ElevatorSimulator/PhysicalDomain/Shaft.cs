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

        public List<Car> Cars { get; private set; }

        public Shaft(ShaftData data)
        {
            Cars = new List<Car>();
            shaftData = data;
        }

        internal void addCar(CarAttributes attributes, int startFloor)
        {
            this.Cars.Add(new Car(shaftData, attributes, startFloor));
        }

    }

}
