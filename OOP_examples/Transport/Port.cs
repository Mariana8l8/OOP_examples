using OOP_examples.Containers;
using OOP_examples.Interfaces;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_examples.Transport
{
    internal class Port : IHub, IPort
    {
        public int ID { get; }
        public double Latitude { get; }
        public double Longitude { get; }

        public List<Container> Containers { get; } = new();
        public List<IShip> History { get; } = new();
        public List<IShip> Current { get; } = new();

        public Port(int id, double lat, double lon, List<Container> containers)
        {
            ID = id;
            Latitude = lat;
            Longitude = lon;
            Containers = containers;
        }

        public void IncomingShip(IShip ship)
        {
            if (ship == null) return;
            if (!Current.Contains(ship))
            {
                Current.Add(ship);
                History.Add(ship);
            }

        }
        public void OutgoingShip(IShip ship)
        {
            if (ship == null) return;
            if (Current.Contains(ship))
            {
                Current.Remove(ship);
            }
        }
        public double DistanceTo(IHub other)
        {
            bool validA = Latitude >= -90 && Latitude <= 90 && Longitude >= -180 && Longitude <= 180;
            bool validB = other.Latitude >= -90 && other.Latitude <= 90 && other.Longitude >= -180 && other.Longitude <= 180;

            if (validA && validB)
            {
                var a = new GeoCoordinate(Latitude, Longitude);
                var b = new GeoCoordinate(other.Latitude, other.Longitude);
                return a.GetDistanceTo(b) / 1000.0; 
            }
            else
            {
                double dx = Latitude - other.Latitude;
                double dy = Longitude - other.Longitude;
                return Math.Sqrt(dx * dx + dy * dy);
            }
        }
    }
}
