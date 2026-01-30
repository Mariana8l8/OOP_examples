using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_examples.Containers
{
    internal class HeavyContainer : Container
    {
        public HeavyContainer(int id, double weight) : base(id, weight)
        {
            if (weight <= 3000) Console.WriteLine("HeavyContainer weight must be > 3000.");
        }   
        public override double Consumption() => 3.0 * Weight; 
    }
}
