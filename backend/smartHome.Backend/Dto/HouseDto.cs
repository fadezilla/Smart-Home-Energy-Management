namespace SmartHome.backend.Dto;
public class HouseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<DeviceDto> Devices { get; set; } = new();
}
