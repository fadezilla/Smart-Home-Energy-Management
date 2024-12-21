using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using SmartHome.backend.Hubs;
using SmartHome.backend.Data;
using SmartHome.backend.Models;
using SmartHome.backend.Services;

public class ScheduleExecutorService : BackgroundService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<EnergyHub> _hubContext;
    private readonly ILogger<ScheduleExecutorService> _logger;

    public ScheduleExecutorService(ApplicationDbContext context,
        IHubContext<EnergyHub> hubContext,
        ILogger<ScheduleExecutorService> logger)
    {
        _context = context;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Check for schedules that need to run
            var now = DateTime.UtcNow;
            var dueSchedules = await _context.Schedules
                .Where(s => s.ScheduledTime <= now)
                .ToListAsync(stoppingToken);

            foreach (var schedule in dueSchedules)
            {
                // Apply schedule
                await ApplySchedule(schedule);
                // Optionally remove or mark the schedule as done
                _context.Schedules.Remove(schedule);
            }

            await _context.SaveChangesAsync(stoppingToken);

            await Task.Delay(60000, stoppingToken); // check every 1 minute
        }
    }

    private async Task ApplySchedule(Schedule schedule)
    {
        if (schedule.TargetType == "device")
        {
            var device = await _context.Devices.FindAsync(schedule.TargetId);
            if (device != null)
            {
                device.IsOn = schedule.Action == "on";
                _context.Devices.Update(device);
                // Optionally broadcast device update via SignalR
                await _hubContext.Clients.All.SendAsync("DeviceUpdated", device);
            }
        }
        else if (schedule.TargetType == "group")
        {
            var group = await _context.DeviceGroups
                .Include(g => g.Devices)
                .FirstOrDefaultAsync(g => g.Id == schedule.TargetId);

            if (group != null)
            {
                foreach (var dev in group.Devices)
                {
                    dev.IsOn = schedule.Action == "on";
                }
                _context.DeviceGroups.Update(group);
                // Optionally broadcast group devices update
                await _hubContext.Clients.All.SendAsync("GroupUpdated", group);
            }
        }
    }
}
