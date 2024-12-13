using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Models;
using SmartHome.backend.Dto;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SmartHome.backend.Controllers
{
    /// <summary>
    /// Handles apartment complex-related operations, such as listing all complexes, retrieving a single complex and its apartments, 
    /// and querying devices within the complex.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ApartmentComplexesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApartmentComplexesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all apartment complexes, along with their apartments and each apartment's devices.
        /// </summary>
        /// <returns>A list of ApartmentComplexDto objects.</returns>
        /// <response code="200">Returns all apartment complexes with apartments and devices</response>
        [HttpGet]
        [SwaggerResponse(200, "Returns all apartment complexes with their apartments and devices", typeof(ApartmentComplexDto[]))]
        public async Task<IActionResult> GetApartmentComplexes()
        {
            var complexes = await _context.ApartmentComplexes
                .Include(ac => ac.Apartments)
                    .ThenInclude(a => a.Devices)
                .ToListAsync();

            var complexDtos = complexes.Select(ac => new ApartmentComplexDto
            {
                ApartmentComplexId = ac.ApartmentComplexId,
                Name = ac.Name,
                Apartments = ac.Apartments.Select(a => new ApartmentDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    ApartmentComplexId = a.ApartmentComplexId,
                    ApartmentComplexName = ac.Name,
                    Devices = a.Devices.Select(d => new DeviceDto
                    {
                        Id = d.DeviceId,
                        Name = d.Name,
                        IsOn = d.IsOn,
                        EnergyConsumptionRate = d.EnergyConsumptionRate
                    }).ToList()
                }).ToList()
            }).ToList();

            return Ok(complexDtos);
        }

        /// <summary>
        /// Retrieves a single apartment complex by its ID, including its apartments and the apartments' devices.
        /// </summary>
        /// <param name="id">The apartment complex ID.</param>
        /// <returns>An ApartmentComplexDto object representing the requested complex.</returns>
        /// <response code="200">Returns the requested apartment complex</response>
        /// <response code="404">If the apartment complex is not found</response>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Returns the requested apartment complex", typeof(ApartmentComplexDto))]
        [SwaggerResponse(404, "Apartment complex not found")]
        public async Task<IActionResult> GetApartmentComplex(int id)
        {
            var complex = await _context.ApartmentComplexes
                .Include(ac => ac.Apartments)
                    .ThenInclude(a => a.Devices)
                .FirstOrDefaultAsync(ac => ac.ApartmentComplexId == id);

            if (complex == null)
            {
                return NotFound("Apartment complex not found.");
            }

            var complexDto = new ApartmentComplexDto
            {
                ApartmentComplexId = complex.ApartmentComplexId,
                Name = complex.Name,
                Apartments = complex.Apartments.Select(a => new ApartmentDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    ApartmentComplexId = a.ApartmentComplexId,
                    ApartmentComplexName = complex.Name,
                    Devices = a.Devices.Select(d => new DeviceDto
                    {
                        Id = d.DeviceId,
                        Name = d.Name,
                        IsOn = d.IsOn,
                        EnergyConsumptionRate = d.EnergyConsumptionRate
                    }).ToList()
                }).ToList()
            };

            return Ok(complexDto);
        }

        /// <summary>
        /// Returns the top 5 devices with the highest energy consumption rate across all apartments in a given apartment complex.
        /// </summary>
        /// <param name="id">The apartment complex ID.</param>
        /// <returns>Up to 5 DeviceDto objects representing the top-consuming devices.</returns>
        /// <response code="200">Returns the top 5 devices</response>
        /// <response code="404">If the apartment complex is not found</response>
        [HttpGet("{id}/top-devices")]
        [SwaggerResponse(200, "Returns the top 5 highest consumption devices for the apartment complex", typeof(DeviceDto[]))]
        [SwaggerResponse(404, "Apartment complex not found")]
        public async Task<IActionResult> GetTopDevicesForComplex(int id)
        {
            var complex = await _context.ApartmentComplexes
                .Include(ac => ac.Apartments)
                    .ThenInclude(a => a.Devices)
                .FirstOrDefaultAsync(ac => ac.ApartmentComplexId == id);

            if (complex == null)
            {
                return NotFound("Apartment complex not found.");
            }

            var allDevices = complex.Apartments.SelectMany(a => a.Devices).ToList();
            var topDevices = allDevices
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
        /// Returns only devices currently turned on across all apartments in a given apartment complex.
        /// </summary>
        /// <param name="id">The apartment complex ID.</param>
        /// <returns>A list of DeviceDto objects that are on.</returns>
        /// <response code="200">Returns on devices</response>
        /// <response code="404">If the apartment complex is not found</response>
        [HttpGet("{id}/on-devices")]
        [SwaggerResponse(200, "Returns devices that are currently on in the apartment complex", typeof(DeviceDto[]))]
        [SwaggerResponse(404, "Apartment complex not found")]
        public async Task<IActionResult> GetOnDevicesForComplex(int id)
        {
            var complex = await _context.ApartmentComplexes
                .Include(ac => ac.Apartments)
                    .ThenInclude(a => a.Devices)
                .FirstOrDefaultAsync(ac => ac.ApartmentComplexId == id);

            if (complex == null)
            {
                return NotFound("Apartment complex not found.");
            }

            var onDevices = complex.Apartments
                .SelectMany(a => a.Devices)
                .Where(d => d.IsOn)
                .Select(d => new DeviceDto
                {
                    Id = d.DeviceId,
                    Name = d.Name,
                    IsOn = d.IsOn,
                    EnergyConsumptionRate = d.EnergyConsumptionRate
                }).ToList();

            return Ok(onDevices);
        }

        /// <summary>
        /// Returns the total energy consumption (sum of all devices' rates) for a given apartment complex.
        /// </summary>
        /// <param name="id">The apartment complex ID.</param>
        /// <returns>An object containing the ApartmentComplexId and TotalEnergy.</returns>
        /// <response code="200">Returns total energy consumption</response>
        /// <response code="404">If the apartment complex is not found</response>
        [HttpGet("{id}/total-energy")]
        [SwaggerResponse(200, "Returns total energy consumption for the apartment complex")]
        [SwaggerResponse(404, "Apartment complex not found")]
        public async Task<IActionResult> GetTotalEnergyForComplex(int id)
        {
            var complex = await _context.ApartmentComplexes
                .Include(ac => ac.Apartments)
                    .ThenInclude(a => a.Devices)
                .FirstOrDefaultAsync(ac => ac.ApartmentComplexId == id);

            if (complex == null)
            {
                return NotFound("Apartment complex not found.");
            }

            double totalEnergy = complex.Apartments.SelectMany(a => a.Devices).Sum(d => d.EnergyConsumptionRate);
            return Ok(new { ApartmentComplexId = complex.ApartmentComplexId, TotalEnergy = totalEnergy });
        }
    }
}
