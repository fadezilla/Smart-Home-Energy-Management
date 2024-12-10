using SmartHome.backend.Data;
using SmartHome.backend.Models;
using System;
using System.Linq;
using System.Collections.Generic;

public static class DataSeeder
{
    public static void seedData(ApplicationDbContext context)
    {
        if (!context.Houses.Any() && !context.Apartments.Any() && !context.ApartmentComplexes.Any())
        {
            var random = new Random();
            var deviceNames = new List<string>
            {
                "smart Light", "Thermostat", "Refrigerator", "washing Machine", "Dryer",
                "Air Purifier", "Heater", "Smart TV", "Laptop Charger", "Desktop PC",
                "Microwave", "Coffee Machine", "Dishwasher", "Smart Speaker", "Router",
                "Security Camera", "Ceiling Fan", "Electric Kettle", "Vacuum Cleaner", "Playstation 5",
                "Garden Sprinkler"
            };

            // 10 houses
            var houses = new List<House>();
            for (int i = 1; i <= 10; i++)
            {
                var house = new House
                {
                    Name = $"House {i}"
                };

                // Assign random devices
                int deviceCount = random.Next(5,12);
                for (int j = 0; j < deviceCount; j++)
                {
                    var deviceName = deviceNames[random.Next(deviceNames.Count)];
                    house.Devices.Add(new Device
                    {
                        Name = deviceName,
                        IsOn = random.Next(2)==0,
                        EnergyConsumptionRate = Math.Round(random.NextDouble() * 500, 2)
                    });
                }
                houses.Add(house);
            }

            //Create 1 apartmentComplex with 6 apartments
            var apartmentComplex = new ApartmentComplex
            {
                Name = "Sunset Apartment Complex"
            };

            var apartments = new List<Apartment>();
            for (int k = 1; k <= 6; k++)
            {
                var apt = new Apartment
                {
                    Name = $"Apartment {k}",
                    ApartmentComplex = apartmentComplex
                };

                int deviceCount = random.Next(5, 12);
                for (int j = 0; j < deviceCount; j++)
                {
                    var deviceName = deviceNames[random.Next(deviceNames.Count)];
                    apt.Devices.Add(new Device
                    {
                        Name = deviceName,
                        IsOn = random.Next(2)==0,
                        EnergyConsumptionRate = Math.Round(random.NextDouble() * 500, 2)
                    });
                }
                apartments.Add(apt);
            }
            Console.WriteLine("DataSeeder: Seeding data...");
            context.Houses.AddRange(houses);
            context.Apartments.AddRange(apartments);
            context.ApartmentComplexes.Add(apartmentComplex);
            
            context.SaveChanges();
        }
    }
}