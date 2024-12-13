using System.ComponentModel.DataAnnotations;
namespace SmartHome.backend.Dto;
public class ApartmentDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Apartment name is required")]
    [StringLength(50, ErrorMessage = "Apartment name cannot exceed 50 characters")]
    public string Name { get; set; } = string.Empty;
    public List<DeviceDto> Devices { get; set; } = new();
    public int ApartmentComplexId { get; set; }
    public string? ApartmentComplexName { get; set; }
}
