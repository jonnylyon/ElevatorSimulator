using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Agenda;

namespace ElevatorSimulator.PhysicalDomain
{
    class Car : PassengerLocation, IEventOwner
    {
        private Shaft shaft;

        private CarState carState;

        private CallsList p1Calls;
        private CallsList p2Calls;
        private CallsList p3Calls;

        private CarStateChange nextAgendaEvent;

        private int capacity;

        private double maxSpeed; // in metres per second
        private double acceleration; // in metres per second squared; assumes linear acc'n
        private double deceleration; // in positive metres per second squared; assumes linear dec'n

        private double directionChangeTime; // in seconds
        private double passengerBoardTime; // in seconds
        private double passengerAlightTime; // in seconds

        private double doorsCloseTime; // in seconds
        private double doorsOpenTime; // in seconds

        public bool addPassengers(PassengerGroup newPassengers)
        {
            if (this.getNumberOfPassengers() <= this.capacity - newPassengers.getSize())
            {
                this.passengers.Add(newPassengers);

                return true;
            }
            else
            {
                return false;
            }
        }

        internal void setShaft(Shaft shaft)
        {
            this.shaft = shaft;

            this.carState = new CarState()
            {
                Floor = shaft.getBottomFloor(),
                Direction = Direction.Up
            };
        }

        internal void addHallCall(HallCall hallCall, DateTime currentTime)
        {
            if (this.carState.Direction == Direction.Up)
            {
                if (hallCall.getDirection() == Direction.Down)
                {
                    this.p2Calls.addCall(hallCall);
                }
                else if (hallCall.getFloor() > this.carState.Floor)
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
                if (hallCall.getDirection() == Direction.Up)
                {
                    this.p2Calls.addCall(hallCall);
                }
                else if (hallCall.getFloor() < this.carState.Floor)
                {
                    this.p1Calls.addCall(hallCall);
                }
                else
                {
                    this.p3Calls.addCall(hallCall);
                }
            }

            this.updateAgenda();
        }

        public void changeState(CarState newCarState)
        {
            this.carState = newCarState;
            this.updateAgenda();
        }

        private void updateAgenda()
        {
            if (!object.ReferenceEquals(this.nextAgendaEvent, null))
            {
                Agenda.Agenda.removeAgendaItem(this.nextAgendaEvent);
            }

            if (this.carState.Action == CarAction.Idle)
            {
                if (p1Calls.isEmpty() && p2Calls.isEmpty() && p3Calls.isEmpty())
                {
                    // do nothing
                    // remain idle
                }
                else
                {
                    Direction directionToMoveIn;

                    // Determine whether to move up or down, based on contents of p1Calls list
                    if (p1Calls.getHighestCall().getFloor() > this.carState.Floor)
                    {
                        directionToMoveIn = Direction.Up;
                    }
                    else
                    {
                        directionToMoveIn = Direction.Down;
                    }

                    // If direction of car changes, we need to wait for this to happen
                    if (directionToMoveIn != this.carState.Direction)
                    {
                        // Change state of car to reversing
                        CarState newState = new CarState() { Action = CarAction.Reversing, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                        this.changeState(newState);
                    }
                    else
                    {
                        // Change state of car to loading
                        CarState newState = new CarState() { Action = CarAction.Loading, Direction = directionToMoveIn, Floor = this.carState.Floor, InitialSpeed = 0 };

                        this.changeState(newState);
                    }
                }
            }

            if (this.carState.Action == CarAction.Loading)
            {
                if (p1Calls.isEmpty())
                {
                    if (p2Calls.isEmpty() && p2Calls.isEmpty())
                    {
                        // do nothing

                        // Change state of car to idle
                        CarState newState = new CarState() { Action = CarAction.Idle, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                        this.changeState(newState);
                    }
                    else
                    {
                        // Change state of car to reversing
                        CarState intermediateState = new CarState() { Action = CarAction.Reversing, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                        this.changeState(intermediateState);
                    }
                }
                else
                {
                    // TODO: Need to test if there are passengers to load or unload
                    if (false) // TODO: If there are passengers to load or unload
                    {
                        // TODO: Load or Unload passengers

                        // TODO: Place event on agenda to fire when passengers have finished loading and unloading
                    }
                    else
                    {
                        // Change state of car to departing (start closing doors)
                        CarState newState = new CarState() { Action = CarAction.DoorsClosing, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                        this.changeState(newState);
                    }
                }

            }

            if (this.carState.Action == CarAction.Reversing)
            {
                Direction newDirection = this.carState.Direction == Direction.Up ? Direction.Down : Direction.Up;

                // Place event on agenda to fire when reversing action has completed
                CarState newState = new CarState() { Action = CarAction.Loading, Direction = newDirection, Floor = this.carState.Floor, InitialSpeed = 0 };

                CarStateChange newEvent = new CarStateChange(this, Agenda.Agenda.getCurrentTime().AddSeconds(this.directionChangeTime), newState);

                Agenda.Agenda.addAgendaItem(newEvent);
            }

            if (this.carState.Action == CarAction.DoorsClosing)
            {
                // Place event on agenda to fire when doors have finished closing
                CarState newState = new CarState() { Action = CarAction.Leaving, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                CarStateChange newEvent = new CarStateChange(this, Agenda.Agenda.getCurrentTime().AddSeconds(this.doorsCloseTime), newState);

                Agenda.Agenda.addAgendaItem(newEvent);
            }

            if (this.carState.Action == CarAction.Leaving)
            {
                // TODO: Place event on agenda to fire when car reaches decision point of next floor

                // Requires working out which floor is the next floor (current floor +/- 1 depending on direction)
                // and what the speed will be at the time that we reach the decision point (function of
                // interfloor distance, rate of acceleration and maximum speed of car)
            }

            if (this.carState.Action == CarAction.Moving)
            {
                if (this.carState.Floor == this.getNextCall().getFloor())
                {
                    // Start stopping car
                    // TODO: Place event on agenda to fire when car has stopped at floor

                    // Requires working out how long it will take to decelerate (function of deceleration rate)
                }
                else
                {
                    // TODO: Place event on agenda to fire when car reaches decision point of next floor

                    // Requires working out which floor is the next floor (current floor +/- 1 depending on direction)
                    // and what the speed will be at the time that we reach the decision point (function of
                    // current speed, interfloor distance, rate of acceleration and maximum speed of car)
                    // This is quite complicated, I think.
                }
            }

            if (this.carState.Action == CarAction.DoorsOpening)
            {
                // Place an event on the agenda to fire when the doors have finished opening
                CarState newState = new CarState() { Action = CarAction.Loading, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                CarStateChange newEvent = new CarStateChange(this, Agenda.Agenda.getCurrentTime().AddSeconds(this.doorsOpenTime), newState);

                Agenda.Agenda.addAgendaItem(newEvent);
            }
        }

        private Call getNextCall()
        {
            if (this.p1Calls.count() > 0)
            {
                return this.p1Calls.getNextCall(this.carState.Floor, this.carState.Direction);
            }
            else if (this.p2Calls.count() > 0)
            {
                if (this.carState.Direction == Direction.Up)
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
                if (this.carState.Direction == Direction.Up)
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
