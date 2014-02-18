using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ElevatorSimulator.PassengerArrivals.DistributionSpecification;

namespace ElevatorSimulator.PassengerArrivals
{
    class PassengerDistributionCreator
    {
        private Random random = new Random();

        private List<ArrivalFloor> arrivalFloorSpecifications;

        private int maxPassengerGroupSize;

        /// <summary>
        /// Constructor for PassengerDistributionCreator
        /// Parses the XML specification file into ArrivalFloor
        /// and DestinationFloor data classes.
        /// </summary>
        /// <param name="specificationFile"></param>
        public PassengerDistributionCreator(string specificationFile, int maxPassengerGroupSize)
        {
            this.maxPassengerGroupSize = maxPassengerGroupSize;

            arrivalFloorSpecifications = new List<ArrivalFloor>();
            
            XDocument xmlDoc = XDocument.Load(specificationFile);

            foreach (XElement arrivalFloor in xmlDoc.Root.Elements("ArrivalFloor"))
            {
                ArrivalFloor a = new ArrivalFloor() { Floor = int.Parse(arrivalFloor.Attribute("Floor").Value) };

                foreach (XElement destinationFloor in arrivalFloor.Elements("DestinationFloor"))
                {
                    a.DestinationFloors.Add(new DestinationFloor()
                    {
                        Floor = int.Parse(destinationFloor.Attribute("Floor").Value),
                        ArrivalsPerMinuteMean = double.Parse(destinationFloor.Attribute("ArrivalsPerMinuteMean").Value),
                        GroupSizeMean = double.Parse(destinationFloor.Attribute("GroupSizeMean").Value)
                    });
                }

                this.arrivalFloorSpecifications.Add(a);
            }
        }

        /// <summary>
        /// Method to create and return a randomised passenger arrival distribution
        /// using the poisson distribution and the floor data from the instance-specific
        /// specification file
        /// </summary>
        /// <param name="start">The time at which the distribution should start</param>
        /// <param name="end">The time at which the distribution should end</param>
        /// <param name="resolution">The time interval of the poisson distribution in milliseconds</param>
        /// <returns>The passenger arrival distribution</returns>
        public PassengerDistribution createPassengerDistribution(DateTime start, DateTime end, int resolution)
        {
            PassengerDistribution dist = new PassengerDistribution();
            DateTime currentTime = start;

            while (currentTime <= end)
            {
                foreach (ArrivalFloor a in this.arrivalFloorSpecifications)
                {
                    foreach (DestinationFloor d in a.DestinationFloors)
                    {
                        for (int i = 0; i < this.poissonRandomNumber((d.ArrivalsPerMinuteMean * resolution / 60000)); i++)
                        {
                            dist.addPassengerGroup(new PassengerGroupArrivalData()
                            {
                                Size = this.poissonRandomNumberInRange(d.GroupSizeMean, 1, this.maxPassengerGroupSize),
                                Origin = a.Floor,
                                Destination = d.Floor,
                                ArrivalTime = currentTime
                            });
                        }
                    }
                }

                currentTime = currentTime.AddMilliseconds(resolution);
            }

            return dist;
        }

        /// <summary>
        /// Generates a random integer based on a poisson distribution
        /// with the specified mean, with the additional restriction
        /// that the integer must be in the specified range
        /// </summary>
        /// <param name="mean">The mean of the poisson distribution</param>
        /// <param name="bottom">The lowest value allowed by the range</param>
        /// <param name="top">The highest value allowed by the range</param>
        /// <returns>A random number in the specified range based on a poisson distribution</returns>
        private int poissonRandomNumberInRange(double mean, int bottom, int top)
        {
            int result = bottom - 1;
            while (result < bottom || result > top)
            {
                result = this.poissonRandomNumber(mean);
            }
            return result;
        }

        /// <summary>
        /// Uses Knuth's Algorithm to generate a random integer based
        /// on a poisson distribution with the specified mean
        /// </summary>
        /// <param name="mean">The mean of the poisson distribution</param>
        /// <returns>A random number generated based on a poisson distribution</returns>
        private int poissonRandomNumber(double mean)
        {
            double L = Math.Pow(Math.E, mean * -1);
            double p = 1;
            int k = 0;

            while (p > L)
            {
                k++;
                p *= random.NextDouble();
            }

            return k - 1;
        }
    }
}
