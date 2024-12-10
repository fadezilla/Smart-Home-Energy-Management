using System;
namespace SmartHome.backend.Models
{
    public class Apartment : ResidentialUnit
    {
        public int ApartmentComplexId { get; set; }
        public ApartmentComplex ApartmentComplex { get; set; }
    }
}