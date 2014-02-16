﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.PhysicalDomain
{
    class Building
    {
        public List<Shaft> Shafts { get; set; }

        private int shaftIdCounter;

        public Building()
        {
            this.Shafts = new List<Shaft>();
            shaftIdCounter = 1;
        }

        public void addShaft(int topFloor, int bottomFloor, double interfloor)
        {
            var shaftData = new ShaftData(topFloor, bottomFloor, interfloor, shaftIdCounter);
            this.Shafts.Add(new Shaft(shaftData));
            shaftIdCounter++;
        }

        public bool allIdle()
        {
            bool result = true;

            foreach (Shaft shaft in this.Shafts)
            {
                foreach (Car car in shaft.Cars)
                {
                    result &= car.State.Action == CarAction.Idle;
                }
            }

            return result;
        }
    }
}
