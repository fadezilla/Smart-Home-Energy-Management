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
