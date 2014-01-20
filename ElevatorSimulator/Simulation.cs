using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;

namespace ElevatorSimulator
{
    static class Simulation
    {
        internal static Agenda.Agenda agenda = new Agenda.Agenda();

        // TODO
        internal static CallsQueue getQueue(int floor, Direction direction, Car car = null)
        {
            return new CallsQueue();
        }
    }
}
