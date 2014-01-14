using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Agenda
{
    static class Agenda
    {
        private static List<Event> agendaList = new List<Event>();

        private static DateTime currentTime = new DateTime(0);

        public static void addAgendaItem(Event item)
        {
            agendaList.Add(item);
        }

        public static void removeAgendaItem(Event item)
        {
            agendaList.Remove(item);
        }

        public static DateTime getCurrentTime()
        {
            return currentTime;
        }

        public static Event moveToNextEvent()
        {
            Event nextEvent = null;

            foreach (Event item in agendaList)
            {
                if (nextEvent == null)
                {
                    nextEvent = item;
                }
                else if (item.getTime() < nextEvent.getTime())
                {
                    nextEvent = item;
                }
            }

            if (!object.ReferenceEquals(nextEvent, null))
            {
                Agenda.removeAgendaItem(nextEvent);
                currentTime = nextEvent.getTime();
            }

            return nextEvent;
        }

        internal static bool isEmpty()
        {
            return agendaList.Count() == 0;
        }
    }
}
