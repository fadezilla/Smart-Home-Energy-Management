using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using SmartHome.backend.Models;

namespace smartHome.backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnergyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnergyController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets historical energy consumption data for a given period.
        /// period can be: daily, weekly, monthly
        /// </summary>
        /// <param name="period">"daily", "weekly", or "monthly"</param>
        /// <returns>A list of time-series data points</returns>
        [HttpGet("history")]
        [SwaggerResponse(200, "Returns historical energy data")]
        public async Task<IActionResult> GetHistoricalData([FromQuery] string period = "daily")
        {
            IQueryable<EnergyData> query = _context.EnergyData;

            DateTime now = DateTime.UtcNow;
            DateTime start = now;

            switch (period.ToLower())
            {
                case "daily":
                    start = now.AddDays(-1);
                    break;
                case "weekly":
                    start = now.AddDays(-7);
                    break;
                case "monthly":
                    start = now.AddMonths(-1);
                    break;
                default:
                    start = now.AddDays(-1);
                    break;
            }

            var data = await query
                .Where(e => e.Timestamp >= start && e.Timestamp <= now)
                .OrderBy(e => e.Timestamp)
                .ToListAsync();

            return Ok(data);
        }
    }
}