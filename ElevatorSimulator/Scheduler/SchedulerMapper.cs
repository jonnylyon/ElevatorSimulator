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
                    return new TCOSETABasic.TCOSETABasic();
                case SchedulerType.TCOSETDBasic:
                    return new TCOSETDBasic.TCOSETDBasic();
                case SchedulerType.TCOSETAAdvanced:
                    return new TCOSETAAdvanced.TCOSETAAdvanced(1, false);
                case SchedulerType.TCOSETAAdvancedSquared:
                    return new TCOSETAAdvanced.TCOSETAAdvanced(2, false);
                case SchedulerType.TCOSETAAdvancedSquaredPenalty:
                    return new TCOSETAAdvanced.TCOSETAAdvanced(2, true);
                case SchedulerType.TCOSETAAdvancedPenalty:
                    return new TCOSETAAdvanced.TCOSETAAdvanced(1, true);
                case SchedulerType.TCOSETDAdvanced:
                    return new TCOSETDAdvanced.TCOSETDAdvanced(1, false);
                case SchedulerType.TCOSETDAdvancedSquared:
                    return new TCOSETDAdvanced.TCOSETDAdvanced(2, false);
                case SchedulerType.TCOSETDAdvancedSquaredPenalty:
                    return new TCOSETDAdvanced.TCOSETDAdvanced(2, true);
                case SchedulerType.TCOSETDAdvancedPenalty:
                    return new TCOSETDAdvanced.TCOSETDAdvanced(1, true);
                case SchedulerType.TCOSClosestCar:
                    return new TCOSClosestCar.TCOSClosestCar();
                case SchedulerType.TCOSMinimalOverlap:
                    return new TCOSMinimalOverlap.TCOSMinimalOverlap();
                case SchedulerType.TCOSTradHybridUniform:
                    return new TCOSTradHybrid.TCOSTradHybrid(TCOSTradHybrid.TCOSTradHybrid.SplitPointsType.Uniform);
                case SchedulerType.TCOSTraDHybridExtremes:
                    return new TCOSTradHybrid.TCOSTradHybrid(TCOSTradHybrid.TCOSTradHybrid.SplitPointsType.Extremes);
                case SchedulerType.TCOSTradHybridCentral:
                    return new TCOSTradHybrid.TCOSTradHybrid(TCOSTradHybrid.TCOSTradHybrid.SplitPointsType.Central);
            }

            return null;
        }
    }
}
