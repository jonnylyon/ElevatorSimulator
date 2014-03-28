using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Scheduler.TCOSTwoZones
{
    class TCOSTwoZones : IScheduler
    {
        public void AllocateCall(PassengerGroup group, Building building)
        {
            ICar preferredCar;
            ICar otherCar;

            Simulation.logger.logLine(string.Format("   Origin: {0}, Destination {1}", group.Origin, group.Destination));
            foreach (TCOSCar car in building.Shafts[0].Cars)
            {
                Simulation.logger.logLine(string.Format("   Car zone: {0}; location: {1}; direction {2}", string.Join(", ", car.CurrentZone.ToArray()), string.Join(", ", car.CurrentFloorsOccupied.ToArray()), car.State.Direction.ToString()));
            }

            if ((group.Origin + group.Destination) / 2 >= 4.5)
            {
                preferredCar = building.Shafts[0].Cars[1];
                otherCar = building.Shafts[0].Cars[0];
            }
            else
            {
                preferredCar = building.Shafts[0].Cars[0];
                otherCar = building.Shafts[0].Cars[1];
            }

            if (preferredCar.allocateHallCall(new HallCall(group)))
            {
                group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
            }
            else
            {
                if (otherCar.allocateHallCall(new HallCall(group)))
                {
                    group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
                }
                else
                {
                    Simulation.logger.logLine("NB: Call has failed allocation");
                    if (group.Origin != 9 && group.Destination != 9)
                    {
                        Console.WriteLine("Breakpoint");
                    }
                }
            }            
        }
    }
}
