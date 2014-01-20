using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.Exceptions;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.PhysicalDomain
{
    class PassengerGroup : Agenda.IEventOwner
    {
        private int size;
        private int origin;
        private int destination;

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
            this.size = size;
            this.origin = origin;
            this.destination = destination;

            this.passengerState = PassengerState.Unborn;
        }

        public void changeState(PassengerState newState, DateTime currentTime)
        {
            this.passengerState = newState;

            if (newState == PassengerState.Waiting)
            {
                hallCallTime = currentTime;
            }
            else if (newState == PassengerState.InTransit)
            {
                carBoardTime = currentTime;
            }
            else if (newState == PassengerState.Arrived)
            {
                carAlightTime = currentTime;
            }
        }

        public int getSize()
        {
            return this.size;
        }

        public int getOrigin()
        {
            return this.origin;
        }

        public int getDestination()
        {
            return this.destination;
        }
    }
}
