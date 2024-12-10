namespace SmartHome.backend.Dto;
public class ApartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<DeviceDto> Devices { get; set; } = new();
    public int ApartmentComplexId { get; set; }
    public string? ApartmentComplexName { get; set; }
}
