using System.ComponentModel.DataAnnotations;
namespace SmartHome.backend.Dto;
public class HouseDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "House name is required")]
    [StringLength(50, ErrorMessage = "House name cannot exceed 50 characters")]
    public string Name { get; set; } = string.Empty;
    public List<DeviceDto> Devices { get; set; } = new();
}
