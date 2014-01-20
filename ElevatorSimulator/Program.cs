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
            Shaft shaft = new Shaft();
            Car car = new Car();
            shaft.addCar(car);

            car.addHallCall(new HallCall(new PassengerGroup(3,1,5)));
            car.addHallCall(new HallCall(new PassengerGroup(1,5,6)));
            car.addHallCall(new HallCall(new PassengerGroup(2,3,2)));

            while (!(Simulation.agenda.isEmpty() && car.carState.Action == CarAction.Idle))
            {
                Console.ReadKey();
                Console.WriteLine("");
                car.changeState((Simulation.agenda.moveToNextEvent() as CarStateChange).getNewCarState());
            }

            Console.WriteLine("");
            Console.WriteLine("Car is now idle.  About to add another call");

            car.addHallCall(new HallCall(new PassengerGroup(4,6,1)));

            Console.WriteLine("Call added.  Floor 6, down.");

            while (!(Simulation.agenda.isEmpty() && car.carState.Action == CarAction.Idle))
            {
                Console.ReadKey();
                Console.WriteLine("");
                car.changeState((Simulation.agenda.moveToNextEvent() as CarStateChange).getNewCarState());
            }

            Console.WriteLine("");
            Console.WriteLine("End of execution");
            Console.ReadKey();
        }
    }
}
