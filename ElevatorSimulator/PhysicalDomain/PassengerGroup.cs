using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.Exceptions;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.PhysicalDomain
{
    /// <summary>
    /// Represents 1 or more passengers with the same origin, 
    /// destination and arrival times. 
    /// </summary>
    class PassengerGroup : Agenda.IEventOwner
    {
        public int Size { get; private set; }
        public int Origin { get; private set; }
        public int Destination { get; private set; }

        public PassengerState PassengerState { get; private set; }

        public DateTime HallCallTime { get; private set; }
        public DateTime CarBoardTime { get; private set; }
        public DateTime CarAlightTime { get; private set; }

        public Direction Direction
        {
            get
            {
                if (this.Destination < this.Origin)
                {
                    return Direction.Down;
                }

                if (this.Destination > this.Origin)
                {
                    return Direction.Up;
                }

                return Direction.None;
            }
        }

        public PassengerGroup(int size, int origin, int destination)
        {
            if (origin == destination)
            {
                throw new SimulationAssumptionException("Origin and Destination floors will not be equal");
            }
            this.Size = size;
            this.Origin = origin;
            this.Destination = destination;

            this.PassengerState = PassengerState.Unborn;
        }

        public void changeState(PassengerState newState, DateTime currentTime)
        {
            this.PassengerState = newState;

            switch (newState)
            {
                case PassengerState.Waiting: HallCallTime = currentTime;
                    break;
                case PassengerState.InTransit: CarBoardTime = currentTime;
                    break;
                case PassengerState.Arrived: CarAlightTime = currentTime;
                    break;
                default:
                    //Todo ...
                    break;
            }
        }
    }
}
