using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Agenda;
using ElevatorSimulator.DataStructures;
using ElevatorSimulator.Tools;

namespace ElevatorSimulator.PhysicalDomain
{
    class Car : ICar
    {
        private readonly ShaftData shaftData;
        private List<PassengerGroup> passengers = new List<PassengerGroup>();
        private CallAllocationList allocatedCalls = new CallAllocationList();

        protected readonly Shaft shaft;

        private CarAttributes CarAttributes { get; set; }

        public CarState State { get; private set; }

        private int _TotalNumberOfAllocations = 0;
        public int TotalNumberOfAllocations
        {
            get
            {
                return _TotalNumberOfAllocations;
            }
        }

        public int NumberOfCalls
        {
            get
            {
                return this.allocatedCalls.allCalls.Count();
            }
        }

        public int TotalCapacity
        {
            get
            {
                return this.CarAttributes.Capacity;
            }
        }

        public CallAllocationList CallAllocationList
        {
            get
            {
                return allocatedCalls;
            }
        }

        /// <summary>
        /// This is the list of floors currently occupied by the car
        /// i.e. if it is stopped at floor 5, this list will be { 5 }
        /// if it is moving from floor 5 to floor 6, this list will be { 5, 6 }
        /// </summary>
        public List<int> CurrentFloorsOccupied
        {
            get
            {
                var result = new List<int>();

                result.Add(this.State.Floor);

                if (this.State.Action == CarAction.Moving || this.State.Action == CarAction.Leaving)
                {
                    result.Add(this.State.Direction == Direction.Up ? this.State.Floor + 1 : this.State.Floor - 1);
                }

                return result;
            }
        }

        public int NumberOfPassengers
        {
            get
            {
                return passengers.Sum(c => c.Size);
            }
        }

        private int CapacityRemaining
        {
            get
            {
                return this.CarAttributes.Capacity - this.NumberOfPassengers;
            }
        }

        public Car(Shaft shaft, ShaftData data, CarAttributes attributes, int startFloor)
        {
            CarAttributes = attributes;

            this.shaft = shaft;
            this.shaftData = data;

            State = new CarState()
            {
                Action = CarAction.Stopped,
                Floor = startFloor,
                Direction = Direction.Up,
                InitialSpeed = 0,
                DoorsOpen = true
            };

            this.changeState(State);
        }

        /// <summary>
        /// Add a passenger group to the list of passengers in the car.
        /// </summary>
        /// <param name="newPassengers">Group to add</param>
        private void addPassengers(PassengerGroup newPassengers)
        {
            this.passengers.Add(newPassengers);
        }

        /// <summary>
        /// Remove passenger group from the car.
        /// </summary>
        /// <param name="passengersToRemove">Group to remove.</param>
        private void removePassengers(PassengerGroup passengersToRemove)
        {
            this.passengers.Remove(passengersToRemove);
        }

        /// <summary>
        /// Method to be called by the Shaft when another Car in the shaft has changed
        /// its state.
        /// </summary>
        public void otherCarStateHasChanged()
        {
            // Do nothing
        }

        public bool allocateHallCall(HallCall hallCall, int? deck = null)
        {
            //TODO
            allocatedCalls.addHallCall(hallCall, this.State);

            Simulation.logger.logLine(String.Format("{0}: Car {1}.0 Hall call allocated; {2}, {3}, {4}", Simulation.agenda.getCurrentSimTime().ToString(), this.shaftData.ShaftId, hallCall.Passengers.Size, hallCall.Passengers.Origin, hallCall.Passengers.Destination));

            if (this.State.Action == CarAction.Idle)
            {
                CarState newState = new CarState() { Action = CarAction.Stopped, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
                this.changeState(newState);
            }

            _TotalNumberOfAllocations++;

            return true;
        }

        public void changeState(CarState newCarState)
        {
            this.State = newCarState;
            Simulation.logger.logLine(String.Format("{0}: Car {1}.0, {2}, {3}, {4}, {5}, {6}", Simulation.agenda.getCurrentSimTime().ToString("dd/MM/yyyy HH:mm:ss.fff"), this.shaftData.ShaftId, this.State.Action, this.State.Direction, this.State.Floor, this.State.InitialSpeed, this.NumberOfPassengers));
            this.updateAgenda();
        }

        private void updateAgenda()
        {
            switch (this.State.Action)
            {
                case CarAction.Idle:
                    updateAgendaHelper_IdleState();
                    break;
                case CarAction.Stopped:
                    updateAgendaHelper_StoppedState();
                    break;
                case CarAction.Unloading:
                    updateAgendaHelper_UnloadingState();
                    break;
                case CarAction.Loading:
                    updateAgendaHelper_LoadingState();
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
                case CarAction.Stopping:
                    updateAgendaHelper_StoppingState();
                    break;
                default:
                    //exception~?
                    break;

            }
        }

        private void updateAgendaHelper_IdleState()
        {
            if (!allocatedCalls.isEmpty())
            {
                // Change state of car to loading
                CarState newState = new CarState() { Action = CarAction.Stopped, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
                this.changeState(newState);
            }
        }

        private void updateAgendaHelper_StoppedState()
        {
            CarState newState;

            // Can we become idle
            if (allocatedCalls.isEmpty())
            {
                newState = new CarState() { Action = CarAction.Idle, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
                this.changeState(newState);
                return;
            }

            // Anyone to unload?
            if (this.allocatedCalls.getCarCallsForFloor(this.State.Floor).Count > 0)
            {

                // Open doors if not already.
                if (!this.State.DoorsOpen)
                {
                    newState = new CarState() { Action = CarAction.DoorsOpening, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
                    this.changeState(newState);
                    return;
                }
                // Begin unloading
                newState = new CarState() { Action = CarAction.Unloading, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
                this.changeState(newState);
                return;
            }
            
            // Get next call (all car calls for current floor processed at this point)
            var nextCall = this.allocatedCalls.getNextCall(this.State);

            if (this.State.Direction == Direction.Up)
            {
                if (nextCall.CallLocation < this.State.Floor
                    || nextCall.CallLocation == this.State.Floor && nextCall.CallDirection == Direction.Down)
                {
                    var newDirection = this.State.Direction == Direction.Down ? Direction.Up : Direction.Down;
                    newState = new CarState() { Action = CarAction.Reversing, Direction = newDirection, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
                    this.changeState(newState);
                    return;
                }
            }

            if (this.State.Direction == Direction.Down)
            {
                if (nextCall.CallLocation > this.State.Floor
                    || nextCall.CallLocation == this.State.Floor && nextCall.CallDirection == Direction.Up)
                {
                    var newDirection = this.State.Direction == Direction.Down ? Direction.Up : Direction.Down;
                    newState = new CarState() { Action = CarAction.Reversing, Direction = newDirection, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
                    this.changeState(newState);
                    return;
                }
            }

            if (nextCall.CallLocation == this.State.Floor)
            {
                // Open doors if not already.
                if (!this.State.DoorsOpen)
                {
                    newState = new CarState() { Action = CarAction.DoorsOpening, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
                    this.changeState(newState);
                    return;
                }

                // Load passengers
                newState = new CarState() { Action = CarAction.Loading, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
                this.changeState(newState);
                return;
            }

            if (this.State.DoorsOpen)
            {
                newState = new CarState() { Action = CarAction.DoorsClosing, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
                this.changeState(newState);
                return;
            }

            newState = new CarState() { Action = CarAction.Leaving, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
            this.changeState(newState);
        }


        private void updateAgendaHelper_UnloadingState()
        {
            var carCallsForFloor = allocatedCalls.getCarCallsForFloor(this.State.Floor);

            // count passengers to alight
            int passengerCount = 0;
            foreach (Call c in carCallsForFloor)
            {
                passengerCount += c.Passengers.Size;
            }

            // calculate total and mean alighting times for passengers
            double totalAlightTimeSeconds = this.CarAttributes.PassengerAlightTime * passengerCount;
            double averageAlightTimeSeconds = 0.5 * this.CarAttributes.PassengerAlightTime * (passengerCount + 1);

            DateTime timeOfAlight = Simulation.agenda.getCurrentSimTime().AddSeconds(averageAlightTimeSeconds);

            // alight passengers
            foreach (Call c in carCallsForFloor)
            {
                PassengerGroup p = c.Passengers;
                this.removePassengers(p);
                this.allocatedCalls.removeCall(c); // The call has been served
                p.changeState(PassengerState.Arrived, timeOfAlight);

                Simulation.logger.logLine(string.Format("    Unload group {0}, {1}, {2}", p.Size, p.Origin, p.Destination));
            }

            // Place an event on the agenda to fire when the passengers have finished alighting
            CarState newState = new CarState() { Action = CarAction.Stopped, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
            CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(totalAlightTimeSeconds), newState);
            Simulation.agenda.addAgendaEvent(newEvent);
        }


        private void updateAgendaHelper_LoadingState()
        {
            var hallCallsForFloor = allocatedCalls.getHallCallsForFloor(this.State.Floor, this.State.Direction).FindAll(a => a.CallDirection == this.State.Direction).OrderBy(c => c.Passengers.HallCallTime);

            double boardingTimeSoFar = 0;

            foreach (HallCall callToBoard in hallCallsForFloor)
            {
                PassengerGroup passengersToBoard = callToBoard.Passengers;

                if (passengersToBoard.Size <= this.CapacityRemaining)
                {
                    this.addPassengers(passengersToBoard);
                    this.allocatedCalls.removeCall(callToBoard);
                    this.allocatedCalls.addCarCall(new CarCall(passengersToBoard), this.State);
                    passengersToBoard.changeState(PassengerState.InTransit, Simulation.agenda.getCurrentSimTime().AddSeconds(boardingTimeSoFar));
                    boardingTimeSoFar += this.CarAttributes.PassengerBoardTime * passengersToBoard.Size;

                    Simulation.logger.logLine(string.Format("    Load group {0}, {1}, {2}", passengersToBoard.Size, passengersToBoard.Origin, passengersToBoard.Destination));
                }
                else
                {
                    this.allocatedCalls.postponeHallCall(callToBoard);
                }
            }

            CarState newState = new CarState() { Action = CarAction.Stopped, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
            CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(boardingTimeSoFar), newState);
            Simulation.agenda.addAgendaEvent(newEvent);
        }

        private void updateAgendaHelper_ReversingState()
        {
            // The car's direction will already be changed before entering the reversing state
            // Place event on agenda to fire when reversing action has completed
            CarState newState = new CarState() { Action = CarAction.Stopped, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
            CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(this.CarAttributes.DirectionChangeTime), newState);
            Simulation.agenda.addAgendaEvent(newEvent);

            allocatedCalls.reverseListsDirection();
        }

        private void updateAgendaHelper_DoorsClosingState()
        {
            // Place event on agenda to fire when doors have finished closing
            CarState newState = new CarState() { Action = CarAction.Stopped, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = false };
            CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(this.CarAttributes.DoorsCloseTime), newState);
            Simulation.agenda.addAgendaEvent(newEvent);
        }

        private void updateAgendaHelper_LeavingState()
        {
            int nextFloor = this.State.Direction == Direction.Up ? this.State.Floor + 1 : this.State.Floor - 1;
            double nextFloorDistance = getInterfloorDistance(this.State.Floor, this.State.Direction);
            double nextFloorDecisionPointSpeed;
            double nextFloorDecisionPointTime;

            CarMotionMaths.CalculateDecisionPointSpeedAndTime(this.CarAttributes, this.State, nextFloorDistance, out nextFloorDecisionPointSpeed, out nextFloorDecisionPointTime);

            // Place event on agenda to fire when car reaches decision point of next floor
            CarState newState = new CarState() { Action = CarAction.Moving, Direction = this.State.Direction, Floor = nextFloor, InitialSpeed = nextFloorDecisionPointSpeed, DoorsOpen = this.State.DoorsOpen };
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
            if (this.State.Floor == this.allocatedCalls.getNextCall(this.State).CallLocation)
            {
                // Put car in stopping state
                CarState newState = new CarState() { Action = CarAction.Stopping, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = this.State.InitialSpeed, DoorsOpen = this.State.DoorsOpen };
                this.changeState(newState);
                return;
            }

            int nextFloor = this.State.Direction == Direction.Up ? this.State.Floor + 1 : this.State.Floor - 1;
            double nextFloorDistance = (Math.Pow(this.State.InitialSpeed, 2) / (2 * this.CarAttributes.Deceleration)) + getInterfloorDistance(this.State.Floor, this.State.Direction);
            double nextFloorDecisionPointSpeed;
            double nextFloorDecisionPointTime;

            CarMotionMaths.CalculateDecisionPointSpeedAndTime(this.CarAttributes, this.State, nextFloorDistance, out nextFloorDecisionPointSpeed, out nextFloorDecisionPointTime);

            // Place event on agenda to fire when car reaches decision point of next floor
            CarState newCarState = new CarState() { Action = CarAction.Moving, Direction = this.State.Direction, Floor = nextFloor, InitialSpeed = nextFloorDecisionPointSpeed, DoorsOpen = this.State.DoorsOpen };
            CarStateChangeEvent newStateEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(nextFloorDecisionPointTime), newCarState);
            Simulation.agenda.addAgendaEvent(newStateEvent);

        }

        private void updateAgendaHelper_StoppingState()
        {
            // Begin stopping car

            // Calculate time taken to stop
            var stoppingTime = CarMotionMaths.StoppingTime(this.State.InitialSpeed, this.CarAttributes.Deceleration);

            // Place event on agenda to fire when car has stopped at floor
            CarState newState = new CarState() { Action = CarAction.Stopped, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = this.State.DoorsOpen };
            CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(stoppingTime), newState);
            Simulation.agenda.addAgendaEvent(newEvent);
        }

        private void updateAgendaHelper_DoorsOpeningState()
        {
            // Place an event on the agenda to fire when the doors have finished opening
            CarState newState = new CarState() { Action = CarAction.Stopped, Direction = this.State.Direction, Floor = this.State.Floor, InitialSpeed = 0, DoorsOpen = true};
            CarStateChangeEvent newEvent = new CarStateChangeEvent(this, Simulation.agenda.getCurrentSimTime().AddSeconds(this.CarAttributes.DoorsOpenTime), newState);
            Simulation.agenda.addAgendaEvent(newEvent);
        }

        public List<Call> getOrderedCalls()
        {
            return this.allocatedCalls.getOrderedCalls(this.State);
        }

    }
}
