using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace SmartHome.Backend.Controllers
{
    /// <summary>
    /// Manages schedules for turning devices or device groups on/off at specific times.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// DTO for creating a new schedule.
        /// </summary>
        public class CreateScheduleDto
        {
            [Required]
            public string TargetType { get; set; } = string.Empty; // "device" or "group"

            [Required]
            public int TargetId { get; set; }

            [Required]
            public string Action { get; set; } = "on"; // "on"/"off"/etc.

            [Required]
            public DateTime ScheduledTime { get; set; }
        }

        /// <summary>
        /// Creates a new schedule.
        /// </summary>
        /// <param name="dto">Schedule details.</param>
        /// <returns>The created schedule.</returns>
        /// <response code="201">Schedule created successfully</response>
        /// <response code="400">Validation errors or invalid data</response>
        [HttpPost]
        [SwaggerResponse(201, "Schedule created successfully", typeof(Schedule))]
        [SwaggerResponse(400, "Validation errors or invalid data")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var schedule = new Schedule
            {
                TargetType = dto.TargetType,
                TargetId = dto.TargetId,
                Action = dto.Action,
                ScheduledTime = dto.ScheduledTime
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.Id }, schedule);
        }

        /// <summary>
        /// Retrieves a schedule by its ID.
        /// </summary>
        /// <param name="id">Schedule ID.</param>
        /// <returns>The requested schedule if found.</returns>
        /// <response code="200">Returns the requested schedule</response>
        /// <response code="404">Schedule not found</response>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Returns the requested schedule", typeof(Schedule))]
        [SwaggerResponse(404, "Schedule not found")]
        public async Task<IActionResult> GetSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound("Schedule not found");
            }
            return Ok(schedule);
        }

        /// <summary>
        /// Retrieves all schedules in the system.
        /// </summary>
        /// <returns>A list of all schedules.</returns>
        /// <response code="200">Returns a list of schedules</response>
        [HttpGet]
        [SwaggerResponse(200, "Returns a list of schedules", typeof(IEnumerable<Schedule>))]
        public async Task<IActionResult> GetAllSchedules()
        {
            var schedules = await _context.Schedules.ToListAsync();
            return Ok(schedules);
        }

        /// <summary>
        /// Updates an existing schedule.
        /// </summary>
        /// <param name="id">Schedule ID.</param>
        /// <param name="dto">Updated schedule details.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Schedule updated successfully</response>
        /// <response code="404">Schedule not found</response>
        /// <response code="400">Validation errors or invalid data</response>
        [HttpPut("{id}")]
        [SwaggerResponse(204, "Schedule updated successfully")]
        [SwaggerResponse(404, "Schedule not found")]
        [SwaggerResponse(400, "Validation errors or invalid data")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] CreateScheduleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound("Schedule not found.");
            }

            schedule.TargetType = dto.TargetType;
            schedule.TargetId = dto.TargetId;
            schedule.Action = dto.Action;
            schedule.ScheduledTime = dto.ScheduledTime;

            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a schedule by its ID.
        /// </summary>
        /// <param name="id">Schedule ID.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Schedule deleted successfully</response>
        /// <response code="404">Schedule not found</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(204, "Schedule deleted successfully")]
        [SwaggerResponse(404, "Schedule not found")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound("Schedule not found.");
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
