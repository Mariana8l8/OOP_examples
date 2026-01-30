using OOP_examples.Containers;
using OOP_examples.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_examples.Transport
{
    internal class Ship : IShip
    {
        public int ID { get; }
        public double Fuel { get; set; }
        public double MaxRangeKm { get; }
        public IPort? CurrentPort { get; set; }
        public double Weight { get; }
        public int MaxNumberOfAllContainers { get; }
        public int MaxNumberOfHeavyContainers { get; }
        public int MaxNumberOfBasicContainers { get; }
        public int MaxNumberOfRefrigeratedContainers { get; }
        public int MaxNumberOfLiquidContainers { get; }
        public double FuelConsumptionPerKM { get; }
        public List<Container> CurrentContainers { get; set; } = new();

        public Ship(int id, double initialFuel, double weight, int maxAll, int maxHeavy, int maxRefrigerated, int maxLiquid, double fuelConsumptionPerKm, IPort? currentPort, double maxRangeKm)
        {
            ID = id;
            Fuel = initialFuel;
            Weight = weight;
            MaxNumberOfAllContainers = maxAll;
            MaxNumberOfHeavyContainers = maxHeavy;
            MaxNumberOfRefrigeratedContainers = maxRefrigerated;
            MaxNumberOfLiquidContainers = maxLiquid;
            FuelConsumptionPerKM = fuelConsumptionPerKm;
            CurrentPort = currentPort;
            MaxRangeKm = maxRangeKm;
        }
        public List<Container> GetCurrentContainers() => new List<Container>(CurrentContainers);
        public void ReFuel(double newFuel)
        {
            if (newFuel <= 0) return;
            Fuel += newFuel;
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
            if (MaxNumberOfBasicContainers > 0 && basic > MaxNumberOfBasicContainers) return false;
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
                return false;
            }
            else
            {
                CurrentContainers.Add(cont);
                return true;
            }
        }
        public bool Unload(Container cont)
        {
            if (cont == null) return false;

            if (CurrentContainers.Contains(cont))
            {
                CurrentContainers.Remove(cont);
                return true;
            }
            else
            {
                return false;
            }
        }
        public double WeightTotalCalc(List<Container> CurrentContainers)
        {
            double totalFuel = 0;
            foreach (var container in CurrentContainers) totalFuel += container.Consumption();
            return Weight + totalFuel;
        }
        public bool MoveTo(IPort destination)
        {
            if (destination == null) return false;
            if (CurrentPort == null) return false;
            if (CurrentPort == destination) return true;

            double dist = CurrentPort.DistanceTo(destination);
            double totalWeight = WeightTotalCalc(CurrentContainers) / 1000.0;

            // The coefficient of increase in consumption per ton: 0.05 l/km.
            //double totalFuel = dist * (FuelConsumptionPerKM + 0.05 * totalWeight);

            //if (dist > MaxRangeKm || Fuel < totalFuel) return false;

            //Fuel -= totalFuel;
            //CurrentPort = destination;
            //return true;

            double perKm = FuelConsumptionPerKM + 0.05 * totalWeight;
            double needed = dist * perKm;

            if (dist > MaxRangeKm || Fuel < needed) return false;

            Fuel -= needed;
            CurrentPort = destination;
            return true;
        }
    }
}
