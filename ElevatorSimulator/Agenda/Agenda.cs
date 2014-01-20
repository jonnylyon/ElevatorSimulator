using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Agenda
{
    class Agenda
    {
        private List<Event> agendaList = new List<Event>();

        private DateTime currentTime = new DateTime(0);

        public void addAgendaItem(Event item)
        {
            agendaList.Add(item);
        }

        public void removeAgendaItem(Event item)
        {
            agendaList.Remove(item);
        }

        public DateTime getCurrentTime()
        {
            return currentTime;
        }

        public Event moveToNextEvent()
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
                this.removeAgendaItem(nextEvent);
                currentTime = nextEvent.getTime();
            }

            return nextEvent;
        }

        internal bool isEmpty()
        {
            return agendaList.Count() == 0;
        }
    }
}
