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

        public List<SingleDeckCollisionDetector> CollisionDetectors { get; private set; }

        public Shaft(ShaftData data)
        {
            Cars = new List<ICar>();
            CollisionDetectors = new List<SingleDeckCollisionDetector>();
            shaftData = data;
        }

        internal void addCar(CarAttributes attributes, int startFloor, CarType type)
        {
            switch (type)
            {
                case CarType.Single:
                    var newCar = new Car(shaftData, attributes, startFloor);
                    this.Cars.Add(newCar);

                    foreach (Car c in this.Cars.Where(c => !object.ReferenceEquals(c, newCar)))
                    {
                        if (c.State.Floor > newCar.State.Floor)
                        {
                            this.CollisionDetectors.Add(new SingleDeckCollisionDetector(c, newCar));
                        }
                        else
                        {
                            this.CollisionDetectors.Add(new SingleDeckCollisionDetector(newCar, c));
                        }
                    }

                    break;
                case CarType.Double:
                    //TODO
                    break;
            }
        }
    }
}
