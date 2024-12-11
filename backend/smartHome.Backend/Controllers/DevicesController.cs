using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Models;
using SmartHome.backend.Dto;
using System.Threading.Tasks;

namespace SmartHome.backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get request to fetch all devices that are currently turned on.
        [HttpGet("on")]
        public async Task<IActionResult> GetDevicesThatAreOn()
        {
            var deviceOn = await _context.Devices
                .Where(d => d.IsOn)
                .ToListAsync();

            var deviceDtos = deviceOn.Select(d => new DeviceDto
            {
                Id = d.DeviceId,
                Name = d.Name,
                IsOn = d.IsOn,
                EnergyConsumptionRate = d.EnergyConsumptionRate
            }).ToList();

            return Ok(deviceDtos);
        }

        // Post request to toggle on and off a specifc device by id
        [HttpPost("{id}/toggle")]
        public async Task<IActionResult> ToggleDevice(int id)
        {
            var device = await _context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                return NotFound("Device not found.");
            }

            device.IsOn = !device.IsOn;
            await _context.SaveChangesAsync();

            var deviceDto = new DeviceDto
            {
                Id = device.DeviceId,
                Name = device.Name,
                IsOn = device.IsOn,
                EnergyConsumptionRate = device.EnergyConsumptionRate
            };

            return Ok(deviceDto);
        }
    }
}
