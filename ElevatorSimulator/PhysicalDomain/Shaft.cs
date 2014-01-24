using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.PhysicalDomain
{
    class Shaft
    {
        private int topFloor;
        private int bottomFloor;
        private double interfloorDistance = 10;

        private List<Car> cars = new List<Car>();

        internal void addCar(Car car)
        {
            this.cars.Add(car);
            car.setShaft(this);
        }

        internal int getBottomFloor()
        {
            return this.bottomFloor;
        }
        
        /// <summary>
        /// Get the distance in metres between the specified floor and the
        /// floor immediately above or below it
        /// </summary>
        /// <param name="floor">The specified floor</param>
        /// <param name="direction">The specified direction</param>
        /// <returns>Interfloor Distance in metres</returns>
        internal double getInterfloorDistance(int floor, Direction direction)
        {
            // Current implementation assumes that all interfloor distances are the same
            return interfloorDistance;
        }
    }
}
