using Microsoft.AspNetCore.Http.HttpResults;
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
    public class HousesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HousesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get endpint to fetch all houses
        [HttpGet]
        public async Task<IActionResult> GetHouses()
        {
            var houses = await _context.Houses
                .Include(h => h.Devices)
                .ToListAsync();

            var houseDtos = houses.Select(h => new HouseDto
            {
                Id = h.Id,
                Name = h.Name,
                Devices = h.Devices.Select(d => new DeviceDto
                {
                    Id = d.DeviceId,
                    Name = d.Name,
                    IsOn = d.IsOn,
                    EnergyConsumptionRate = d.EnergyConsumptionRate
                }).ToList()
            }).ToList();

            return Ok(houseDtos);
        }

        // get endpoint to fetch specifc house by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHouse(int id)
        {
            var house = await _context.Houses
                .Include(h => h.Devices)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return NotFound();
            }

            var houseDto = new HouseDto
            {
                Id = house.Id,
                Name = house.Name,
                Devices = house.Devices.Select(d => new DeviceDto
                {
                    Id = d.DeviceId,
                    Name = d.Name,
                    IsOn = d.IsOn,
                    EnergyConsumptionRate = d.EnergyConsumptionRate
                }).ToList()
            };

            return Ok(houseDto);
        }
        //Get request to get total energy usage from specific house
        [HttpGet("{id}/total-energy")]
        public async Task<IActionResult> GetTotalEnergyForHouse(int id)
        {
            var house = await _context.Houses
                .Include(h => h.Devices)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return NotFound("House not found.");
            }

            double totalEnergy = house.Devices.Sum(d => d.EnergyConsumptionRate);
            return Ok(new { HouseId = house.Id, TotalEnergy = totalEnergy });
        }

        //Get request to return all devices that are currently turned on in a specific house by id
        [HttpGet("{id}/on-devices")]
        public async Task<IActionResult> GetOnDevicesForHouse(int id)
        {
            var house = await _context.Houses
                .Include(h => h.Devices)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return NotFound("House not found.");
            }

            var onDevices = house.Devices
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

        // Post request to add A device to a specific house by id
        [HttpPost("{id}/add-device")]
        public async Task<IActionResult> AddDeviceToHouse(int id, [FromBody] AddDeviceDto dto)
        {
            var house = await _context.Houses
                .Include(h => h.Devices)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return NotFound("House not found.");
            }

            var device = new Device
            {
                Name = dto.Name,
                IsOn = dto.IsOn,
                EnergyConsumptionRate = dto.EnergyConsumptionRate,
                ResidentialUnitId = house.Id
            };

            house.Devices.Add(device);
            await _context.SaveChangesAsync();

            // Return a DeviceDto as confirmation
            var deviceDto = new DeviceDto
            {
                Id = device.DeviceId,
                Name = device.Name,
                IsOn = device.IsOn,
                EnergyConsumptionRate = device.EnergyConsumptionRate
            };

            return CreatedAtAction(nameof(GetHouse), new { id = house.Id }, deviceDto);
        }
    }
}