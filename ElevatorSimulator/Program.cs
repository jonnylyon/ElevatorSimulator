using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Agenda;
using ElevatorSimulator.Calls;

namespace ElevatorSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var shaftData = new ShaftData(24, 0, 10);
            Shaft shaft = new Shaft(shaftData);
            shaft.addCar();

            shaft.Cars[0].allocateHallCall(new HallCall(new PassengerGroup(3, 1, 5)));
            shaft.Cars[0].allocateHallCall(new HallCall(new PassengerGroup(1, 5, 6)));
            shaft.Cars[0].allocateHallCall(new HallCall(new PassengerGroup(2, 3, 2)));

            while (!(Simulation.agenda.isEmpty() && shaft.Cars[0].State.Action == CarAction.Idle))
            {
                Console.ReadKey();
                Console.WriteLine("");
                // car.changeState((Simulation.agenda.moveToNextEvent() as CarStateChange).getNewCarState());

                var nextEvent = Simulation.agenda.moveToNextEvent();

                if (nextEvent is CarStateChangeEvent)
                {
                    shaft.Cars[0].changeState((nextEvent as CarStateChangeEvent).CarState);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Car is now idle.  About to add another call");

            shaft.Cars[0].allocateHallCall(new HallCall(new PassengerGroup(4, 6, 1)));

            Console.WriteLine("Call added.  Floor 6, down.");

            while (!(Simulation.agenda.isEmpty() && shaft.Cars[0].State.Action == CarAction.Idle))
            {
                Console.ReadKey();
                Console.WriteLine("");
                shaft.Cars[0].changeState((Simulation.agenda.moveToNextEvent() as CarStateChangeEvent).CarState);
            }

            Console.WriteLine("");
            Console.WriteLine("End of execution");
            Console.ReadKey();
        }
    }
}
