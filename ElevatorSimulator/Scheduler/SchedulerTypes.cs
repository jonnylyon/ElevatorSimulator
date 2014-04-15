using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Scheduler
{
    enum SchedulerType
    {
        Manual,
        Random,
        ClosestCar,
        ClosestCarDirectional,
        CarZero,
        TCOSAlternateSingleShaft,
        TCOSTwoZones,
        TCOSTwoZonesOrigin,
        TCOSETABasic
    }
}
