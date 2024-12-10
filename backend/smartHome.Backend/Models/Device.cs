using System;

namespace SmartHome.backend.Models
{
    public class Device
    {
        public int DeviceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsOn { get; set; }
        public double EnergyConsumptionRate { get; set; }

        public int ResidentialUnitId { get; set; }
        public ResidentialUnit ResidentialUnit { get; set; }
    }
}