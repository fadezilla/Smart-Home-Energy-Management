using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Models;
using SmartHome.backend.Dto;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.backend.Controllers
{
    /// <summary>
    /// Handles device-related operations, such as toggling devices, listing devices, and querying devices that are on.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lists all devices across all houses and apartments.
        /// </summary>
        /// <returns>A list of DeviceDto objects.</returns>
        /// <response code="200">Returns all devices</response>
        [HttpGet]
        [SwaggerResponse(200, "Returns all devices", typeof(DeviceDto[]))]
        public async Task<IActionResult> GetDevices()
        {
            var devices = await _context.Devices.ToListAsync();
            var deviceDtos = devices.Select(d => new DeviceDto
            {
                Id = d.DeviceId,
                Name = d.Name,
                IsOn = d.IsOn,
                EnergyConsumptionRate = d.EnergyConsumptionRate
            }).ToList();

            return Ok(deviceDtos);
        }

        /// <summary>
        /// Retrieves a single device by its ID.
        /// </summary>
        /// <param name="id">The device ID.</param>
        /// <returns>A DeviceDto object representing the requested device.</returns>
        /// <response code="200">Returns the requested device</response>
        /// <response code="404">If the device is not found</response>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Returns the requested device", typeof(DeviceDto))]
        [SwaggerResponse(404, "Device not found")]
        public async Task<IActionResult> GetDevice(int id)
        {
            var device = await _context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device == null)
            {
                return NotFound("Device not found.");
            }

            var deviceDto = new DeviceDto
            {
                Id = device.DeviceId,
                Name = device.Name,
                IsOn = device.IsOn,
                EnergyConsumptionRate = device.EnergyConsumptionRate
            };

            return Ok(deviceDto);
        }

        /// <summary>
        /// Toggles the on/off state of a device by its ID.
        /// </summary>
        /// <param name="id">The device ID.</param>
        /// <returns>The updated DeviceDto.</returns>
        /// <response code="200">Device toggled successfully</response>
        /// <response code="404">If the device is not found</response>
        [HttpPost("{id}/toggle")]
        [SwaggerResponse(200, "Device toggled successfully", typeof(DeviceDto))]
        [SwaggerResponse(404, "Device not found")]
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

        /// <summary>
        /// Returns all devices that are currently turned on, across all houses and apartments.
        /// </summary>
        /// <returns>A list of DeviceDto objects that are on.</returns>
        /// <response code="200">Returns all devices currently on</response>
        [HttpGet("on")]
        [SwaggerResponse(200, "Returns all devices currently on", typeof(DeviceDto[]))]
        public async Task<IActionResult> GetDevicesThatAreOn()
        {
            var devicesOn = await _context.Devices
                .Where(d => d.IsOn)
                .ToListAsync();

            var deviceDtos = devicesOn.Select(d => new DeviceDto
            {
                Id = d.DeviceId,
                Name = d.Name,
                IsOn = d.IsOn,
                EnergyConsumptionRate = d.EnergyConsumptionRate
            }).ToList();

            return Ok(deviceDtos);
        }
    }
}
