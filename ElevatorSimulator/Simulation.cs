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

        // TODO (one thing is the max group size -- if we have a group of 4 and its too big, will all 4 wait or will 2 get on?)
        internal static CallsQueue getQueue(int floor, Direction direction, int? maxGroupSize = null, Car car = null)
        {
            return new CallsQueue();
        }
    }
}
