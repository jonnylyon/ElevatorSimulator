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

        private PassengerState passengerState;

        private DateTime hallCallTime;
        private DateTime carBoardTime;
        private DateTime carAlightTime;

        public PassengerGroup(int size, int origin, int destination)
        {
            if (origin == destination)
            {
                throw new SimulationAssumptionException("Origin and Destination floors will not be equal");
            }
            this.Size = size;
            this.Origin = origin;
            this.Destination = destination;

            this.passengerState = PassengerState.Unborn;
        }

        public void changeState(PassengerState newState, DateTime currentTime)
        {
            this.passengerState = newState;

            switch (newState)
            {
                case PassengerState.Waiting: hallCallTime = currentTime;
                    break;
                case PassengerState.InTransit: carBoardTime = currentTime;
                    break;
                case PassengerState.Arrived: carAlightTime = currentTime;
                    break;
                default:
                    //Todo ...
                    break;
            }
        }
    }
}
