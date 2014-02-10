﻿using System;
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
            SchedulerTypes scheduler = SchedulerTypes.Random;
            PassengerDistributionSource pdSource = PassengerDistributionSource.Load;
            string pdSpecification = @"distribution spec.xml";
            string pdFile = @"test.xml";
            string logFile = @"log.xml";

            PassengerDistribution dist = new PassengerDistribution();

            switch (pdSource)
            {
                case PassengerDistributionSource.Load:
                    dist.load(pdFile);
                    break;
                case PassengerDistributionSource.New:
                    var pdCreator = new PassengerDistributionCreator(pdSpecification);
                    var pdStart = new DateTime(2014, 02, 06, 13, 0, 0);
                    var pdEnd = new DateTime(2014, 02, 06, 17, 0, 0);
                    var pdResolution = 200;

                    dist = pdCreator.createPassengerDistribution(pdStart, pdEnd, pdResolution);

                    dist.save(pdFile);
                    break;
            }

            Logger.Logger logger = new Logger.Logger(logFile, true);

            Simulation.logger = logger;

            Building building = new Building();

            building.addShaft(24, 0, 10);
            building.Shafts[0].addCar();

            IScheduler sched = SchedulerMapper.getScheduler(scheduler);

            Simulation.controller = new Controller(building, dist, sched);

            Simulation.controller.Start();

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
