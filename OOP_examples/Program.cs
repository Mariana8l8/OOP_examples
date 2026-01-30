using OOP_examples.Containers;
using OOP_examples.Transport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Lab2_patterns
{
    internal static class Program
    {
        static Dictionary<int, Port> ports = new();
        static Dictionary<int, Ship> ships = new();
        static Dictionary<int, Container> cons = new();
        static Dictionary<int, int> shipMaxWeight = new(); 

        static int nextPortId = 0;
        static int nextConId = 0;
        static int nextShipId = 0;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Usage: Lab2_patterns.exe <path-to-commands.jsonl>");
                return;
            }
            var path = args[0];
            if (!File.Exists(path))
            {
                Console.Error.WriteLine("File not found: " + path);
                return;
            }

            foreach (var row in File.ReadLines(args[0]))
            {
                var line = row.Trim();
                if (line.Length == 0) continue;

                var cmd = JsonNode.Parse(line)!.AsObject(); 
                var act = (cmd["action"]?.GetValue<string>() ?? "").ToLowerInvariant();

                if (act == "create_port") CreatePort(cmd);
                else if (act == "create_container") CreateCon(cmd);
                else if (act == "create_ship") CreateShip(cmd);
                else if (act == "load") Load(cmd);
                else if (act == "unload") Unload(cmd);
                else if (act == "sail") SailTo(cmd);
                else if (act == "refuel") Refuel(cmd);
            }

            Console.WriteLine(BuildOut()); 
        }


        static void CreatePort(JsonObject o)
        {
            int id = o["id"]?.GetValue<int>() ?? nextPortId++;
            double lat = o["lat"]?.GetValue<double>() ?? 0;
            double lon = o["lon"]?.GetValue<double>() ?? 0;

            if (ports.ContainsKey(id)) return;
            ports[id] = new Port(id, lat, lon, new List<Container>());
        }

        static void CreateCon(JsonObject o)
        {
            int portId = o["port_id"]?.GetValue<int>() ?? 0;
            if (!ports.TryGetValue(portId, out var port)) return;

            double weight = 0;
            string? type = null;

            string? weightString = o["ws"]?.GetValue<string>();
            if (!string.IsNullOrEmpty(weightString))
            {
                char last = weightString[^1];
                if (char.ToUpperInvariant(last) == 'R' || char.ToUpperInvariant(last) == 'L')
                {
                    type = char.ToUpperInvariant(last).ToString();
                    weightString = weightString[..^1]; 
                }
                double.TryParse(weightString, out weight);
            }
            else
            {
                weight = o["w"]?.GetValue<int>() ?? 0;
                type = o["t"]?.GetValue<string>()?.ToUpperInvariant();
            }

            int id = nextConId++;
            Container c;
            if (type == "R") c = new RefrigeratedContainer(id, weight);
            else if (type == "L") c = new LiquidContainer(id, weight);
            else c = (weight <= 3000) ? new BasicContainer(id, weight) : new HeavyContainer(id, weight);

            cons[id] = c;
            port.Containers.Add(c);
        }

        static void CreateShip(JsonObject o)
        {
            int id = o["id"]?.GetValue<int>() ?? NextShipId();
            int portId = o["port_id"]?.GetValue<int>() ?? 0;
            if (!ports.TryGetValue(portId, out var p)) return;

            int maxWeight = o["max_weight"]?.GetValue<int>() ?? 0;
            int maxAll = o["max_all"]?.GetValue<int>() ?? 0;
            int maxHeavy = o["max_heavy"]?.GetValue<int>() ?? 0;
            int maxRefr = o["max_refrigerated"]?.GetValue<int>() ?? 0;
            int maxLiq = o["max_liquid"]?.GetValue<int>() ?? 0;

            double fuelPerKM = o["fuel_per_km"]?.GetValue<double>() ?? 0.1;
            double fuel = o["fuel"]?.GetValue<double>() ?? 0.0;
            double maxRange = o["max_range"]?.GetValue<double>() ?? 1e6;
            double emptyWeight = o["empty_w"]?.GetValue<double>() ?? 2000.0;

            if (ships.ContainsKey(id)) return;

            var s = new Ship(
                id: id,
                initialFuel: fuel,
                weight: emptyWeight,
                maxAll: maxAll,
                maxHeavy: maxHeavy,
                maxRefrigerated: maxRefr,
                maxLiquid: maxLiq,
                fuelConsumptionPerKm: fuelPerKM,
                currentPort: p,
                maxRangeKm: maxRange
            );

            ships[id] = s;
            shipMaxWeight[id] = maxWeight;

            p.IncomingShip(s);
        }

        static int NextShipId()
        {
            int id = nextShipId;
            while (ships.ContainsKey(id)) id++;
            nextShipId = id + 1;
            return id;
        }

        static void Load(JsonObject o)
        {
            int? shipId = o["ship_id"]?.GetValue<int>();
            int? contId = o["container_id"]?.GetValue<int>();
            if (shipId == null || contId == null) return;

            if (!ships.TryGetValue(shipId.Value, out var ship)) return;
            if (!cons.TryGetValue(contId.Value, out var cont)) return;

            var port = ship.CurrentPort;
            if (port == null) return;
            if (!port.Containers.Contains(cont)) return;

            if (!ship.CheckingForPermQuantityContainers(cont, ship.CurrentContainers)) return;

            double maxWeight = shipMaxWeight.TryGetValue(ship.ID, out var maxW) ? maxW : 1e5;
            double curWeight = ship.CurrentContainers.Sum(x => x.Weight);
            if (curWeight + cont.Weight > maxWeight) return;

            port.Containers.Remove(cont);
            ship.CurrentContainers.Add(cont);
        }

        static void Unload(JsonObject o)
        {
            int? shipId = o["ship_id"]?.GetValue<int>();
            int? contId = o["container_id"]?.GetValue<int>();
            if (shipId == null || contId == null) return;

            if (!ships.TryGetValue(shipId.Value, out var ship)) return;
            if (!cons.TryGetValue(contId.Value, out var cont)) return;

            var port = ship.CurrentPort;
            if (port == null) return;

            if (!ship.CurrentContainers.Contains(cont)) return;

            ship.CurrentContainers.Remove(cont);
            if (!port.Containers.Contains(cont)) port.Containers.Add(cont);
        }

        static void SailTo(JsonObject o)
        {
            int? shipId = o["ship_id"]?.GetValue<int>();
            int? destId = o["dest_port_id"]?.GetValue<int>();
            if (shipId == null || destId == null) return;

            if (!ships.TryGetValue(shipId.Value, out var ship)) return;
            if (!ports.TryGetValue(destId.Value, out var dest)) return;

            var currentPort = ship.CurrentPort;

            if (currentPort == null) return;
            if (ReferenceEquals(currentPort, dest)) return;

            if (ship.MoveTo(dest))
            {
                currentPort.OutgoingShip(ship);
                dest.IncomingShip(ship);
            }
        }

        static void Refuel(JsonObject o)
        {
            int? shipId = o["ship_id"]?.GetValue<int>();
            if (shipId == null) return;
            if (!ships.TryGetValue(shipId.Value, out var ship)) return;

            double? fuel = o["fuel"]?.GetValue<double>();

            if (fuel != null) ship.ReFuel(fuel.Value);
        }

        static string BuildOut()
        {
            var result = new JsonObject();

            foreach (var port in ports.Values.OrderBy(x => x.ID))
            {
                var portObj = new JsonObject
                {
                    ["lat"] = port.Latitude.ToString("F2"),
                    ["lon"] = port.Longitude.ToString("F2")
                };

                var basic = new List<int>();
                var heavy = new List<int>();
                var refrigerated = new List<int>();
                var liquid = new List<int>();

                foreach (var container in port.Containers)
                {
                    if (container is RefrigeratedContainer) refrigerated.Add(container.ID);
                    else if (container is LiquidContainer) liquid.Add(container.ID);
                    else if (container is HeavyContainer) heavy.Add(container.ID);
                    else basic.Add(container.ID);
                }
                basic.Sort(); heavy.Sort(); refrigerated.Sort(); liquid.Sort();

                portObj["basic_container"] = ToJsonArray(basic);
                portObj["heavy_container"] = ToJsonArray(heavy);
                portObj["refrigerated_container"] = ToJsonArray(refrigerated);
                portObj["liquid_container"] = ToJsonArray(liquid);

                foreach (var s in port.Current.OrderBy(x => x.ID))
                {
                    var shipObj = new JsonObject
                    {
                        ["fuel_left"] = s.Fuel.ToString("F2").Replace(',', '.')
                    };

                    var shipBasic = new List<int>();
                    var shipHeavy = new List<int>();
                    var shipRefrigerated = new List<int>();
                    var shipLiquid = new List<int>();

                    foreach (var container in s.CurrentContainers)
                    {
                        if (container is RefrigeratedContainer) shipRefrigerated.Add(container.ID);
                        else if (container is LiquidContainer) shipLiquid.Add(container.ID);
                        else if (container is HeavyContainer) shipHeavy.Add(container.ID);
                        else shipBasic.Add(container.ID);
                    }
                    shipBasic.Sort(); shipHeavy.Sort(); shipRefrigerated.Sort(); shipLiquid.Sort();

                    shipObj["basic_container"] = ToJsonArray(shipBasic);
                    shipObj["heavy_container"] = ToJsonArray(shipHeavy);
                    shipObj["refrigerated_container"] = ToJsonArray(shipRefrigerated);
                    shipObj["liquid_container"] = ToJsonArray(shipLiquid);

                    portObj[$"ship_{s.ID}"] = shipObj;
                }

                result[$"Port {port.ID}"] = portObj;
            }

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }

        static JsonArray ToJsonArray(IEnumerable<int> values)
        {
            var arr = new JsonArray();
            foreach (var v in values) arr.Add(JsonValue.Create(v));
            return arr;
        }
    }
}
