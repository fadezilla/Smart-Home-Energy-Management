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
    /// Handles house-related operations such as retrieving house details, listing all houses, adding devices to a house, etc.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HousesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HousesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all houses and their associated devices.
        /// </summary>
        /// <returns>A list of HouseDto objects.</returns>
        /// <response code="200">Returns the list of all houses</response>
        /// <response code="500">Server error</response>
        [HttpGet]
        [SwaggerResponse(200, "Returns all houses with their devices", typeof(HouseDto[]))]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetHouses()
        {
            var houses = await _context.Houses.Include(h => h.Devices).ToListAsync();
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

        /// <summary>
        /// Retrieves a single house by its ID, including its devices.
        /// </summary>
        /// <param name="id">The ID of the house to retrieve.</param>
        /// <returns>A HouseDto object representing the requested house.</returns>
        /// <response code="200">Returns the requested house</response>
        /// <response code="404">If the house is not found</response>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Returns the requested house", typeof(HouseDto))]
        [SwaggerResponse(404, "House not found")]
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

        /// <summary>
        /// Adds a new device to a specific house.
        /// </summary>
        /// <param name="id">The ID of the house to add the device to.</param>
        /// <param name="dto">The device details (AddDeviceDto).</param>
        /// <returns>The newly created device (DeviceDto).</returns>
        /// <response code="201">Device added successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">If the house is not found</response>
        [HttpPost("{id}/add-device")]
        [SwaggerResponse(201, "Device added successfully", typeof(DeviceDto))]
        [SwaggerResponse(400, "Validation error in input data")]
        [SwaggerResponse(404, "House not found")]
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

            var deviceDto = new DeviceDto
            {
                Id = device.DeviceId,
                Name = device.Name,
                IsOn = device.IsOn,
                EnergyConsumptionRate = device.EnergyConsumptionRate
            };

            return CreatedAtAction(nameof(GetHouse), new { id = house.Id }, deviceDto);
        }

        /// <summary>
        /// Returns the top 5 devices with the highest energy consumption in a given house.
        /// </summary>
        /// <param name="id">The house ID.</param>
        /// <returns>A list of up to 5 DeviceDto objects sorted by energy consumption descending.</returns>
        /// <response code="200">Returns the top 5 devices</response>
        /// <response code="404">If the house is not found</response>
        [HttpGet("{id}/top-devices")]
        [SwaggerResponse(200, "Returns the top 5 highest consumption devices", typeof(DeviceDto[]))]
        [SwaggerResponse(404, "House not found")]
        public async Task<IActionResult> GetTopDevicesForHouse(int id)
        {
            var house = await _context.Houses
                .Include(h => h.Devices)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return NotFound("House not found.");
            }

            var topDevices = house.Devices
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
        /// Searches for houses by name.
        /// </summary>
        /// <param name="name">The name or partial name of the house to search for.</param>
        /// <returns>A list of houses that match the given name.</returns>
        /// <response code="200">Returns matching houses</response>
        /// <response code="400">If 'name' query parameter is missing or invalid</response>
        [HttpGet("search")]
        [SwaggerResponse(200, "Returns matching houses", typeof(HouseDto[]))]
        [SwaggerResponse(400, "Invalid query parameter")]
        public async Task<IActionResult> SearchHouses([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Please provide a valid 'name' query parameter.");
            }

            var houses = await _context.Houses
                .Include(h => h.Devices)
                .Where(h => h.Name.Contains(name))
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

        /// <summary>
        /// Returns only devices that are currently on for a specific house.
        /// </summary>
        /// <param name="id">The house ID.</param>
        /// <returns>A list of DeviceDto objects for devices that are on.</returns>
        /// <response code="200">Returns on devices</response>
        /// <response code="404">House not found</response>
        [HttpGet("{id}/on-devices")]
        [SwaggerResponse(200, "Returns devices that are currently on", typeof(DeviceDto[]))]
        [SwaggerResponse(404, "House not found")]
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

        /// <summary>
        /// Returns the total energy consumption for a specific house.
        /// </summary>
        /// <param name="id">The house ID.</param>
        /// <returns>An object containing the HouseId and TotalEnergy.</returns>
        /// <response code="200">Returns total energy consumption</response>
        /// <response code="404">If the house is not found</response>
        [HttpGet("{id}/total-energy")]
        [SwaggerResponse(200, "Returns total energy consumption for the house")]
        [SwaggerResponse(404, "House not found")]
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
    }
}
