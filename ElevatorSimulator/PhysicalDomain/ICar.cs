using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.Calls;
using ElevatorSimulator.Agenda;

namespace ElevatorSimulator.PhysicalDomain
{
    interface ICar : IEventOwner
    {
        CarState State { get; }
        void changeState(CarState newCarState);
        bool allocateHallCall(HallCall hallCall, int? deck = null);
        void otherCarStateHasChanged();
    }
}
