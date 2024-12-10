namespace SmartHome.backend.Dto;
public class DeviceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsOn { get; set; }
    public double EnergyConsumptionRate { get; set; }
}
public class AddDeviceDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsOn { get; set; }
    public double EnergyConsumptionRate { get; set; }
}
