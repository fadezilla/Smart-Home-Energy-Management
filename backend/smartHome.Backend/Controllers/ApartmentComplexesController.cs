using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Models;
using SmartHome.backend.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApartmentComplexesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApartmentComplexesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get to fetch all apartmentComplexes
        [HttpGet]
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

        // Get endpoint to return specifc apartmentComplex by id
        [HttpGet("{id}")]
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
        // Get request to return all devices that are currently on in a specifc apartment by id
        [HttpGet("{id}/on-devices")]
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
