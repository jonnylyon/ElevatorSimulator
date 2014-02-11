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
        internal static Controller controller;
        internal static Logger.Logger logger;

        internal static List<PassengerGroup> allPassengers = new List<PassengerGroup>();
    }
}
