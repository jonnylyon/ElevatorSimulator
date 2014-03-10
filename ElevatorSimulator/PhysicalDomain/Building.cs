using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.PhysicalDomain
{
    class Building
    {
        public List<Shaft> Shafts { get; set; }

        private int shaftIdCounter;

        public Building()
        {
            this.Shafts = new List<Shaft>();
            shaftIdCounter = 1;
        }

        public Shaft addShaft(int topFloor, int bottomFloor, double interfloor)
        {
            var shaftData = new ShaftData(topFloor, bottomFloor, interfloor, shaftIdCounter);
            var shaft = new Shaft(shaftData);
            this.Shafts.Add(shaft);
            shaftIdCounter++;

            return shaft;
        }

        public bool allIdleOrParked()
        {
            bool result = true;

            foreach (Shaft shaft in this.Shafts)
            {
                foreach (ICar car in shaft.Cars)
                {
                    result &= car.State.Action == CarAction.Idle || car.State.Action == CarAction.Parked;
                }
            }

            return result;
        }
    }
}
