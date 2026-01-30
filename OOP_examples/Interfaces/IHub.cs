using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_examples.Interfaces
{
    internal interface IHub
    {
        public int ID { get; }
        public double Latitude { get; } 
        public double Longitude { get; }
        public double DistanceTo(IHub other);
    }
}
