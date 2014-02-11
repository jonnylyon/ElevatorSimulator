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
        Stopped, // The lift is not moving and sits at a floor with its doors closed
        DoorsOpening, // From when the car stops to when the doors are open
        Unloading, // State for unloading passengers
        Loading, // State for loading passengers
        Idle, // Time that the lift spends idle (stopped, with no calls to serve)
        Reversing, // Time spent changing direction of the car
        DoorsClosing, // From when the doors begin closing to when the car starts moving
        Leaving, // Accelerating from 'Stopped' state
    }
}
