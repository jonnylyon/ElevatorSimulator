﻿using System;
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

            car.addHallCall(new HallCall(1, Direction.Up));
            car.addHallCall(new HallCall(5, Direction.Up));
            car.addHallCall(new HallCall(3, Direction.Down));

            while (!(Simulation.agenda.isEmpty() && car.carState.Action == CarAction.Idle))
            {
                Console.ReadKey();
                Console.WriteLine("");
                car.changeState((Simulation.agenda.moveToNextEvent() as CarStateChange).getNewCarState());
            }

            Console.WriteLine("");
            Console.WriteLine("Car is now idle.  About to add another call");

            car.addHallCall(new HallCall(6, Direction.Down));

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
