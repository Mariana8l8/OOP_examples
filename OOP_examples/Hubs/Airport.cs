using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;
using OOP_examples.Interfaces;
using OOP_examples.Containers;

namespace OOP_examples.Hubs
{
    internal class Airport : IHub, IAirport
    {
        public int ID { get; }
        public double Latitude { get; }
        public double Longitude { get; }
        public int MaxGates { get; set; } 
        public List<Container> Containers { get; } = new();
        public List<IPlane> History { get; } = new();
        public List<IPlane> Current { get; } = new();

        public Airport(int id, double lat, double lon, int maxGates, List<Container> containers)
        {
            ID = id;
            Latitude = lat;
            Longitude = lon;
            MaxGates = maxGates;
            Containers = containers;
        }

        public void IncomingPlane(IPlane plane)
        {
            if (plane == null) return;
            if (!Current.Contains(plane) && HasFreeGate())
            {
                Current.Add(plane);
                History.Add(plane);
                MaxGates--;
            }
        }
        public void OutgoingPlane(IPlane plane)
        {
            if (plane == null) return;
            if (Current.Contains(plane))
            {
                Current.Remove(plane);
                MaxGates++;
            }
        }
        public bool HasFreeGate() => MaxGates > Current.Count ? true : false;
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
