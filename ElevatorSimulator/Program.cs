using System;
using ElevatorSimulator.PassengerArrivals;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Scheduler;

namespace ElevatorSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Runtime specific parameters.  Should get these from command line, but it's
            // annoying to run from Visual Studio then.
            SchedulerTypes scheduler = SchedulerTypes.ClosestCar;
            PassengerDistributionSource pdSource = PassengerDistributionSource.Load;
            string pdSpecification = @"10 floor uniform interfloor spec.xml";
            string pdFile = @"10 floor uniform interfloor.xml";
            string logFile = @"log.txt";

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

            building.addShaft(10, 0, 10);
            building.addShaft(10, 0, 10);
            building.addShaft(10, 0, 10);
            building.addShaft(10, 0, 10);
            building.addShaft(10, 0, 10);
            building.addShaft(10, 0, 10);

            building.Shafts[0].addCar();
            building.Shafts[1].addCar();
            building.Shafts[2].addCar();
            building.Shafts[3].addCar();
            building.Shafts[4].addCar();
            building.Shafts[5].addCar();

            IScheduler sched = SchedulerMapper.getScheduler(scheduler);

            Simulation.controller = new Controller(building, dist, sched);

            Simulation.controller.Start();

            Console.WriteLine("End");
            Console.WriteLine();

            if (Simulation.getAllPassengersStillNotAtDestination().Count > 0)
            {
                Console.WriteLine("ATTENTION: Not all passengers have arrived at their destinations");
                Console.WriteLine();
            }

            Console.WriteLine("Average waiting time:                {0}", Simulation.getAverageWaitingTime());
            Console.WriteLine("Average squared waiting time:        {0}", Simulation.getAverageSquaredWaitingTime());
            Console.WriteLine("Average time to destination:         {0}", Simulation.getAverageTimeToDestination());
            Console.WriteLine("Average squared time to destination: {0}", Simulation.getAverageSquaredTimeToDestination());
            Console.ReadKey();

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
