using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Scheduler
{
    static class Tools
    {
        public static Pass CalculatePassOfCall(ICar car, PassengerGroup group)
        {
            if (group.Direction != car.State.Direction)
            {
                return Pass.P2;
            }

            if (group.Direction == Direction.Down && group.Origin < car.State.Floor)
            {
                return Pass.P1;
            }

            if (group.Direction == Direction.Up && group.Origin > car.State.Floor)
            {
                return Pass.P1;
            }

            if (group.Origin == car.State.Floor && car.State.Action != CarAction.Leaving && car.State.Action != CarAction.Moving)
            {
                return Pass.P1;
            }

            return Pass.P3;
        }
    }
}
