using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Scheduler
{
    static class SchedulerMapper
    {
        public static IScheduler getScheduler(SchedulerTypes scheduler)
        {
            switch (scheduler)
            {
                case SchedulerTypes.Manual:
                    return new ManualScheduler.ManualScheduler();
                    break;
            }

            return null;
        }
    }
}
