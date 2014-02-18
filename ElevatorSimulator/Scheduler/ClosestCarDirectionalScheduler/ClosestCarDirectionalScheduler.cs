using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Calls;

namespace ElevatorSimulator.Scheduler.ClosestCarDirectionalScheduler
{
    class ClosestCarDirectionalScheduler : IScheduler
    {

        public void allocateCall(PassengerGroup group, Building building)
        {
            // compile list of all cars (can we do this in one linq expression?)
            List<ICar> cars = new List<ICar>();
            building.Shafts.ForEach(s => s.Cars.ForEach(c => cars.Add(c)));

            // find best car
            ICar bestCar = null;

            foreach (ICar c in cars)
            {
                if (object.ReferenceEquals(bestCar, null) || carAMoreSuitableThanB(c, bestCar, group))
                {
                    bestCar = c;
                }
            }

            // allocate call
            bestCar.allocateHallCall(new HallCall(group));

            group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
        }

        public bool carAMoreSuitableThanB(ICar a, ICar b, PassengerGroup group)
        {
            int pass_a = getPassOfHallCallGroup(a, group);
            int pass_b = getPassOfHallCallGroup(b, group);

            if (pass_a != pass_b)
            {
                return pass_a < pass_b;
            }

            switch (pass_a)
            {
                case 1:
                case 3:
                    if (a.State.Direction == Direction.Down)
                    {
                        return a.State.Floor < b.State.Floor;
                    }
                    return a.State.Floor > b.State.Floor;
                case 2:
                default:
                    if (a.State.Direction == Direction.Down)
                    {
                        return a.State.Floor > b.State.Floor;
                    }
                    return a.State.Floor < b.State.Floor;
            }
        }

        public int getPassOfHallCallGroup(ICar c, PassengerGroup group)
        {
            if (c.State.Direction != group.Direction)
            {
                return 2;
            }

            // TODO: We're glossing over the complicated case where floors are equal
            if (c.State.Direction == Direction.Down && group.Origin < c.State.Floor
                || c.State.Direction == Direction.Up && group.Origin > c.State.Floor)
            {
                return 1;
            }

            return 3;
        }

        public void reallocateCall(PassengerGroup group, Building building, ICar rejectedFrom)
        {
            throw new NotImplementedException();
        }
    }
}
