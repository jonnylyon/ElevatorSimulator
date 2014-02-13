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

        internal static List<PassengerGroup> getAllPassengersStillNotAtDestination()
        {
            return allPassengers.Where(c => c.CarAlightTime == null).ToList();
        }

        internal static double getAverageWaitingTime()
        {
            return allPassengers.Average(c => c.CarBoardTime.Subtract(c.HallCallTime).TotalSeconds);
        }

        internal static double getAverageTimeToDestination()
        {
            return allPassengers.Average(c => c.CarAlightTime.Subtract(c.HallCallTime).TotalSeconds);
        }

        internal static double getAverageSquaredWaitingTime()
        {
            return allPassengers.Average(c => Math.Pow(c.CarBoardTime.Subtract(c.HallCallTime).TotalSeconds, 2));
        }

        internal static double getAverageSquaredTimeToDestination()
        {
            return allPassengers.Average(c => Math.Pow(c.CarAlightTime.Subtract(c.HallCallTime).TotalSeconds, 2));
        }
    }
}
