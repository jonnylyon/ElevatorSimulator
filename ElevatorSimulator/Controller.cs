using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.PassengerArrivals;
using ElevatorSimulator.Agenda;
using ElevatorSimulator.Scheduler;

namespace ElevatorSimulator
{
    class Controller
    {
        public Building building;
        private IScheduler scheduler;

        public Controller(Building building, PassengerDistribution passengerDistribution, IScheduler scheduler)
        {
            this.building = building;
            this.scheduler = scheduler;

            this.processPassengerDistribution(passengerDistribution);
        }

        private void processPassengerDistribution(PassengerDistribution passengerDistribution)
        {
            foreach (PassengerGroupArrivalData arrival in passengerDistribution.ArrivalData)
            {
                PassengerGroup group = new PassengerGroup(arrival.Size, arrival.Origin, arrival.Destination);
                PassengerHallCallEvent hallCallEvent = new PassengerHallCallEvent(group, arrival.ArrivalTime);
                Simulation.agenda.addAgendaEvent(hallCallEvent);

                Simulation.allPassengers.Add(group);
            }
        }

        internal void Start()
        {
            while (!(Simulation.agenda.isEmpty() && this.building.allIdleOrParked()))
            {
                var nextEvent = Simulation.agenda.moveToNextEvent();

                if (nextEvent is CarStateChangeEvent)
                {
                    (nextEvent.Owner as ICar).changeState((nextEvent as CarStateChangeEvent).CarState);
                }
                else if (nextEvent is PassengerHallCallEvent)
                {
                    this.scheduler.AllocateCall(nextEvent.Owner as PassengerGroup, this.building);
                }
            }
        }

        public void reallocateHallCall(PassengerGroup group, ICar rejectedFrom)
        {

        }
    }
}
