using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Agenda;

namespace ElevatorSimulator.PhysicalDomain
{
    class Car : IEventOwner
    {
        private Shaft shaft;
        private List<PassengerGroup> passengers = new List<PassengerGroup>();

        private CallsList p1Calls = new CallsList(); // Pass 1 (current direction)
        private CallsList p2Calls = new CallsList(); // Pass 2 (opposite direction, reverse once)
        private CallsList p3Calls = new CallsList(); // Pass 3 (opposite direction, reverse twice)

        private int capacity;

        private double maxSpeed = 5; // in metres per second
        private double acceleration = 2; // in metres per second squared; assumes linear acc'n
        private double deceleration = 4; // in positive metres per second squared; assumes linear dec'n

        private double directionChangeTime = 1; // in seconds
        private double passengerBoardTime = 10; // in seconds
        private double passengerAlightTime = 10; // in seconds

        private double doorsCloseTime = 3; // in seconds
        private double doorsOpenTime = 3; // in seconds

        public CarState State { get; private set; }

        public int getNumberOfPassengers()
        {
            int count = 0;

            foreach (PassengerGroup p in this.passengers)
            {
                count += p.Size;
            }

            return count;
        }

        public int FreeSpace
        {
            get
            {
                return this.capacity - this.passengers.Count();
            }
        }

        public bool addPassengers(PassengerGroup newPassengers)
        {
            if (this.getNumberOfPassengers() <= this.capacity - newPassengers.Size)
            {
                this.passengers.Add(newPassengers);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void removePassengers(PassengerGroup passengersToRemove)
        {
            this.passengers.Remove(passengersToRemove);
        }

        internal void setShaft(Shaft shaft)
        {
            this.shaft = shaft;

            State = new CarState()
            {
                Action = CarAction.Loading,
                Floor = shaft.getBottomFloor(),
                Direction = Direction.Up,
                InitialSpeed = 0
            };

            this.changeState(State);
        }

        internal void allocateHallCall(HallCall hallCall)
        {
            if (this.State.Direction == Direction.Up)
            {
                if (hallCall.CallDirection == Direction.Down)
                {
                    this.p2Calls.addCall(hallCall);
                }
                else if (hallCall.Passengers.Origin > this.State.Floor)
                {
                    this.p1Calls.addCall(hallCall);
                }
                else
                {
                    this.p3Calls.addCall(hallCall);
                }
            }
            else
            {
                if (hallCall.CallDirection == Direction.Up)
                {
                    this.p2Calls.addCall(hallCall);
                }
                else if (hallCall.Passengers.Origin < this.State.Floor)
                {
                    this.p1Calls.addCall(hallCall);
                }
                else
                {
                    this.p3Calls.addCall(hallCall);
                }
            }

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

        public CallsList getCarCallsForFloor(int floor)
        {
            CallsList result = new CallsList();

            foreach (Call c in this.p1Calls)
            {
                if (c is CarCall && c.Passengers.Destination == floor)
                {
                    result.addCall(c);
                }
            }

            return result;
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
            if (!(p1Calls.isEmpty() && p2Calls.isEmpty() && p3Calls.isEmpty()))
            {
                // Change state of car to loading
                CarState newState = new CarState() { Action = CarAction.Loading, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                this.changeState(newState);
            }
        }

        private void updateAgenderHelper_LoadingState()
        {
            CallsQueue queueToBoard = Simulation.getQueue(this.State.Floor, this.State.Direction, this.FreeSpace, this);

            // Can we become idle
            if (this.p1Calls.isEmpty() && this.p2Calls.isEmpty() && this.p3Calls.isEmpty() && queueToBoard.isEmpty())
            {
                CarState newState = new CarState() { Action = CarAction.Idle, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                this.changeState(newState);
                return;
            }

            bool nextCallIsInCurrentDirection = false;
            Call nextCall = this.getNextCall();

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
            if ((this.p1Calls.isEmpty() && queueToBoard.isEmpty()) && (this.p2Calls.isEmpty() || !nextCallIsInCurrentDirection))
            {
                // Change state of car to reversing
                CarState intermediateState = new CarState() { Action = CarAction.Reversing, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                this.changeState(intermediateState);
                return;
            }

            CallsList carCallsForFloor = this.getCarCallsForFloor(this.State.Floor);

            // Can we depart
            if (carCallsForFloor.isEmpty() && queueToBoard.isEmpty())
            {
                // Change state of car to departing (start closing doors)
                CarState newState = new CarState() { Action = CarAction.DoorsClosing, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0 };
                this.changeState(newState);
                return;
            }

            if (!carCallsForFloor.isEmpty())
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

            this.p1Calls = this.p2Calls;
            this.p2Calls = this.p3Calls;
            this.p3Calls = new CallsList();
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
            double nextFloorDistance = shaft.getInterfloorDistance(this.State.Floor, this.State.Direction);
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

        private void updateAgendaHelper_MovingState()
        {
            if (this.State.Floor == this.getNextCall().getElevatorDestination())
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
            double nextFloorDistance = (Math.Pow(this.State.InitialSpeed, 2) / (2 * this.deceleration)) + shaft.getInterfloorDistance(this.State.Floor, this.State.Direction);
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


        private void removeP1Calls(int floor, Direction direction)
        {
            CallsList callsToRemove = new CallsList();

            foreach (Call call in this.p1Calls)
            {
                if (call.getElevatorDestination() == floor && (call.CallDirection == direction))
                {
                    callsToRemove.addCall(call);
                }
            }

            foreach (Call call in callsToRemove)
            {
                this.p1Calls.removeCall(call);
            }
        }

        private Call getNextCall()
        {
            if (this.p1Calls.count() > 0)
            {
                return this.p1Calls.getNextCallInCurrentDirection(this.State.Floor, this.State.Direction);
            }
            else if (this.p2Calls.count() > 0)
            {
                if (this.State.Direction == Direction.Up)
                {
                    return this.p2Calls.getHighestCall();
                }
                else
                {
                    return this.p2Calls.getLowestCall();
                }
            }
            else if (this.p3Calls.count() > 0)
            {
                if (this.State.Direction == Direction.Up)
                {
                    return this.p3Calls.getLowestCall();
                }
                else
                {
                    return this.p3Calls.getHighestCall();
                }
            }
            else
            {
                return null;
            }
        }
    }
}
