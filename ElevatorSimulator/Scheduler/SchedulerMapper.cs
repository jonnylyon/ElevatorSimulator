using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Scheduler
{
    static class SchedulerMapper
    {
        public static IScheduler getScheduler(SchedulerType scheduler)
        {
            switch (scheduler)
            {
                case SchedulerType.Manual:
                    return new ManualScheduler.ManualScheduler();
                case SchedulerType.Random:
                    return new RandomScheduler.RandomScheduler();
                case SchedulerType.ClosestCar:
                    return new ClosestCarScheduler.ClosestCarScheduler();
                case SchedulerType.ClosestCarDirectional:
                    return new ClosestCarDirectionalScheduler.ClosestCarDirectionalScheduler();
                case SchedulerType.CarZero:
                    return new CarZeroScheduler.CarZeroScheduler();
                case SchedulerType.TCOSAlternateSingleShaft:
                    return new TCOSAlternateSingleShaft.TCOSAlternateSingleShaft();
                case SchedulerType.TCOSTwoZones:
                    return new TCOSTwoZones.TCOSTwoZones();
                case SchedulerType.TCOSTwoZonesOrigin:
                    return new TCOSTwoZonesOrigin.TCOSTwoZonesOrigin();
                case SchedulerType.TCOSETABasic:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.WaitingTime, false, 1, false);
                case SchedulerType.TCOSETABasicPenalty:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.WaitingTime, false, 1, true);
                case SchedulerType.TCOSETABasicSquared:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.WaitingTime, false, 2, false);
                case SchedulerType.TCOSETABasicSquaredPenalty:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.WaitingTime, false, 2, true);
                case SchedulerType.TCOSETDBasic:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.SystemTime, false, 1, false);
                case SchedulerType.TCOSETDBasicPenalty:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.SystemTime, false, 1, true);
                case SchedulerType.TCOSETDBasicSquared:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.SystemTime, false, 2, false);
                case SchedulerType.TCOSETDBasicSquaredPenalty:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.SystemTime, false, 2, true);
                case SchedulerType.TCOSETAAdvanced:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.WaitingTime, true, 1, false);
                case SchedulerType.TCOSETAAdvancedSquared:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.WaitingTime, true, 2, false);
                case SchedulerType.TCOSETAAdvancedSquaredPenalty:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.WaitingTime, true, 2, true);
                case SchedulerType.TCOSETAAdvancedPenalty:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.WaitingTime, true, 1, true);
                case SchedulerType.TCOSETDAdvanced:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.SystemTime, true, 1, false);
                case SchedulerType.TCOSETDAdvancedSquared:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.SystemTime, true, 2, false);
                case SchedulerType.TCOSETDAdvancedSquaredPenalty:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.SystemTime, true, 2, true);
                case SchedulerType.TCOSETDAdvancedPenalty:
                    return new TCOSETAETD.TCOSETAETD(TCOSETAETD.OptimizationType.SystemTime, true, 1, true);
                case SchedulerType.TCOSClosestCar:
                    return new TCOSClosestCar.TCOSClosestCar();
                case SchedulerType.TCOSMinimalOverlap:
                    return new TCOSMinimalOverlap.TCOSMinimalOverlap();
                case SchedulerType.TCOSTradHybridUniform:
                    return new TCOSTradHybrid.TCOSTradHybrid(TCOSTradHybrid.SplitPointsType.Uniform);
                case SchedulerType.TCOSTradHybridExtremes:
                    return new TCOSTradHybrid.TCOSTradHybrid(TCOSTradHybrid.SplitPointsType.Extremes);
                case SchedulerType.TCOSTradHybridCentral:
                    return new TCOSTradHybrid.TCOSTradHybrid(TCOSTradHybrid.SplitPointsType.Central);
                case SchedulerType.ETA:
                    return new ETAETD.ETAETD(ETAETD.OptimizationType.WaitingTime, 1, false);
                case SchedulerType.ETAPenalty:
                    return new ETAETD.ETAETD(ETAETD.OptimizationType.WaitingTime, 1, true);
                case SchedulerType.ETASquared:
                    return new ETAETD.ETAETD(ETAETD.OptimizationType.WaitingTime, 2, false);
                case SchedulerType.ETASquaredPenalty:
                    return new ETAETD.ETAETD(ETAETD.OptimizationType.WaitingTime, 2, true);
                case SchedulerType.ETD:
                    return new ETAETD.ETAETD(ETAETD.OptimizationType.SystemTime, 1, false);
                case SchedulerType.ETDPenalty:
                    return new ETAETD.ETAETD(ETAETD.OptimizationType.SystemTime, 1, true);
                case SchedulerType.ETDSquared:
                    return new ETAETD.ETAETD(ETAETD.OptimizationType.SystemTime, 2, false);
                case SchedulerType.ETDSquaredPenalty:
                    return new ETAETD.ETAETD(ETAETD.OptimizationType.SystemTime, 2, true);
            }

            return null;
        }
    }
}
