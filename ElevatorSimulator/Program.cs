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
            string configFile = @"simconfig\examplesimcfg.xml";
            var simcfg = new SimulationConfigLoader(configFile);

            SchedulerType scheduler = simcfg.SchedulerType;
            PassengerDistribution dist = new PassengerDistribution();

            switch (simcfg.PDSource)
            {
                case PassengerDistributionSource.Error:
                    return;
                case PassengerDistributionSource.New:
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
        }
    }
}
