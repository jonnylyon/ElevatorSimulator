using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using ElevatorSimulator.Tools;
using ElevatorSimulator.Calls;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Scheduler.TCOSTradHybrid
{
    enum SplitPointsType
    {
        Uniform,
        Extremes,
        Central
    }

    class TCOSTradHybrid : IScheduler
    {
        private List<CarRepresentation> cars;
        private List<ICar> carsInOrderOfLastUse;

        private int[] splitLocations = new int[] { -1, 4, 7, 9, 10, 12, 13, 13, 14, 14, 15, 17, 18, 20, 23 };
        private int[] locationQuants = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        public TCOSTradHybrid(SplitPointsType type)
        {
            switch (type)
            {
                case SplitPointsType.Central:
                    break;
                case SplitPointsType.Extremes:
                    break;
                case SplitPointsType.Uniform:
                    break;
            }
        }

        public void AllocateCall(PassengerGroup group, Building building)
        {
            if (object.ReferenceEquals(cars, null))
            {
                PopulateCarRepresentations(building);
            }

            // now find which cars can serve the call

            List<CarRepresentation> applicableCars = ApplicableCars(group.Origin, group.Destination);

            List<ICar> carPreference = new List<ICar>();
            foreach (CarRepresentation cr in applicableCars.OrderBy(c => c.Floors.Count()).ThenBy(c => building.Shafts[c.Shaft].Cars[c.Car].NumberOfCalls))
            {
                carPreference.Add(building.Shafts[cr.Shaft].Cars[cr.Car]);
            }
            bool allocated = false;

            while (!allocated && carPreference.Any())
            {
                var car = carPreference.First();

                allocated = car.allocateHallCall(new HallCall(group));

                carPreference.Remove(car);
            }

            if (allocated)
            {
                group.changeState(PassengerState.Waiting, Simulation.agenda.getCurrentSimTime());
            }
            else
            {
                Simulation.logger.logLine("NB: Call has failed allocation");
            }
        }

        private List<CarRepresentation> ApplicableCars(int origin, int destination)
        {
            return this.cars.Where(c => c.Floors.Contains(origin) && c.Floors.Contains(destination)).ToList();
        }

        private void PopulateCarRepresentations(Building building)
        {
            this.cars = new List<CarRepresentation>();

            for (int i = 0; i < splitLocations.Count(); i++)
            {
                for (int j = 0; j < locationQuants[i]; j++)
                {
                    var lowerCar = new CarRepresentation(i, 0, -1, splitLocations[i]);
                    var higherCar = new CarRepresentation(i, 1, splitLocations[i] + 1, 29);

                    this.cars.Add(lowerCar);
                    this.cars.Add(higherCar);
                }
            }
        }

        private void PopulateCarRepresentations_old(Building building)
        {
            this.cars = new List<CarRepresentation>();

            int shaftCount = building.Shafts.Count();
            int[] shaftSplit = new int[shaftCount]; // the highest floor of the bottom car

            int bottomFloor = building.Shafts[0].allFloors.Min();
            int topFloor = building.Shafts[0].allFloors.Max();

            shaftSplit[0] = -1;

            List<int> availableSplitPoints = GeneralTools.getRange(1, topFloor - 2);

            double splitPointSpacing = (double)availableSplitPoints.Count() / (double)shaftCount;

            double[] splitPointAllocations = new double[shaftCount];

            splitPointAllocations[0] = 1;

            for (int i = 1; i < shaftCount; i++)
            {
                splitPointAllocations[i] = splitPointAllocations[i - 1] + splitPointSpacing;
                shaftSplit[i] = (int)splitPointAllocations[i];
            }

            for (int i = 0; i < shaftCount; i++)
            {
                var lowerCar = new CarRepresentation(i, 0, bottomFloor - 1, shaftSplit[i]);
                var higherCar = new CarRepresentation(i, 1, shaftSplit[i] + 1, topFloor);

                this.cars.Add(lowerCar);
                this.cars.Add(higherCar);
            }
        }

        struct CarRepresentation
        {
            public int Shaft;
            public int Car;
            public int LowestFloor;
            public int HighestFloor;

            public CarRepresentation(int Shaft, int Car, int LowestFloor, int HighestFloor)
            {
                this.Shaft = Shaft;
                this.Car = Car;
                this.LowestFloor = LowestFloor;
                this.HighestFloor = HighestFloor;
            }

            public List<int> Floors
            {
                get
                {
                    return GeneralTools.getRange(this.LowestFloor, this.HighestFloor);
                }
            }

            public String ToString()
            {
                return string.Format("{0}.{1}: {2} - {3}", this.Shaft, this.Car, this.LowestFloor, this.HighestFloor);
            }
        }
    }
}
