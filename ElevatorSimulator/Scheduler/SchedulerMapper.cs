﻿using System;
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
            }

            return null;
        }
    }
}
