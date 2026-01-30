using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_examples.Interfaces
{
    internal interface IAirport : IHub
    {
        public int MaxGates { get; }
        public void IncomingPlane(IPlane plane);
        public void OutgoingPlane(IPlane plane);
        public List<IPlane> Current { get; }
    }
}
