using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Calls
{
    class CallsQueue
    {
        private List<Call> calls = new List<Call>();

        public void enQueue(Call call)
        {
            calls.Add(call);
        }

        public Call deQueue()
        {
            Call call = calls.ElementAt(0);
            calls.RemoveAt(0);
            return call;
        }

        public bool isEmpty()
        {
            return calls.Count() == 0;
        }
    }
}
