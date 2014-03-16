using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Calls;

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

            return true;

            //var currentZone = car.CurrentCompleteZone;
            //var overlapZone = car.CurrentCompleteZone.Intersect(otherCar.CurrentCompleteZone).ToList();

            //// If both the origin and destination of the call are within the car’s current zone, the call will be accepted.
            //if (currentZone.Contains(call.Passengers.Origin) && currentZone.Contains(call.Passengers.Destination))
            //{
            //    return true;
            //}

            //// If the other car has no current zone, and both the origin and destination of the call lie within the range of
            //// floors that the car’s current zone can be extended into without including the current floor of the other car
            //// in the same shaft, the call will be accepted and the car’s current zone will be extended accordingly.
            //if (!otherCar.CurrentZone.Any())
            //{
            //    List<int> outOfBoundsZone;

            //    // Upper car logic
            //    if (car.State.Floor > otherCar.State.Floor)
            //    {
            //        outOfBoundsZone = Enumerable.Range(shaftData.BottomFloor, otherCar.CurrentFloorsOccupied.Max() + 1 - shaftData.BottomFloor).ToList();
            //    }
            //    // Lower car logic
            //    else
            //    {
            //        outOfBoundsZone = Enumerable.Range(otherCar.CurrentFloorsOccupied.Min(), shaftData.TopFloor + 1 - otherCar.CurrentFloorsOccupied.Min()).ToList();
            //    }

            //    var allowableZone = allFloors.Except(outOfBoundsZone).ToList();

            //    if (allowableZone.Contains(call.Passengers.Origin) && allowableZone.Contains(call.Passengers.Destination))
            //    {
            //        return true;
            //    }

            //    return false;
            //}

            //// If the car’s current floor is not inside the overlap zone, and both the origin and destination of the call
            //// are within the other car’s zone without being the last floor in its zone (or the other car is parking), the
            //// call will be accepted and the car’s current zone will be extended accordingly.
            //// Or, if the car is  moving away from its park floor and the other car is moving in the same direction
            //// (away from this car)
            //if (!overlapZone.Contains(car.State.Floor) || (!car.movingInDirectionOfParkFloor() && car.State.Direction == otherCar.State.Direction) )
            //{
            //    List<int> outOfBoundsZone = null;

            //    // Upper car logic
            //    if (car.State.Floor > otherCar.State.Floor)
            //    {
            //        outOfBoundsZone = Enumerable.Range(shaftData.BottomFloor, otherCar.CurrentZone.Min() + 1 - shaftData.BottomFloor).ToList();
            //    }
            //    // Lower car logic
            //    else
            //    {
            //        outOfBoundsZone = Enumerable.Range(otherCar.CurrentZone.Max(), shaftData.TopFloor + 1 - otherCar.CurrentZone.Max()).ToList();
            //    }

            //    var allowableZone = allFloors.Except(outOfBoundsZone).ToList();

            //    if (allowableZone.Contains(call.Passengers.Origin) && allowableZone.Contains(call.Passengers.Destination))
            //    {
            //        return true;
            //    }
            //}

            //// If both the origin and destination lie within the range of floors that the car’s current zone can be extended
            //// into without including the current floor of the other car in the same shaft, the call will be accepted and the
            //// car’s current zone will be extended accordingly.
            //if (!overlapZone.Contains(car.State.Floor))
            //{
            //    List<int> outOfBoundsZone;

            //    // Upper car logic
            //    if (car.State.Floor > otherCar.State.Floor)
            //    {
            //        outOfBoundsZone = Enumerable.Range(shaftData.BottomFloor, otherCar.CurrentFloorsOccupied.Max() + 1 - shaftData.BottomFloor).ToList();
            //    }
            //    // Lower car logic
            //    else
            //    {
            //        outOfBoundsZone = Enumerable.Range(otherCar.CurrentFloorsOccupied.Min(), shaftData.TopFloor + 1 - otherCar.CurrentFloorsOccupied.Min()).ToList();
            //    }

            //    var allowableZone = allFloors.Except(outOfBoundsZone).ToList();

            //    if (allowableZone.Contains(call.Passengers.Origin) && allowableZone.Contains(call.Passengers.Destination))
            //    {
            //        return true;
            //    }
            //}

            //// Otherwise, the call will be rejected and the Scheduler will have to allocate the call to another car.
            //return false;
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

            var thisCarZone = car.CurrentCompleteZone;
            var otherCarZone = otherCar.CurrentCompleteZone;
            var overlapZone = thisCarZone.Intersect(otherCarZone).ToList();

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

            var thisCarZone = car.CurrentCompleteZone;
            var otherCarZone = otherCar.CurrentCompleteZone;
            var overlapZone = thisCarZone.Intersect(otherCarZone).ToList();

            // If next floor is not within the other car's zone, go ahead and move
            if (!otherCarZone.Contains(nextFloor))
            {
                return true;
            }

            // If the zone of this car is the empty list, this car will be parking and moving away from other car
            // If both of those things are true, go ahead and move
            if (!car.CurrentZone.Any() && ((nextFloor - car.State.Floor) * (car.State.Floor - otherCar.State.Floor) > 0))
            {
                return true;
            }

            // If the next call is in the overlap zone && the other car is outside of the overlap zone, go ahead and move
            if (overlapZone.Contains(nextFloor) && !overlapZone.Intersect(otherCar.CurrentFloorsOccupied).Any())
            {
                return true;
            }

            // If the next call is in the overlap zone, the other car is inside the overlap zone but moving in the same
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
