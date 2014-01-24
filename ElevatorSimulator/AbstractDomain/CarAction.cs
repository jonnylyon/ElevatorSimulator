using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.AbstractDomain
{
    
    /// <summary>
    /// The current (direction-independent) behaviour the car
    /// </summary>
    enum CarAction
    {
        Moving, // From when the car begins moving to the moment at which it stops
        DoorsOpening, // From when the car stops to when the doors are open
        Loading, // From the moment that the doors are open to the moment that they begin closing (minus any idle/direction change time)
        Idle, // Time that the lift spends idle (stopped, with no calls to serve)
        Reversing, // Time spent changing direction of the car
        DoorsClosing, // From when the doors begin closing to when the car starts moving
        Leaving, // The instant when the doors have finished closing and the car starts to move
    }
}
