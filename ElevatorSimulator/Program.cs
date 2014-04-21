using System;
using ElevatorSimulator.PassengerArrivals;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Scheduler;
using ElevatorSimulator.ConfigLoader;

namespace ElevatorSimulator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string configFile;
            if (args.Length == 0)
            {
                configFile = @"simconfig\evaluation\TCOSTradHybridUniform_downpeak2.xml";
            }
            else
            {
                configFile = args[0];
            }

            var simcfg = new SimulationConfigLoader(configFile);

            SchedulerType scheduler = simcfg.SchedulerType;
            PassengerDistribution dist = new PassengerDistribution();

            if (simcfg.PDSource == PassengerDistributionSource.Error)
            {
                return;
            }

            // If a new PD is needed, generate it and save it to file
            if (simcfg.PDSource == PassengerDistributionSource.New)
            {
                var pdCreator = new PassengerDistributionCreator(simcfg.PDSpecFilePath, simcfg.PDMaxGroupSize);
                var pdStart = simcfg.PDStartTime;
                var pdEnd = simcfg.PDEndTime;
                var pdResolution = simcfg.PDResolution;

                var newDist = pdCreator.createPassengerDistribution(pdStart, pdEnd, pdResolution);

                newDist.save(simcfg.PDDistFilePath);
            }

            // Load the PD back from the file (whether it is new or not)
            // This is implemented like this in case saving and loading
            // the PD file reduces the accuracy of timestamps, to ensure
            // that the simulation runs are consistent every time the
            // same PD is used.
            dist.load(simcfg.PDDistFilePath);

            string logFile = simcfg.LogFile;

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
            Simulation.logger.logLine("Longest waiting time:                " + Simulation.getLongestWaitingTime());
            Simulation.logger.logLine("Longest time to destination:         " + Simulation.getLongestTimeToDestination());
            //Console.ReadKey();

            //Simulation.logTotalNumberOfAllocationsPerCar();
            //Console.ReadKey();

            //Simulation.logPassengerGroupDetails();
            //Console.ReadKey();

            //Simulation.logUnArrivedPassengerGroupDetails();
            //Console.ReadKey();
        }
    }
}
