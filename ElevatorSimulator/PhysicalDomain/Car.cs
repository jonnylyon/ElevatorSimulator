using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Agenda;
using ElevatorSimulator.DataStructures;

namespace ElevatorSimulator.PhysicalDomain
{
    class Car : IEventOwner
    {
        private readonly ShaftData shaftData;
        private List<PassengerGroup> passengers = new List<PassengerGroup>();
        private CallAllocationList allocatedCalls = new CallAllocationList();

        private int capacity = 18;

        private double maxSpeed = 5; // in metres per second
        private double acceleration = 2; // in metres per second squared; assumes linear acc'n
        private double deceleration = 4; // in positive metres per second squared; assumes linear dec'n

        private double directionChangeTime = 1; // in seconds
        private double passengerBoardTime = 10; // in seconds
        private double passengerAlightTime = 10; // in seconds

        private double doorsCloseTime = 3; // in seconds
        private double doorsOpenTime = 3; // in seconds

        private int numberOfPassengers;

        public CarState State { get; private set; }

        public Car(ShaftData data)
        {
            this.shaftData = data;

            State = new CarState()
            {
                Action = CarAction.Loading,
                Floor = shaftData.BottomFloor,
                Direction = Direction.Up,
                InitialSpeed = 0
            };

            this.changeState(State);
        }
        
        /// <summary>
        /// Add a passenger group to the list of passengers in the car.
        /// Increment the total number of passengers in the car by the group size.
        /// </summary>
        /// <param name="newPassengers">Group to add</param>
        /// <returns>True if successfully added. False otherwise.</returns>
        public bool addPassengers(PassengerGroup newPassengers)
        {
            if ((capacity - numberOfPassengers) < newPassengers.Size)
            {
                return false;
            }

            this.passengers.Add(newPassengers);
            numberOfPassengers += newPassengers.Size;
            return true; 
        }

        /// <summary>
        /// Remove passenger group from the car.
        /// Decrement total number of passengers by group size.
        /// </summary>
        /// <param name="passengersToRemove">Group to remove.</param>
        public void removePassengers(PassengerGroup passengersToRemove)
        {
            this.passengers.Remove(passengersToRemove);
            numberOfPassengers -= passengersToRemove.Size;
        }

        internal void allocateHallCall(HallCall hallCall)
        {
            //TODO
            allocatedCalls.addHallCall(hallCall, this.State);
          
            if (this.State.Action == CarAction.Idle)
            {
                CarState newState = new CarState() { Action = CarAction.Loading, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                this.changeState(newState);
            }
        }

        public void changeState(CarState newCarState)
        {
            this.State = newCarState;
            Console.WriteLine("{0}: {1}, {2}, {3}, {4}", Simulation.agenda.getCurrentSimTime().ToString(), this.State.Action, this.State.Direction, this.State.Floor, this.State.InitialSpeed);
            this.updateAgenda();
        }

        private void updateAgenda()
        {
            switch (this.State.Action)
            {
                case CarAction.Idle:
                    updateAgenderHelper_IdleState();
                    break;
                case CarAction.Loading:
                    updateAgenderHelper_LoadingState();
                    break;
                case CarAction.Moving:
                    updateAgendaHelper_MovingState();
                    break;
                case CarAction.Reversing:
                    updateAgendaHelper_ReversingState();
                    break;
                case CarAction.Leaving:
                    updateAgendaHelper_LeavingState();
                    break;
                case CarAction.DoorsOpening:
                    updateAgendaHelper_DoorsOpeningState();
                    break;
                case CarAction.DoorsClosing:
                    updateAgendaHelper_DoorsClosingState();
                    break;
                default:
                    //exception~?
                    break;

            }
        }

        private void updateAgenderHelper_IdleState()
        {
            if (!allocatedCalls.isEmpty())
            {
                // Change state of car to loading
                CarState newState = new CarState() { Action = CarAction.Loading, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                this.changeState(newState);
            }
        }

        private void updateAgenderHelper_LoadingState()
        {
            CallsQueue queueToBoard = Simulation.getQueue(this.State.Floor, this.State.Direction, (capacity - numberOfPassengers), this);

            // Can we become idle
            if (allocatedCalls.isEmpty() && queueToBoard.isEmpty())
            {
                CarState newState = new CarState() { Action = CarAction.Idle, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                this.changeState(newState);
                return;
            }

            bool nextCallIsInCurrentDirection = false;
            Call nextCall = this.allocatedCalls.getNextCall(this.State);

            if (!object.ReferenceEquals(nextCall, null))
            {
                if (this.State.Direction == Direction.Up && nextCall.getElevatorDestination() > this.State.Floor)
                {
                    nextCallIsInCurrentDirection = true;
                }
                else if (this.State.Direction == Direction.Down && nextCall.getElevatorDestination() < this.State.Floor)
                {
                    nextCallIsInCurrentDirection = true;
                }
            }

            // Should we reverse
            if (!nextCallIsInCurrentDirection)
            {
                // Change state of car to reversing
                CarState intermediateState = new CarState() { Action = CarAction.Reversing, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                this.changeState(intermediateState);
                return;
            }

            var carCallsForFloor = allocatedCalls.getCarCallsForFloor(this.State.Floor);

            // Can we depart (no-one getting on or off at this floor)
            if (!carCallsForFloor.Any() && queueToBoard.isEmpty())
            {
                // Change state of car to departing (start closing doors)
                CarState newState = new CarState() { Action = CarAction.DoorsClosing, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                this.changeState(newState);
                return;
            }

            // TODO i think this doesnt account for when both hall call and car call are relevant?
            if (carCallsForFloor.Any())
            {
                // count passengers to alight
                int passengerCount = 0;
                foreach (Call c in carCallsForFloor)
                {
                    passengerCount += c.Passengers.Size;
                }

                // calculate total and mean alighting times for passengers
                double totalAlightTimeSeconds = this.passengerAlightTime * passengerCount;
                double averageAlightTimeSeconds = 0.5 * this.passengerAlightTime * (passengerCount + 1);

                DateTime timeOfAlight = Simulation.agenda.getCurrentSimTime().AddSeconds(averageAlightTimeSeconds);

                // alight passengers
                foreach (Call c in carCallsForFloor)
                {
                    PassengerGroup p = c.Passengers;
                    this.removePassengers(p);
                    p.changeState(PassengerState.Arrived, timeOfAlight);
                }

                // Place an event on the agenda to fire when the passengers have finished alighting
                CarState newState = new CarState() { Action = CarAction.Loading, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(totalAlightTimeSeconds), newState);

                Simulation.agenda.addAgendaEvent(newEvent);
            }
            else if (!queueToBoard.isEmpty())
            {
                HallCall callToBoard = queueToBoard.deQueue() as HallCall;
                PassengerGroup passengersToBoard = callToBoard.Passengers;

                double boardingTime = this.passengerBoardTime * passengersToBoard.Size;

                this.addPassengers(passengersToBoard);
                passengersToBoard.changeState(PassengerState.InTransit, Simulation.agenda.getCurrentSimTime());

                // Place an event on the agenda to fire when the passengers have finished alighting
                CarState newState = new CarState() { Action = CarAction.Loading, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };

                CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(boardingTime), newState);

                Simulation.agenda.addAgendaEvent(newEvent);
            }

        }

        private void updateAgendaHelper_ReversingState()
        {
            Direction newDirection = this.State.Direction == Direction.Up ? Direction.Down : Direction.Up;

            // Place event on agenda to fire when reversing action has completed
            CarState newState = new CarState() { Action = CarAction.Loading, Direction = newDirection, Floor = this.State.Floor, InitialSpeed = 0 };

            CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(this.directionChangeTime), newState);

            Simulation.agenda.addAgendaEvent(newEvent);
            allocatedCalls.reverseListsDirection();
        }

        private void updateAgendaHelper_DoorsClosingState()
        {
            // Place event on agenda to fire when doors have finished closing
            CarState newState = new CarState() { Action = CarAction.Leaving, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
            CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(this.doorsCloseTime), newState);
            Simulation.agenda.addAgendaEvent(newEvent);
        }

        private void updateAgendaHelper_LeavingState()
        {
            int nextFloor = this.State.Direction == Direction.Up ? this.State.Floor + 1 : this.State.Floor - 1;
            double nextFloorDistance = getInterfloorDistance(this.State.Floor, this.State.Direction);
            double nextFloorDecisionPointSpeed;
            double nextFloorDecisionPointTime;

            // First, using linear acc'n and dec'n and assuming no maximum speed, work out the parameters
            // of the point Q at which we must finally decide whether or not to stop at the next floor
            // (the decision point of the next floor)

            double Q_D; // The distance to point Q from here
            double Q_V; // The speed that this car will have reached by point Q
            double Q_T; // The time taken to get to point Q

            Q_D = (this.deceleration * nextFloorDistance) / (this.acceleration + this.deceleration);
            Q_V = Math.Sqrt(2 * this.acceleration * Q_D);
            Q_T = (1 / this.acceleration) * Q_V;

            // If Q_V is less than or equal to the maximum speed, then Q is the decision point for whether or
            // not to stop at the next floor.
            // Otherwise, we must calculate the decision point R by working out the point P at which we reach
            // our maximum speed when accelerating from our current position.

            if (Q_V > this.maxSpeed)
            {
                // note that P_V would be equal to the max speed of the car
                double P_D; // The distance to point P from here
                double P_T; // The time taken to get to point P

                P_T = this.maxSpeed / this.acceleration;
                P_D = 0.5 * this.acceleration * Math.Pow(P_T, 2);
                // note that R_V = P_V = the max speed of the car
                double R_D; // The distance to point R from here
                double R_T; // The time taken to get to point R from here

                R_D = nextFloorDistance - (Math.Pow(this.maxSpeed, 2) / (2 * this.deceleration));
                R_T = P_T + ((R_D - P_D) / this.maxSpeed);

                nextFloorDecisionPointSpeed = this.maxSpeed;
                nextFloorDecisionPointTime = R_T;
            }
            else
            {
                nextFloorDecisionPointSpeed = Q_V;
                nextFloorDecisionPointTime = Q_T;
            }

            // Place event on agenda to fire when car reaches decision point of next floor

            CarState newState = new CarState() { Action = CarAction.Moving, Direction = this.State.Direction, Floor = nextFloor, InitialSpeed = nextFloorDecisionPointSpeed };

            CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(nextFloorDecisionPointTime), newState);

            Simulation.agenda.addAgendaEvent(newEvent);
        }

        /// <summary>
        /// Get the distance in metres between the specified floor and the
        /// floor immediately above or below it
        /// </summary>
        /// <param name="floor">The specified floor</param>
        /// <param name="direction">The specified direction</param>
        /// <returns>Interfloor Distance in metres</returns>
        private double getInterfloorDistance(int floor, Direction direction)
        {
            // Current implementation assumes that all interfloor distances are the same
            return this.shaftData.InterfloorDistance;
        }

        private void updateAgendaHelper_MovingState()
        {
            if (this.State.Floor == this.allocatedCalls.getNextCall(this.State).getElevatorDestination())
            {
                // Begin stopping car

                // Calculate time taken to stop
                var stoppingTime = this.State.InitialSpeed / this.deceleration;

                // Place event on agenda to fire when car has stopped at floor
                CarState newState = new CarState() { Action = CarAction.DoorsOpening, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(stoppingTime), newState);
                Simulation.agenda.addAgendaEvent(newEvent);

                return;
            }

            int nextFloor = this.State.Direction == Direction.Up ? this.State.Floor + 1 : this.State.Floor - 1;
            double nextFloorDistance = (Math.Pow(this.State.InitialSpeed, 2) / (2 * this.deceleration)) + getInterfloorDistance(this.State.Floor, this.State.Direction);
            double nextFloorDecisionPointSpeed;
            double nextFloorDecisionPointTime;

            // First, using linear acc'n and dec'n and assuming no maximum speed, work out the parameters
            // of the point Q at which we must finally decide whether or not to stop at the next floor
            // (the decision point of the next floor)

            double Q_D; // The distance to point Q from here
            double Q_V; // The speed that this car will have reached by point Q
            double Q_T; // The time taken to get to point Q

            Q_D = ((2 * this.deceleration * nextFloorDistance) - Math.Pow(this.State.InitialSpeed, 2)) / (2 * (this.acceleration + this.deceleration));
            Q_V = Math.Sqrt(Math.Pow(this.State.InitialSpeed, 2) + (2 * this.acceleration * Q_D));
            Q_T = (1 / this.acceleration) * (Q_V - this.State.InitialSpeed);

            // If Q_V is less than or equal to the maximum speed, then Q is the decision point for whether or
            // not to stop at the next floor.
            // Otherwise, we must calculate the decision point R by working out the point P at which we reach
            // our maximum speed when accelerating from our current position.

            if (Q_V > this.maxSpeed)
            {
                // note that P_V would be equal to the max speed of the car
                double P_D; // The distance to point P from here
                double P_T; // The time taken to get to point P

                P_T = (this.maxSpeed - this.State.InitialSpeed) / this.acceleration;
                P_D = this.State.InitialSpeed + (0.5 * this.acceleration * Math.Pow(P_T, 2));

                // note that R_V = P_V = the max speed of the car
                double R_D; // The distance to point R from here
                double R_T; // The time taken to get to point R from here

                R_D = nextFloorDistance - (Math.Pow(this.maxSpeed, 2) / (2 * this.deceleration));
                R_T = P_T + ((R_D - P_D) / this.maxSpeed);

                nextFloorDecisionPointSpeed = this.maxSpeed;
                nextFloorDecisionPointTime = R_T;
            }
            else
            {
                nextFloorDecisionPointSpeed = Q_V;
                nextFloorDecisionPointTime = Q_T;
            }

            // Place event on agenda to fire when car reaches decision point of next floor
            CarState newCarState = new CarState() { Action = CarAction.Moving, Direction = this.State.Direction, Floor = nextFloor, InitialSpeed = nextFloorDecisionPointSpeed };
            CarStateChangeEvent newStateEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(nextFloorDecisionPointTime), newCarState);
            Simulation.agenda.addAgendaEvent(newStateEvent);

        }

        private void updateAgendaHelper_DoorsOpeningState()
        {
            // Place an event on the agenda to fire when the doors have finished opening
            CarState newState = new CarState() { Action = CarAction.Loading, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
            CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(this.doorsOpenTime), newState);
            Simulation.agenda.addAgendaEvent(newEvent);
        }       
    }
}
