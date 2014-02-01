using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Agenda;
using ElevatorSimulator.Calls;
using ElevatorSimulator.PassengerArrivals;
using System.IO;

namespace ElevatorSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = @"test.xml";
            PassengerDistribution dist = new PassengerDistribution();

            PassengerGroupArrivalData pg1 = new PassengerGroupArrivalData()
            {
                ArrivalTime = DateTime.Now,
                Size = 4,
                Origin = 2,
                Destination = 1
            };

            PassengerGroupArrivalData pg2 = new PassengerGroupArrivalData()
            {
                ArrivalTime = DateTime.Now.AddSeconds(5),
                Size = 1,
                Origin = 4,
                Destination = 9
            };

            dist.addPassengerGroup(pg1);
            dist.addPassengerGroup(pg2);
            dist.save(filename);

            PassengerDistribution dist2 = new PassengerDistribution();

            dist2.load(filename);

            foreach (PassengerGroupArrivalData pgad in dist2.ArrivalData)
            {
                Console.WriteLine("{0} {1} {2} {3}",
                    pgad.ArrivalTime,
                    pgad.Size,
                    pgad.Origin,
                    pgad.Destination
                    );
            }

            Console.ReadKey();

            //var shaftData = new ShaftData(24, 0, 10);
            //Shaft shaft = new Shaft(shaftData);
            //shaft.addCar();

            //shaft.Cars[0].allocateHallCall(new HallCall(new PassengerGroup(3, 1, 5)));
            //shaft.Cars[0].allocateHallCall(new HallCall(new PassengerGroup(1, 5, 6)));
            //shaft.Cars[0].allocateHallCall(new HallCall(new PassengerGroup(2, 3, 2)));

            //while (!(Simulation.agenda.isEmpty() && shaft.Cars[0].State.Action == CarAction.Idle))
            //{
            //    Console.ReadKey();
            //    Console.WriteLine("");
            //    // car.changeState((Simulation.agenda.moveToNextEvent() as CarStateChange).getNewCarState());

            //    var nextEvent = Simulation.agenda.moveToNextEvent();

            //    if (nextEvent is CarStateChangeEvent)
            //    {
            //        shaft.Cars[0].changeState((nextEvent as CarStateChangeEvent).CarState);
            //    }
            //}

            //Console.WriteLine("");
            //Console.WriteLine("Car is now idle.  About to add another call");

            //shaft.Cars[0].allocateHallCall(new HallCall(new PassengerGroup(4, 6, 1)));

            //Console.WriteLine("Call added.  Floor 6, down.");

            //while (!(Simulation.agenda.isEmpty() && shaft.Cars[0].State.Action == CarAction.Idle))
            //{
            //    Console.ReadKey();
            //    Console.WriteLine("");
            //    shaft.Cars[0].changeState((Simulation.agenda.moveToNextEvent() as CarStateChangeEvent).CarState);
            //}

            //Console.WriteLine("");
            //Console.WriteLine("End of execution");
            //Console.ReadKey();
        }
    }
}
