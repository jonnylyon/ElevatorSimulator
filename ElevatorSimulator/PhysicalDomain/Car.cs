﻿using System;
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

        public CarState carState;

        private CallsList p1Calls = new CallsList();
        private CallsList p2Calls = new CallsList();
        private CallsList p3Calls = new CallsList();

        private int capacity;

        private double maxSpeed = 5; // in metres per second
        private double acceleration = 2; // in metres per second squared; assumes linear acc'n
        private double deceleration = 4; // in positive metres per second squared; assumes linear dec'n

        private double directionChangeTime = 1; // in seconds
        private double passengerBoardTime = 10; // in seconds
        private double passengerAlightTime = 10; // in seconds

        private double doorsCloseTime = 3; // in seconds
        private double doorsOpenTime = 3; // in seconds

        private bool hasWaitedOnFloor = false; // this is a temporary variable that will be deleted eventually, using for test purposes

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

            carState = new CarState()
            {
                Action = CarAction.Loading,
                Floor = shaft.getBottomFloor(),
                Direction = Direction.Up,
                InitialSpeed = 0
            };

            this.changeState(carState);
        }

        internal void addHallCall(HallCall hallCall)
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

            if (this.carState.Action == CarAction.Idle)
            {
                CarState newState = new CarState() { Action = CarAction.Loading, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                this.changeState(newState);
            }
        }

        public void changeState(CarState newCarState)
        {
            this.carState = newCarState;
            Console.WriteLine("{0}: {1}, {2}, {3}, {4}", Agenda.Agenda.getCurrentTime().ToString(), this.carState.Action, this.carState.Direction, this.carState.Floor, this.carState.InitialSpeed);
            this.updateAgenda();
        }

        private void updateAgenda()
        {
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

            else if (this.carState.Action == CarAction.Loading)
            {
                // Remove all P1 calls assigned to the car relating to this floor and direction
                // (may need to consider whether or not this is logically valid)
                this.removeP1Calls(this.carState.Floor, this.carState.Direction);

                if (p1Calls.isEmpty())
                {
                    if (p2Calls.isEmpty() && p3Calls.isEmpty())
                    {
                        // do nothing

                        // Change state of car to idle
                        CarState newState = new CarState() { Action = CarAction.Idle, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                        this.changeState(newState);
                    }
                    else
                    {
                        // TODO: There is a problem with this logic.  If we are going up, p1 is empty and the next call from p2 is higher than
                        // our current floor then we need to carry on going up to reach it rather than changing direction immediately.
                        // Change state of car to reversing
                        CarState intermediateState = new CarState() { Action = CarAction.Reversing, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                        this.changeState(intermediateState);
                    }
                }
                else
                {
                    // TODO: Need to test if there are passengers to load or unload
                    if (!this.hasWaitedOnFloor) // TODO: If there are passengers to load or unload
                    {
                        // TODO: Load or Unload passengers

                        // TODO: Place event on agenda to fire when passengers have finished loading and unloading

                        // Until this is implemented, we will just wait 10 seconds once on each floor
                        CarState newState = new CarState() { Action = CarAction.Loading, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                        CarStateChange newEvent = new CarStateChange(this, Agenda.Agenda.getCurrentTime().AddSeconds(10), newState);

                        Agenda.Agenda.addAgendaItem(newEvent);

                        this.hasWaitedOnFloor = true;
                    }
                    else
                    {
                        // Change state of car to departing (start closing doors)
                        CarState newState = new CarState() { Action = CarAction.DoorsClosing, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                        this.changeState(newState);

                        // This line will be deleted eventually
                        this.hasWaitedOnFloor = false;
                    }
                }

            }

            else if (this.carState.Action == CarAction.Reversing)
            {
                Direction newDirection = this.carState.Direction == Direction.Up ? Direction.Down : Direction.Up;

                // Place event on agenda to fire when reversing action has completed
                CarState newState = new CarState() { Action = CarAction.Loading, Direction = newDirection, Floor = this.carState.Floor, InitialSpeed = 0 };

                CarStateChange newEvent = new CarStateChange(this, Agenda.Agenda.getCurrentTime().AddSeconds(this.directionChangeTime), newState);

                Agenda.Agenda.addAgendaItem(newEvent);

                this.p1Calls = this.p2Calls;
                this.p2Calls = this.p3Calls;
                this.p3Calls = new CallsList();
            }

            else if (this.carState.Action == CarAction.DoorsClosing)
            {
                // Place event on agenda to fire when doors have finished closing
                CarState newState = new CarState() { Action = CarAction.Leaving, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                CarStateChange newEvent = new CarStateChange(this, Agenda.Agenda.getCurrentTime().AddSeconds(this.doorsCloseTime), newState);

                Agenda.Agenda.addAgendaItem(newEvent);
            }

            else if (this.carState.Action == CarAction.Leaving)
            {
                int nextFloor = this.carState.Direction == Direction.Up ? this.carState.Floor + 1 : this.carState.Floor - 1;
                double nextFloorDistance = shaft.getInterfloorDistance(this.carState.Floor, this.carState.Direction);
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

                CarState newState = new CarState() { Action = CarAction.Moving, Direction = this.carState.Direction, Floor = nextFloor, InitialSpeed = nextFloorDecisionPointSpeed };

                CarStateChange newEvent = new CarStateChange(this, Agenda.Agenda.getCurrentTime().AddSeconds(nextFloorDecisionPointTime), newState);

                Agenda.Agenda.addAgendaItem(newEvent);
            }

            else if (this.carState.Action == CarAction.Moving)
            {
                if (this.carState.Floor == this.getNextCall().getFloor())
                {
                    // Start stopping car

                    // Calculate time taken to stop
                    double stoppingTime = this.carState.InitialSpeed / this.deceleration;

                    // Place event on agenda to fire when car has stopped at floor

                    CarState newState = new CarState() { Action = CarAction.DoorsOpening, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                    CarStateChange newEvent = new CarStateChange(this, Agenda.Agenda.getCurrentTime().AddSeconds(stoppingTime), newState);

                    Agenda.Agenda.addAgendaItem(newEvent);
                }
                else
                {
                    int nextFloor = this.carState.Direction == Direction.Up ? this.carState.Floor + 1 : this.carState.Floor - 1;
                    double nextFloorDistance = (Math.Pow(this.carState.InitialSpeed, 2) / (2 * this.deceleration)) + shaft.getInterfloorDistance(this.carState.Floor, this.carState.Direction);
                    double nextFloorDecisionPointSpeed;
                    double nextFloorDecisionPointTime;

                    // First, using linear acc'n and dec'n and assuming no maximum speed, work out the parameters
                    // of the point Q at which we must finally decide whether or not to stop at the next floor
                    // (the decision point of the next floor)

                    double Q_D; // The distance to point Q from here
                    double Q_V; // The speed that this car will have reached by point Q
                    double Q_T; // The time taken to get to point Q

                    Q_D = ((2 * this.deceleration * nextFloorDistance) - Math.Pow(this.carState.InitialSpeed, 2)) / (2 * (this.acceleration + this.deceleration));
                    Q_V = Math.Sqrt(Math.Pow(this.carState.InitialSpeed, 2) + (2 * this.acceleration * Q_D));
                    Q_T = (1 / this.acceleration) * (Q_V - this.carState.InitialSpeed);

                    // If Q_V is less than or equal to the maximum speed, then Q is the decision point for whether or
                    // not to stop at the next floor.
                    // Otherwise, we must calculate the decision point R by working out the point P at which we reach
                    // our maximum speed when accelerating from our current position.

                    if (Q_V > this.maxSpeed)
                    {
                        // note that P_V would be equal to the max speed of the car
                        double P_D; // The distance to point P from here
                        double P_T; // The time taken to get to point P

                        P_T = (this.maxSpeed - this.carState.InitialSpeed) / this.acceleration;
                        P_D = this.carState.InitialSpeed + (0.5 * this.acceleration * Math.Pow(P_T, 2));

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

                    CarState newState = new CarState() { Action = CarAction.Moving, Direction = this.carState.Direction, Floor = nextFloor, InitialSpeed = nextFloorDecisionPointSpeed };

                    CarStateChange newEvent = new CarStateChange(this, Agenda.Agenda.getCurrentTime().AddSeconds(nextFloorDecisionPointTime), newState);

                    Agenda.Agenda.addAgendaItem(newEvent);
                }
            }

            else if (this.carState.Action == CarAction.DoorsOpening)
            {
                // Place an event on the agenda to fire when the doors have finished opening
                CarState newState = new CarState() { Action = CarAction.Loading, Direction = this.carState.Direction, Floor = this.carState.Floor, InitialSpeed = 0 };

                CarStateChange newEvent = new CarStateChange(this, Agenda.Agenda.getCurrentTime().AddSeconds(this.doorsOpenTime), newState);

                Agenda.Agenda.addAgendaItem(newEvent);
            }
        }

        private void removeP1Calls(int floor, Direction direction)
        {
            CallsList callsToRemove = new CallsList();

            foreach (Call call in this.p1Calls)
            {
                if (call.getFloor() == floor && (!call.hasDirection() || call.getDirection() == direction))
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
