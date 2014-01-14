using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Agenda;

namespace ElevatorSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Shaft shaft = new Shaft();
            Car car = new Car();
            shaft.addCar(car);

            car.addHallCall(new Calls.HallCall(1, Direction.Up));
            car.addHallCall(new Calls.HallCall(5, Direction.Up));
            car.addHallCall(new Calls.HallCall(3, Direction.Down));

            while (!(Agenda.Agenda.isEmpty() && car.carState.Action == CarAction.Idle))
            {
                Console.ReadKey();
                Console.WriteLine("");
                car.changeState((Agenda.Agenda.moveToNextEvent() as CarStateChange).getNewCarState());
            }
        }
    }
}
