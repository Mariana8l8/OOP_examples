using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_examples.Interfaces
{
    internal interface ITransport<THub> where THub : IHub
    {
        public int ID { get; }
        public double Fuel { get; set; }
        public double Weight { get; }
        public double MaxRangeKm { get; }
        public bool MoveTo(THub dest);
    }
}
