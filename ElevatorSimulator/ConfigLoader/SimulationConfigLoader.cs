using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.PhysicalDomain;
using System.Xml.Linq;
using ElevatorSimulator.AbstractDomain;
using ElevatorSimulator.PassengerArrivals;
using ElevatorSimulator.Scheduler;

namespace ElevatorSimulator.ConfigLoader
{
    class SimulationConfigLoader
    {
        private XDocument xmlDoc;

        /// <summary>
        /// Constructor for SimulationConfigLoader
        /// </summary>
        /// <param name="filepath">The path to the simcfg xml file</param>
        public SimulationConfigLoader(string filepath)
        {
            xmlDoc = XDocument.Load(filepath);
        }

        /// <summary>
        /// The PassengerDistributionSource enum type specified in the simcfg file
        /// </summary>
        public PassengerDistributionSource PDSource
        {
            get
            {
                switch (xmlDoc.Root.Element("PassengerDistribution").Attribute("source").Value.ToLower())
                {
                    case "new":
                        return PassengerDistributionSource.New;
                    case "load":
                        return PassengerDistributionSource.Load;
                    default:
                        return PassengerDistributionSource.Error;
                }
            }
        }

        /// <summary>
        /// The Passenger Distribution specification filepath specified in the simcfg file
        /// </summary>
        public String PDSpecFilePath
        {
            get
            {
                return xmlDoc.Root.Element("PassengerDistribution").Element("SpecificationFile").Value;
            }
        }

        /// <summary>
        /// The Passenger Distribution filepath specified in the simcfg file
        /// </summary>
        public string PDDistFilePath
        {
            get
            {
                return xmlDoc.Root.Element("PassengerDistribution").Element("DistributionFile").Value;
            }
        }

        /// <summary>
        /// The maximum passenger group size specified in the simcfg file
        /// </summary>
        public int PDMaxGroupSize
        {
            get
            {
                return int.Parse(xmlDoc.Root.Element("PassengerDistribution").Element("MaxGroupSize").Value);
            }
        }

        /// <summary>
        /// The start time of the passenger distribution
        /// </summary>
        public DateTime PDStartTime
        {
            get
            {
                return DateTime.Parse(xmlDoc.Root.Element("PassengerDistribution").Element("StartTime").Value);
            }
        }

        /// <summary>
        /// The end time of the passenger distribution
        /// </summary>
        public DateTime PDEndTime
        {
            get
            {
                return DateTime.Parse(xmlDoc.Root.Element("PassengerDistribution").Element("EndTime").Value);
            }
        }

        /// <summary>
        /// The resolution of the passenger distribution, in milliseconds
        /// </summary>
        public int PDResolution
        {
            get
            {
                return int.Parse(xmlDoc.Root.Element("PassengerDistribution").Element("Resolution").Value);
            }
        }

        /// <summary>
        /// The SchedulerType specified in the simcfg file
        /// </summary>
        public SchedulerType SchedulerType
        {
            get
            {
                return (SchedulerType)Enum.Parse(typeof(SchedulerType), xmlDoc.Root.Element("Scheduler").Value, true);
            }
        }

        /// <summary>
        /// A Building object populated with Shafts and Cars based on the data
        /// in the simcfg xml file
        /// </summary>
        public Building Building
        {
            get
            {
                return this.generateBuildingFromConfig();
            }
        }

        /// <summary>
        /// Generates a Building object populated with Shafts and Cars based
        /// on the data in the simcfg xml file
        /// </summary>
        /// <returns>A populated Building object</returns>
        private Building generateBuildingFromConfig()
        {
            var carAttribs = new Dictionary<String, CarAttributes>();

            foreach (XElement xcarAttributes in xmlDoc.Root.Elements("CarAttributes"))
            {
                var attribs = new CarAttributes()
                    {
                        Acceleration = double.Parse(xcarAttributes.Element("Acceleration").Value),
                        Capacity = int.Parse(xcarAttributes.Element("Capacity").Value),
                        Deceleration = double.Parse(xcarAttributes.Element("Deceleration").Value),
                        DirectionChangeTime = double.Parse(xcarAttributes.Element("DirectionChangeTime").Value),
                        DoorsCloseTime = double.Parse(xcarAttributes.Element("DoorsCloseTime").Value),
                        DoorsOpenTime = double.Parse(xcarAttributes.Element("DoorsOpenTime").Value),
                        MaxSpeed = double.Parse(xcarAttributes.Element("MaxSpeed").Value),
                        PassengerAlightTime = double.Parse(xcarAttributes.Element("PassengerAlightTime").Value),
                        PassengerBoardTime = double.Parse(xcarAttributes.Element("PassengerBoardTime").Value)
                    };

                carAttribs.Add(xcarAttributes.Attribute("name").Value, attribs);
            }

            Building building = new Building();
            XElement xbuilding = xmlDoc.Root.Element("Building");

            int topFloor = int.Parse(xbuilding.Attribute("maxFloor").Value);
            int bottomFloor = int.Parse(xbuilding.Attribute("minFloor").Value);
            int interfloorDistance = int.Parse(xbuilding.Attribute("interfloorDistance").Value);

            foreach (XElement xshaft in xbuilding.Elements("Shaft"))
            {
                var newShaft = building.addShaft(topFloor, bottomFloor, interfloorDistance);

                foreach (XElement xcar in xshaft.Elements("Car"))
                {
                    newShaft.addCar(carAttribs[xcar.Attribute("attributes").Value], int.Parse(xcar.Attribute("startFloor").Value), (CarType)Enum.Parse(typeof(CarType), xcar.Attribute("type").Value));
                }
            }

            return building;
        }

        /// <summary>
        /// The path to the log file, either auto-generated or
        /// specified directly in the simcfg file
        /// </summary>
        public string LogFile
        {
            get
            {
                string folder = "logs/";
                string cfgtext = xmlDoc.Root.Element("LogFile").Value;

                if (String.IsNullOrEmpty(cfgtext) || cfgtext.ToLower().Equals("auto"))
                {
                    return folder + DateTimeAsSafeString(DateTime.Now) + ".txt";
                }

                if (cfgtext.StartsWith(folder))
                {
                    return cfgtext;
                }

                return folder + cfgtext;
            }
        }

        /// <summary>
        /// Given a specified DateTime, converts to a string only containing numbers and a space
        /// Good for file names
        /// </summary>
        /// <param name="datetime">The datetime object</param>
        /// <returns>A 'safe' date/time string "yyyyMMdd HHmmss"</returns>
        private string DateTimeAsSafeString(DateTime datetime)
        {
            return datetime.ToString("yyyyMMdd HHmmss");
        }
    }
}
