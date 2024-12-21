using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.backend.Controllers
{
    /// <summary>
    /// Manages device groups, which allow grouping multiple devices together for collective operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceGroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeviceGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new device group with the specified name.
        /// </summary>
        /// <param name="name">The name of the new device group.</param>
        /// <returns>The created device group.</returns>
        /// <response code="201">Device group created successfully</response>
        /// <response code="400">Invalid data (e.g., name is null or empty)</response>
        [HttpPost]
        [SwaggerResponse(201, "Device group created successfully", typeof(DeviceGroup))]
        [SwaggerResponse(400, "Invalid data")]
        public async Task<IActionResult> CreateGroup([FromBody] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Group name cannot be empty.");
            }

            var group = new DeviceGroup { Name = name };
            _context.DeviceGroups.Add(group);
            await _context.SaveChangesAsync();

            // Return 201 with the newly created resource
            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
        }

        /// <summary>
        /// Retrieves a device group by its ID, including its associated devices.
        /// </summary>
        /// <param name="id">The ID of the device group.</param>
        /// <returns>The requested device group, or 404 if not found.</returns>
        /// <response code="200">Returns the requested device group</response>
        /// <response code="404">If the group is not found</response>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Returns the requested device group", typeof(DeviceGroup))]
        [SwaggerResponse(404, "Group not found")]
        public async Task<IActionResult> GetGroup(int id)
        {
            var group = await _context.DeviceGroups
                .Include(g => g.Devices)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound("Group not found");
            }

            return Ok(group);
        }

        /// <summary>
        /// Adds a device to an existing device group.
        /// </summary>
        /// <param name="groupId">The ID of the device group.</param>
        /// <param name="deviceId">The ID of the device to add.</param>
        /// <returns>The updated device group with the new device, or 404 if not found.</returns>
        /// <response code="200">Returns the updated device group</response>
        /// <response code="404">If the device group or device is not found</response>
        [HttpPost("{groupId}/add-device/{deviceId}")]
        [SwaggerResponse(200, "Returns the updated device group", typeof(DeviceGroup))]
        [SwaggerResponse(404, "Device group or device not found")]
        public async Task<IActionResult> AddDeviceToGroup(int groupId, int deviceId)
        {
            var group = await _context.DeviceGroups
                .Include(g => g.Devices)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return NotFound("DeviceGroup not found.");
            }

            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null)
            {
                return NotFound("Device not found.");
            }

            group.Devices.Add(device);
            await _context.SaveChangesAsync();

            return Ok(group); 
        }
    }
}
