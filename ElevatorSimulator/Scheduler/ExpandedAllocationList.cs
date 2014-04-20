using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.DataStructures;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator.Scheduler
{
    class ExpandedAllocationList
    {
        public CallsList P1Calls = new CallsList();
        public CallsList P2Calls = new CallsList();
        public CallsList P3Calls = new CallsList();

        private List<Call> AllCalls
        {
            get
            {
                return P1Calls.Union(P2Calls).Union(P3Calls).ToList();
            }
        }

        public int? HighestCallLocation
        {
            get
            {
                var allCalls = AllCalls;

                if (allCalls.Count() == 0) { return null; }

                var highestCall = allCalls.OrderBy(c => c.CallLocation).Last();

                return highestCall.CallLocation;
            }
        }

        public int? LowestCallLocation
        {
            get
            {
                var allCalls = AllCalls;

                if (allCalls.Count() == 0) { return null; }

                var lowestCall = allCalls.OrderBy(c => c.CallLocation).First();

                return lowestCall.CallLocation;
            }
        }

        public CallsList GetPassCallsList(Pass pass)
        {
            switch (pass)
            {
                case Pass.P1:
                    return P1Calls;
                case Pass.P2:
                    return P2Calls;
                case Pass.P3:
                    return P3Calls;
                default:
                    return null;
            }
        }

        public void AddGroupCallsToPass(PassengerGroup group, Pass pass)
        {
            var passList = GetPassCallsList(pass);

            passList.Add(new HallCall(group));
            passList.Add(new CarCall(group));
        }

        public List<Call> GetOrderedListOfAllCalls(Direction currentDirection)
        {
            int orderCoefficient = currentDirection == Direction.Up ? 1 : -1;
            var resultList = new List<Call>();

            foreach (Call c in P1Calls.OrderBy(a => a.CallLocation * orderCoefficient))
            {
                resultList.Add(c);
            }

            foreach (Call c in P2Calls.OrderBy(a => -1 * a.CallLocation * orderCoefficient))
            {
                resultList.Add(c);
            }

            foreach (Call c in P3Calls.OrderBy(a => a.CallLocation * orderCoefficient))
            {
                resultList.Add(c);
            }

            return resultList;
        }

        public ExpandedAllocationList(CallAllocationList original)
        {
            foreach (Call c in original.P1List)
            {
                P1Calls.Add(c);
                if (c is HallCall)
                {
                    P1Calls.Add(new CarCall(c.Passengers));
                }
            }

            foreach (Call c in original.P2List)
            {
                P2Calls.Add(c);
                if (c is HallCall)
                {
                    P2Calls.Add(new CarCall(c.Passengers));
                }
            }

            foreach (Call c in original.P3List)
            {
                P3Calls.Add(c);
                if (c is HallCall)
                {
                    P3Calls.Add(new CarCall(c.Passengers));
                }
            }
        }
    }
}
