using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.Tools;

namespace ElevatorSimulator.PhysicalDomain
{
    class Shaft
    {
        private readonly ShaftData shaftData;

        public List<ICar> Cars { get; private set; }

        public List<int> allFloors
        {
            get
            {
                return Enumerable.Range(shaftData.BottomFloor, shaftData.TopFloor + 1 - shaftData.BottomFloor).ToList();
            }
        }

        public Shaft(ShaftData data)
        {
            Cars = new List<ICar>();
            shaftData = data;
        }

        internal void addCar(CarAttributes attributes, int startFloor, CarType type, int? parkFloor = null)
        {
            switch (type)
            {
                case CarType.Single:
                    var newSingleCar = new Car(this, shaftData, attributes, startFloor);
                    this.Cars.Add(newSingleCar);
                    break;
                case CarType.TCOS:
                    var newTCOSCar = new TCOSCar(this, shaftData, attributes, startFloor, parkFloor.Value, this.Cars.Count);
                    this.Cars.Add(newTCOSCar);
                    break;
                case CarType.Double:
                    // not used in TCOS Elevator Simulator
                    break;
            }
        }

        /// <summary>
        /// Checks whether or not the given TCOS car can accept the given
        /// hall call at the current time
        /// </summary>
        /// <param name="car">The TCOS car in question</param>
        /// <param name="call">The hall call in question</param>
        /// <returns>true if the hall call can be accepted, else false</returns>
        internal bool canAcceptHallCallTCOS(TCOSCar car, HallCall call)
        {
            // valid for TCOS as there will only be one other car
            TCOSCar otherCar = (TCOSCar)this.Cars.Where(c => !object.ReferenceEquals(c, car)).First();

            // if this is the lower car, and the call requires visiting the highest floor, reject call allocation
            if (car.State.Floor < otherCar.State.Floor)
            {
                if (call.Passengers.Origin >= this.shaftData.TopFloor || call.Passengers.Destination >= this.shaftData.TopFloor)
                {
                    return false;
                }
            }

            // if this call would be a P3 call, accept call allocation
            if (call.CallDirection == car.State.Direction)
            {
                if (car.State.Direction == Direction.Up && call.CallLocation < car.State.Floor)
                {
                    return true;
                }

                if (car.State.Direction == Direction.Down && call.CallLocation > car.State.Floor)
                {
                    return true;
                }
            }

            var carZone = car.CurrentZone;
            var otherCarZone = otherCar.CurrentZone;
            var overlapZone = carZone.Intersect(otherCarZone).ToList();

            var allShaftFloors = GeneralTools.getRange(this.shaftData.BottomFloor, this.shaftData.TopFloor);

            // if both the origin and destination call are within the car's current zone,
            // accept call allocation
            if (carZone.Contains(call.Passengers.Origin) && carZone.Contains(call.Passengers.Destination))
            {
                return true;
            }

            // if the car’s current floor is not inside the overlap zone, and both the
            // origin and destination of the call are within the other car’s zone without
            // being the last floor in its zone, the call will be accepted and the car’s current
            // zone will be extended accordingly.
            if (!overlapZone.Intersect(car.CurrentFloorsOccupied).Any())
            {
                List<int> outOfBoundsZone;

                if (car.State.Floor > otherCar.State.Floor)
                {
                    outOfBoundsZone = new List<int>() {this.shaftData.BottomFloor - 1};
                }
                else
                {
                    outOfBoundsZone = new List<int>() { this.shaftData.TopFloor };
                }

                var allowableZone = allShaftFloors.Except(outOfBoundsZone);

                if (allowableZone.Contains(call.Passengers.Origin) && allowableZone.Contains(call.Passengers.Destination))
                {
                    return true;
                }
            }

            // if the car's current floor is inside the overlap zone,
            // both the origin and destination lie within the range of floors that the car’s
            // current zone can be extended into without including the current floor of the other
            // car in the same shaft, the call will be accepted and the car’s current zone will
            // be extended accordingly.
            if (overlapZone.Intersect(car.CurrentFloorsOccupied).Any())
            {
                List<int> outOfBoundsZone;

                if (car.State.Floor > otherCar.State.Floor)
                {
                    outOfBoundsZone = GeneralTools.getRange(this.shaftData.BottomFloor - 1, otherCar.CurrentFloorsOccupied.Max());
                }
                else
                {
                    outOfBoundsZone = GeneralTools.getRange(otherCar.CurrentFloorsOccupied.Min(), this.shaftData.TopFloor);
                }

                var allowableZone = allShaftFloors.Except(outOfBoundsZone);

                if (allowableZone.Contains(call.Passengers.Origin) && allowableZone.Contains(call.Passengers.Destination))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether or not a car is required to keep going in its current direction (so as to
        /// allow the other car access to this car's current location)
        /// </summary>
        /// <param name="car">The car querying</param>
        /// <returns>true if the car must continue moving, else false</returns>
        internal bool mustContinueInCurrentDirection(TCOSCar car)
        {
            // If the car is currently moving away from its park location (i.e. the lower
            // car is moving up), then this method never returns true
            if (!car.movingInDirectionOfParkFloor())
            {
                return false;
            }

            TCOSCar otherCar = (TCOSCar)this.Cars.Where(c => !object.ReferenceEquals(c, car)).First();

            var thisCarZoneIfReversed = car.ZoneIfReversed;
            var otherCarZone = otherCar.CurrentZone;
            var overlapZone = thisCarZoneIfReversed.Intersect(otherCarZone).ToList();

            // if both cars are in the overlap zone and moving in the same direction
            if (overlapZone.Contains(otherCar.State.Floor) && overlapZone.Contains(car.State.Floor) && car.State.Direction == otherCar.State.Direction)
            {
                // if this car is ahead of the other car return true
                if (car.State.Floor < otherCar.State.Floor)
                {
                    if (car.State.Direction == Direction.Down)
                    {
                        return true;
                    }
                }
                else
                {
                    if (car.State.Direction == Direction.Up)
                    {
                        return true;
                    }
                }
            }

            // otherwise, return false
            return false;
        }

        /// <summary>
        /// Checks whether or not the given TCOS Car can move to the next floor in its current direction
        /// based on its interaction with other cars in the shaft
        /// </summary>
        /// <param name="car">The car trying to move</param>
        /// <returns>true if the move is valid, else false</returns>
        internal bool canMoveToNextFloorTCOS(TCOSCar car)
        {
            // Sanity; can't move if your direction is set to None
            if (car.State.Direction == Direction.None)
            {
                return false;
            }

            int nextFloor = car.State.Direction == Direction.Up ? car.State.Floor + 1 : car.State.Floor - 1;

            // valid for TCOS as there will only be one other car
            TCOSCar otherCar = (TCOSCar)this.Cars.Where(c => !object.ReferenceEquals(c, car)).First();

            // Can't move if the other car is already at that floor or moving towards it
            if (otherCar.CurrentFloorsOccupied.Contains(nextFloor))
            {
                return false;
            }

            var thisCarZone = car.CurrentZone;
            var otherCarZone = otherCar.CurrentZone;
            var overlapZone = thisCarZone.Intersect(otherCarZone).ToList();

            // If next floor is not within the other car's zone, go ahead and move
            if (!otherCarZone.Contains(nextFloor))
            {
                return true;
            }

            // If the car is moving away from other car, go ahead and move
            if ((nextFloor - car.State.Floor) * (car.State.Floor - otherCar.State.Floor) > 0)
            {
                return true;
            }

            // If the next floor is in the overlap zone && the other car is outside of the overlap zone, go ahead and move
            if (overlapZone.Contains(nextFloor) && !overlapZone.Intersect(otherCar.CurrentFloorsOccupied).Any())
            {
                return true;
            }

            // If the next floor is in the overlap zone, the other car is inside the overlap zone but moving in the same
            // direction as this car, go ahead and move
            if (overlapZone.Contains(nextFloor) && otherCar.State.Direction == car.State.Direction)
            {
                return true;
            }

            // Otherwise, do not move
            return false;
        }

        internal void carStateHasChanged(TCOSCar car)
        {
            var otherCars = this.Cars.Where(c => !object.ReferenceEquals(c, car));

            var allCarsWaiting = true;

            if (this.Cars.Count() > 1)
            {
                foreach (var c in this.Cars)
                {
                    allCarsWaiting &= c.State.Action == CarAction.Waiting;
                }

                if (allCarsWaiting)
                {
                    Simulation.logger.logLine(string.Format("All cars are now waiting in Shaft {0}", shaftData.ShaftId));

                    foreach (TCOSCar c in Cars)
                    {
                        Simulation.logger.logLine(string.Format("   Car zone: {0}; location: {1}", string.Join(", ", c.CurrentZone.ToArray()), string.Join(", ", c.CurrentFloorsOccupied.ToArray())));
                    }
                }
            }

            if (otherCars.Any())
            {
                // valid for TCOS as there will only be at most one other car
                TCOSCar otherCar = (TCOSCar)otherCars.First();

                otherCar.otherCarStateHasChanged();
            }
        }
    }
}
