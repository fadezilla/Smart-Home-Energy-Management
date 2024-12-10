namespace SmartHome.backend.Dto;
public class ApartmentComplexDto
{
    public int ApartmentComplexId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ApartmentDto> Apartments { get; set; } = new();
}
