using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Dto;
using SmartHome.backend.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.backend.Controllers
{
    /// <summary>
    /// Handles apartment-related operations, such as retrieving apartments, listing all apartments, and adding devices to apartments.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ApartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all apartments and their devices.
        /// </summary>
        /// <returns>A list of ApartmentDto objects.</returns>
        /// <response code="200">Returns all apartments</response>
        [HttpGet]
        [SwaggerResponse(200, "Returns all apartments with their devices", typeof(ApartmentDto[]))]
        public async Task<IActionResult> GetApartments()
        {
            var apartments = await _context.Apartments
                .Include(a => a.Devices)
                .Include(a => a.ApartmentComplex)
                .ToListAsync();

            var apartmentDtos = apartments.Select(a => new ApartmentDto
            {
                Id = a.Id,
                Name = a.Name,
                ApartmentComplexId = a.ApartmentComplexId,
                ApartmentComplexName = a.ApartmentComplex?.Name,
                Devices = a.Devices.Select(d => new DeviceDto
                {
                    Id = d.DeviceId,
                    Name = d.Name,
                    IsOn = d.IsOn,
                    EnergyConsumptionRate = d.EnergyConsumptionRate
                }).ToList()
            }).ToList();

            return Ok(apartmentDtos);
        }

        /// <summary>
        /// Retrieves a single apartment by ID, including its devices.
        /// </summary>
        /// <param name="id">The apartment ID.</param>
        /// <returns>An ApartmentDto object.</returns>
        /// <response code="200">Returns the requested apartment</response>
        /// <response code="404">If apartment not found</response>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Returns the requested apartment", typeof(ApartmentDto))]
        [SwaggerResponse(404, "Apartment not found")]
        public async Task<IActionResult> GetApartment(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Devices)
                .Include(a => a.ApartmentComplex)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound("Apartment not found.");
            }

            var apartmentDto = new ApartmentDto
            {
                Id = apartment.Id,
                Name = apartment.Name,
                ApartmentComplexId = apartment.ApartmentComplexId,
                ApartmentComplexName = apartment.ApartmentComplex?.Name,
                Devices = apartment.Devices.Select(d => new DeviceDto
                {
                    Id = d.DeviceId,
                    Name = d.Name,
                    IsOn = d.IsOn,
                    EnergyConsumptionRate = d.EnergyConsumptionRate
                }).ToList()
            };

            return Ok(apartmentDto);
        }

        /// <summary>
        /// Adds a new device to a specific apartment.
        /// </summary>
        /// <param name="id">The apartment ID.</param>
        /// <param name="dto">The device details.</param>
        /// <returns>The newly created device as a DeviceDto.</returns>
        /// <response code="201">Device added successfully</response>
        /// <response code="400">Validation error</response>
        /// <response code="404">Apartment not found</response>
        [HttpPost("{id}/add-device")]
        [SwaggerResponse(201, "Device added successfully", typeof(DeviceDto))]
        [SwaggerResponse(400, "Validation error")]
        [SwaggerResponse(404, "Apartment not found")]
        public async Task<IActionResult> AddDeviceToApartment(int id, [FromBody] AddDeviceDto dto)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Devices)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound("Apartment not found.");
            }

            var device = new Device
            {
                Name = dto.Name,
                IsOn = dto.IsOn,
                EnergyConsumptionRate = dto.EnergyConsumptionRate,
                ResidentialUnitId = apartment.Id
            };

            apartment.Devices.Add(device);
            await _context.SaveChangesAsync();

            var deviceDto = new DeviceDto
            {
                Id = device.DeviceId,
                Name = device.Name,
                IsOn = device.IsOn,
                EnergyConsumptionRate = device.EnergyConsumptionRate
            };

            return CreatedAtAction(nameof(GetApartment), new { id = apartment.Id }, deviceDto);
        }

        /// <summary>
        /// Returns the top 5 devices with the highest energy consumption in a given apartment.
        /// </summary>
        /// <param name="id">Apartment ID</param>
        /// <returns>List of up to 5 DeviceDto objects.</returns>
        /// <response code="200">Returns top 5 devices</response>
        /// <response code="404">Apartment not found</response>
        [HttpGet("{id}/top-devices")]
        [SwaggerResponse(200, "Returns the top 5 devices by energy consumption", typeof(DeviceDto[]))]
        [SwaggerResponse(404, "Apartment not found")]
        public async Task<IActionResult> GetTopDevicesForApartment(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Devices)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound("Apartment not found.");
            }

            var topDevices = apartment.Devices
                .OrderByDescending(d => d.EnergyConsumptionRate)
                .Take(5)
                .Select(d => new DeviceDto
                {
                    Id = d.DeviceId,
                    Name = d.Name,
                    IsOn = d.IsOn,
                    EnergyConsumptionRate = d.EnergyConsumptionRate
                }).ToList();

            return Ok(topDevices);
        }

        /// <summary>
        /// Returns only devices currently turned on in a specific apartment.
        /// </summary>
        /// <param name="id">Apartment ID</param>
        /// <returns>A list of DeviceDto objects that are on.</returns>
        /// <response code="200">Returns on devices</response>
        /// <response code="404">Apartment not found</response>
        [HttpGet("{id}/on-devices")]
        [SwaggerResponse(200, "Returns devices that are currently on", typeof(DeviceDto[]))]
        [SwaggerResponse(404, "Apartment not found")]
        public async Task<IActionResult> GetOnDevicesForApartment(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Devices)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound("Apartment not found.");
            }

            var onDevices = apartment.Devices
                .Where(d => d.IsOn)
                .Select(d => new DeviceDto
                {
                    Id = d.DeviceId,
                    Name = d.Name,
                    IsOn = d.IsOn,
                    EnergyConsumptionRate = d.EnergyConsumptionRate
                })
                .ToList();

            return Ok(onDevices);
        }
    }
}
