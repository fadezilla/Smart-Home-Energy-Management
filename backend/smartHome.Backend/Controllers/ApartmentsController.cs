using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Models;
using SmartHome.backend.Dto;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SmartHome.backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get endpoint to fetch all apartments
        [HttpGet]
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

        // Get endpoints to fetch specifc apartment by id
        [HttpGet("{id}")]
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

        // Post request to add a device to a specific apartment by id
        [HttpPost("{id}/add-device")]
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
    }
}
