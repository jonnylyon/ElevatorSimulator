using System;
using ElevatorSimulator.PassengerArrivals;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Scheduler;
using ElevatorSimulator.View;
using ElevatorSimulator.ConfigLoader;

namespace ElevatorSimulator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //var menu = new MainMenu();
            //menu.ShowDialog();

            string configFile = @"simconfig\examplesimcfg.xml";
            var simcfg = new SimulationConfigLoader(configFile);

            SchedulerType scheduler = simcfg.SchedulerType;
            PassengerDistribution dist = new PassengerDistribution();

            //switch (menu.SourceAction)
            switch (simcfg.PDSource)
            {
                case PassengerDistributionSource.Error:
                    return;
                case PassengerDistributionSource.New:
                    //var pdCreator = new PassengerDistributionCreator(menu.LoadXMLSpecFilePath, 20);
                    var pdCreator = new PassengerDistributionCreator(simcfg.PDSpecFilePath, simcfg.PDMaxGroupSize);
                    var pdStart = simcfg.PDStartTime;
                    var pdEnd = simcfg.PDEndTime;
                    var pdResolution = simcfg.PDResolution;

                    dist = pdCreator.createPassengerDistribution(pdStart, pdEnd, pdResolution);

                    dist.save(simcfg.PDDistFilePath);
                    break;
                case PassengerDistributionSource.Load:
                    dist.load(simcfg.PDDistFilePath);
                    break;
            }

            string logFile = simcfg.LogFile;
            
            //// Runtime specific parameters.  Should get these from command line, but it's
            //// annoying to run from Visual Studio then.
            //SchedulerTypes scheduler = SchedulerTypes.Random;
            //PassengerDistributionSource pdSource = PassengerDistributionSource.Load;
            //string pdSpecification = @"10 floor uniform interfloor spec.xml";
            //string pdFile = @"10 floor uniform interfloor.xml";
            

            //PassengerDistribution dist = new PassengerDistribution();

            //switch (pdSource)
            //{
            //    case PassengerDistributionSource.Load:
            //        dist.load(pdFile);
            //        break;
            //    case PassengerDistributionSource.New:
            //        var pdCreator = new PassengerDistributionCreator(pdSpecification);
            //        var pdStart = new DateTime(2014, 02, 06, 13, 0, 0);
            //        var pdEnd = new DateTime(2014, 02, 06, 17, 0, 0);
            //        var pdResolution = 200;

            //        dist = pdCreator.createPassengerDistribution(pdStart, pdEnd, pdResolution);

            //        dist.save(pdFile);
            //        break;
            //}

            Logger.Logger logger = new Logger.Logger(logFile, true);

            Simulation.logger = logger;

            var building = simcfg.Building;

            IScheduler sched = SchedulerMapper.getScheduler(scheduler);

            Simulation.controller = new Controller(building, dist, sched);

            Simulation.controller.Start();

            Simulation.logger.logLine("End");

            Simulation.logger.logLine("");

            if (Simulation.getAllPassengersStillNotAtDestination().Count > 0)
            {
                Simulation.logger.logLine("ATTENTION: Not all passengers have arrived at their destinations");
                Simulation.logger.logLine("");
            }

            Simulation.logger.logLine("Average waiting time:                " + Simulation.getAverageWaitingTime());
            Simulation.logger.logLine("Average squared waiting time:        " + Simulation.getAverageSquaredWaitingTime());
            Simulation.logger.logLine("Average time to destination:         " + Simulation.getAverageTimeToDestination());
            Simulation.logger.logLine("Average squared time to destination: " + Simulation.getAverageSquaredTimeToDestination());
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
