using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.Agenda
{
    /// <summary>
    /// Class defining an Agenda data structure which ...
    /// </summary>
    class Agenda
    {
        // List of tasks
        private List<AgendaEvent> agendaList = new List<AgendaEvent>();

        // Current simulation time
        private DateTime currentTime = new DateTime(0);

        // Add an event to the agenda
        public void addAgendaEvent(AgendaEvent agendaEvent)
        {
            agendaList.Add(agendaEvent);
        }

        // Remove an event from the agenda
        public void removeAgendaEvent(AgendaEvent agendaEvent)
        {
            agendaList.Remove(agendaEvent);
        }

        public DateTime getCurrentSimTime()
        {
            return currentTime;
        }

        /// <summary>
        /// Retrieves the next event by the earliest time.
        /// </summary>
        /// <returns>Next event on the agenda</returns>
        public AgendaEvent moveToNextEvent()
        {
            //TODO: What about duplicate times - what gets priority?
            AgendaEvent nextEvent = agendaList.OrderBy(a => a.TimeOccurred).First();

            this.removeAgendaEvent(nextEvent);
            currentTime = nextEvent.TimeOccurred;

            return nextEvent;
        }

        internal bool isEmpty()
        {
            return agendaList.Count() == 0;
        }
    }
}
