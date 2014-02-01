using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ElevatorSimulator.PassengerArrivals
{
    class PassengerDistribution
    {
        // A list of all passenger arrivals
        private List<PassengerGroupArrivalData> _ArrivalData = new List<PassengerGroupArrivalData>();

        public List<PassengerGroupArrivalData> ArrivalData
        {
            get
            {
                return this._ArrivalData;
            }
        }

        public PassengerDistribution(string file = null)
        {
            if (!object.ReferenceEquals(file, null))
            {
                this.load(file);
            }
        }

        public void load(string file)
        {
            // is this hacky?
            this._ArrivalData.RemoveAll(g => true);

            XDocument xmlDoc = XDocument.Load(file);

            foreach (XElement passengerGroup in xmlDoc.Root.Elements("PassengerGroup"))
            {
                this._ArrivalData.Add(new PassengerGroupArrivalData()
                {
                    ArrivalTime = DateTime.Parse(passengerGroup.Attribute("ArrivalTime").Value),
                    Size = int.Parse(passengerGroup.Attribute("Size").Value),
                    Origin = int.Parse(passengerGroup.Attribute("Origin").Value),
                    Destination = int.Parse(passengerGroup.Attribute("Destination").Value)
                });
            }
        }

        public void save(string filename)
        {
            XDocument xmlDoc = new XDocument(new XElement("PassengerGroups"));

            foreach (PassengerGroupArrivalData passengerGroup in this._ArrivalData)
            {
                XElement element = new XElement("PassengerGroup");
                element.SetAttributeValue("ArrivalTime", passengerGroup.ArrivalTime.ToString());
                element.SetAttributeValue("Size", passengerGroup.Size.ToString());
                element.SetAttributeValue("Origin", passengerGroup.Origin.ToString());
                element.SetAttributeValue("Destination", passengerGroup.Destination.ToString());

                xmlDoc.Root.Add(element);
            }
            
            // This method will only save in the current working directory
            // We can probably make it save to a specified directory if we want
            // but I haven't found out how
            xmlDoc.Save(filename);
        }

        public void addPassengerGroup(PassengerGroupArrivalData group)
        {
            this._ArrivalData.Add(group);
        }
    }
}
