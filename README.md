# OOP_examples

A small educational project demonstrating **object-oriented programming (OOP)** approaches and patterns for modeling ports, ships, containers, and hubs (airports and planes).

---

## Overview

The application models core domain entities such as ports, ships, containers (multiple types), and hubs (airports and planes).  
It is implemented as a console application that reads a sequence of commands in **JSONL format** (one JSON object per line) and executes operations including:

- Creating entities
- Loading and unloading containers
- Sailing between ports
- Refueling ships

---

## Requirements

- **.NET 9** (TargetFramework: `net9.0`)
- **C# 13**
- **Visual Studio 2022** or `dotnet` CLI
- NuGet package: `System.Device.Location.Portable`  
  (already referenced in `OOP_examples.csproj`)

---

## Build

### Using dotnet CLI

```bash
dotnet build OOP_examples/OOP_examples.csproj
```

### Using Visual Studio

Open the solution or project and select:

**Build → Build Solution**

---

## Run

The program expects a single command-line argument:  
the path to a JSONL file containing commands.

### Run with dotnet CLI

```bash
dotnet run --project OOP_examples/OOP_examples.csproj -- path/to/commands.jsonl
```

### Run the compiled executable

```bash
OOP_examples/bin/Debug/net9.0/OOP_examples.exe path/to/commands.jsonl
```

---

## Commands File Format (JSONL)

Each line in the commands file must be a valid JSON object containing an `action` field and additional parameters.

### Example

```json
{"action":"create_port","id":0,"lat":50.45,"lon":30.52}
{"action":"create_container","port_id":0,"ws":"10k","type":"basic"}
{"action":"create_ship","port_id":0,"fuel":1000,"max_weight":50000}
{"action":"load","ship_id":0,"container_id":0}
{"action":"sail","ship_id":0,"port_id":1}
{"action":"refuel","ship_id":0,"amount":200}
```

Refer to `Program.cs` for the exact field names and command definitions expected by the application.

---

## Project Structure

```text
OOP_examples/
├── Program.cs            # JSONL parser and command execution
├── Transport/            # Ship, Port, and transport-related models
├── Containers/           # Container implementations
│   ├── BasicContainer.cs
│   ├── RefrigeratedContainer.cs
│   ├── LiquidContainer.cs
│   └── HeavyContainer.cs
├── Hubs/                 # Hub examples (Airport, Plane)
├── Interfaces/           # Interfaces and abstractions
└── OOP_examples.csproj
```

---

## Coding Style and Contribution

This repository includes the following files:

- `.editorconfig` — code formatting and style rules
- `CONTRIBUTING.md` — contribution guidelines

Please follow these rules when submitting changes.  
Issues and pull requests are welcome.

---

## Packages

- `System.Device.Location.Portable` — geolocation helpers (declared in the project file)

---

## License

This project is created for educational purposes and is free to use and modify.

---

## Author

**Mariana**  
GitHub: https://github.com/Mariana8l8
