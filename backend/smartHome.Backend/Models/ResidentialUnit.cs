using System;

namespace SmartHome.backend.Models
{
    public abstract class ResidentialUnit
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}