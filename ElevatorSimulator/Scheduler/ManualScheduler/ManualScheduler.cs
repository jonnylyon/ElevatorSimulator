using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Scheduler.ManualScheduler
{
    class ManualScheduler : IScheduler
    {
        public void allocateCall(PassengerGroup group, Building building)
        {
            Console.WriteLine("Please allocate passenger group to car:");
            Console.WriteLine("    Size:        {0}", group.Size);
            Console.WriteLine("    Origin:      {0}", group.Origin);
            Console.WriteLine("    Destination: {0}", group.Destination);
            Console.Write("Choose shaft index: ");
            int shaft = int.Parse(Console.ReadLine());
            Console.Write("Choose car index: ");
            int car = int.Parse(Console.ReadLine());

            building.Shafts[shaft].Cars[car].allocateHallCall(new HallCall(group));

            group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
        }

        public void reallocateCall(PassengerGroup group, Building building, ICar rejectedFrom)
        {
            Console.WriteLine("Car has rejected a hall call.  Please reallocate.");
            this.allocateCall(group, building);
        }
    }
}
