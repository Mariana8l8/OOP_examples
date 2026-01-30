using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_examples.Containers
{
    internal class RefrigeratedContainer : HeavyContainer
    {
        public RefrigeratedContainer(int id, double weight) : base(id, weight) { }
        public override double Consumption() => 5.0 * Weight; 
    }
}
