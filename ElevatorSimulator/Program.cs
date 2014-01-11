using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;

namespace ElevatorSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Shaft shaft = new Shaft();
            shaft.addCar(new Car());
        }
    }
}
