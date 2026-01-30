using OOP_examples.Containers;
using OOP_examples.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OOP_examples.Hubs
{
    internal class Plane : IPlane
    {
        public int ID { get; }
        public double Fuel { get; set; }
        public double MaxRangeKm { get; }
        public double Weight { get; }
        public int MaxNumberOfAllContainers { get; }
        public int MaxNumberOfHeavyContainers { get; }
        public int MaxNumberOfBasicContainers { get; }
        public int MaxNumberOfRefrigeratedContainers { get; }
        public int MaxNumberOfLiquidContainers { get; }
        public double FuelConsumptionPerKM { get; }
        public IAirport? CurrentAirport { get; set; }
        public List<Container> CurrentContainers { get; } = new();

        public Plane(int id, double weight, double initialFuel, int maxAll, int maxHeavy, int maxBasic, int maxRefrigerated, int maxLiquid, double maxRangeKm, IAirport? currentAirport)
        {
            ID = id;
            Weight = weight;
            Fuel = initialFuel;
            MaxRangeKm = maxRangeKm;
            CurrentAirport = currentAirport;
            MaxNumberOfAllContainers = maxAll;
            MaxNumberOfHeavyContainers = maxHeavy;
            MaxNumberOfBasicContainers = maxBasic;
            MaxNumberOfLiquidContainers = maxLiquid;
            MaxNumberOfRefrigeratedContainers = maxRefrigerated;
        }

        public void ReFuel(double newFuel) { Fuel = newFuel; }

        public double WeightTotalCalc(List<Container> CurrentContainers)
        {
            double totalFuel = 0;
            foreach (var container in CurrentContainers) totalFuel += container.Consumption();
            return Weight + totalFuel;
        }

        public bool CheckingForPermQuantityContainers(Container cont, List<Container> currentContainers)
        {
            if (cont == null || currentContainers == null) return false;

            int total = currentContainers.Count;
            int basic = 0;
            int heavy = 0;
            int refr = 0;
            int liquid = 0;

            Action<Container> countByType = c =>
            {
                switch (c)
                {
                    case BasicContainer: basic++; break;
                    case RefrigeratedContainer: refr++; heavy++; break;
                    case LiquidContainer: liquid++; heavy++; break;
                    case HeavyContainer: heavy++; break;
                }
            };


            foreach (var container in currentContainers)
            {
                countByType(container);
            }

            total++;
            countByType(cont);


            if (total > MaxNumberOfAllContainers) return false;
            if (basic > MaxNumberOfBasicContainers) return false;
            if (heavy > MaxNumberOfHeavyContainers) return false;
            if (refr > MaxNumberOfRefrigeratedContainers) return false;
            if (liquid > MaxNumberOfLiquidContainers) return false;

            return true;
        }

        public bool Load(Container cont)
        {
            if (cont == null) return false;

            bool check = CheckingForPermQuantityContainers(cont, CurrentContainers);

            if (!check)
            {
                Console.WriteLine(" ");
                return false;
            }
            else
            {
                CurrentContainers.Add(cont);
                Console.WriteLine(" ");
                return true;
            }
        }
        public bool Unload(Container cont)
        {
            if (cont == null) return false;

            if (!CurrentContainers.Contains(cont))
            {
                Console.WriteLine(" ");
                return false;
            }
            else
            {
                CurrentContainers.Remove(cont);
                Console.WriteLine(" ");
                return true;
            }
        }
        public bool MoveTo(IAirport destination)
        {
            if (destination == null) return false;
            if (CurrentAirport == null) return false;
            if (CurrentAirport == destination) return true;

            double dist = CurrentAirport.DistanceTo(destination);
            double totalWeight = WeightTotalCalc(CurrentContainers);

            // The coefficient of increase in consumption per ton: 0.05 l/km.
            double totalFuel = dist * (FuelConsumptionPerKM + 0.05 * totalWeight);

            if (dist > MaxRangeKm || Fuel < totalFuel) return false;

            Fuel -= totalFuel;
            CurrentAirport = destination;
            return true;
        } 
    }
}
