using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.DataStructures
{
    class AgendaEventTimePriorityComparer : IComparer<Agenda.AgendaEvent>
    {
        int IComparer<Agenda.AgendaEvent>.Compare(Agenda.AgendaEvent x, Agenda.AgendaEvent y)
        {
            if (x.TimeOccurred < y.TimeOccurred)
            {
                return 1;
            }

            if (x.TimeOccurred > y.TimeOccurred)
            {
                return -1;
            }

            return 0;
        }

        public static IComparer<Agenda.AgendaEvent> getAgendaEventTimePriorityComparer()
        {
            return (IComparer<Agenda.AgendaEvent>)new AgendaEventTimePriorityComparer();
        }
    }
}
