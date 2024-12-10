using System;

namespace SmartHome.backend.Models
{
    public class ApartmentComplex
    {
        public int ApartmentComplexId { get; set; }
        public string Name { get; set; } = string.Empty;

        // One apartment complex has multiple apartments
        public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
    }
}