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
using ElevatorSimulator.Scheduler;

namespace ElevatorSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Runtime specific parameters.  Should get these from command line, but it's
            // annoying to run from Visual Studio then.
            SchedulerTypes scheduler = SchedulerTypes.Manual;
            PassengerDistributionSource pdSource = PassengerDistributionSource.New;
            string pdSpecification = @"distribution spec.xml";
            string pdFile = @"test.xml";
            string logFile = @"log.xml";

            PassengerDistribution dist = new PassengerDistributionCreator(pdSpecification)
                                                .createPassengerDistribution(new DateTime(2014, 02, 06, 13, 0, 0), new DateTime(2014, 02, 06, 17, 0, 0), 200);
            dist.save(pdFile);

            PassengerDistribution dist2 = new PassengerDistribution();

            dist2.load(pdFile);

            foreach (PassengerGroupArrivalData pgad in dist2.ArrivalData)
            {
                PassengerGroup pg = new PassengerGroup(pgad.Size, pgad.Origin, pgad.Destination);
                PassengerHallCallEvent phce = new PassengerHallCallEvent(pg, pgad.ArrivalTime);
                Simulation.agenda.addAgendaEvent(phce);
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
