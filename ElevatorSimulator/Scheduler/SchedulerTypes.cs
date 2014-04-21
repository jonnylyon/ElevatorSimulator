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
        TCOSETABasic,
        TCOSETABasicSquared,
        TCOSETABasicSquaredPenalty,
        TCOSETABasicPenalty,
        TCOSETDBasic,
        TCOSETDBasicSquared,
        TCOSETDBasicSquaredPenalty,
        TCOSETDBasicPenalty,
        TCOSETAAdvanced,
        TCOSETAAdvancedSquared,
        TCOSETAAdvancedSquaredPenalty,
        TCOSETAAdvancedPenalty,
        TCOSETDAdvanced,
        TCOSETDAdvancedSquared,
        TCOSETDAdvancedSquaredPenalty,
        TCOSETDAdvancedPenalty,
        TCOSClosestCar,
        TCOSMinimalOverlap,
        TCOSTradHybridUniform,
        TCOSTradHybridCentral,
        TCOSTradHybridExtremes
    }
}
