using System;

namespace SmartHome.backend.Models
{
    public class EnergyData
    {
        public int EnergyDataId { get; set; }
        public DateTime Timestamp { get; set; }
        public double TotalEnergyConsumed { get; set; }
    }
}