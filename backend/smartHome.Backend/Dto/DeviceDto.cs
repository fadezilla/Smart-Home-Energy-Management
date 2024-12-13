using System.ComponentModel.DataAnnotations;
namespace SmartHome.backend.Dto;
public class DeviceDto
{
    
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 Characters")]
    public string Name { get; set; } = string.Empty;
    public bool IsOn { get; set; }
    [Range(0, 10000, ErrorMessage = "EnergyConsumptionRate must be between 0 and 10000")]
    public double EnergyConsumptionRate { get; set; }
}
public class AddDeviceDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsOn { get; set; }
    public double EnergyConsumptionRate { get; set; }
}
