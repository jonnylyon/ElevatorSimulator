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

        private static List<PassengerGroup> allArrivedPassengers
        {
            get
            {
                return allPassengers.Where(c => c.PassengerState == PassengerState.Arrived).ToList();
            }
        }

        internal static List<PassengerGroup> getAllPassengersStillNotAtDestination()
        {
            return allPassengers.Where(c => c.PassengerState != PassengerState.Arrived).ToList();
        }

        internal static double getAverageWaitingTime()
        {
            return allArrivedPassengers.Average(c => c.CarBoardTime.Subtract(c.HallCallTime).TotalSeconds);
        }

        internal static double getAverageTimeToDestination()
        {
            return allArrivedPassengers.Average(c => c.CarAlightTime.Subtract(c.HallCallTime).TotalSeconds);
        }

        internal static double getAverageSquaredWaitingTime()
        {
            return allArrivedPassengers.Average(c => Math.Pow(c.CarBoardTime.Subtract(c.HallCallTime).TotalSeconds, 2));
        }

        internal static double getAverageSquaredTimeToDestination()
        {
            return allArrivedPassengers.Average(c => Math.Pow(c.CarAlightTime.Subtract(c.HallCallTime).TotalSeconds, 2));
        }

        internal static double getLongestWaitingTime()
        {
            return allArrivedPassengers.Max(c => c.CarBoardTime.Subtract(c.HallCallTime).TotalSeconds);
        }

        internal static double getLongestTimeToDestination()
        {
            return allArrivedPassengers.Max(c => c.CarAlightTime.Subtract(c.HallCallTime).TotalSeconds);
        }

        internal static void logPassengerGroupDetails()
        {
            foreach (PassengerGroup pg in allPassengers)
            {
                Simulation.logger.logLine(string.Empty);
                Simulation.logger.logLine(String.Format("Passenger Group - size: {0}; origin: {1}; destination {2};", pg.Size, pg.Origin, pg.Destination));
                Simulation.logger.logLine(String.Format("        Hall Call Time: {0}.{1}", pg.HallCallTime, pg.HallCallTime.Millisecond));
                Simulation.logger.logLine(String.Format("        Car Board Time: {0}.{1}", pg.CarBoardTime, pg.CarBoardTime.Millisecond));
                Simulation.logger.logLine(String.Format("       Car Alight Time: {0}.{1}", pg.CarAlightTime, pg.CarAlightTime.Millisecond));
            }
        }
    }
}
