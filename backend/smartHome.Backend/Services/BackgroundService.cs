using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using SmartHome.backend.Hubs;

public class RealTimeEnergyService : BackgroundService
{
    private readonly IHubContext<EnergyHub> _hubContext;
    private readonly ILogger<RealTimeEnergyService> _logger;
    private readonly Random _random = new Random();

    public RealTimeEnergyService(IHubContext<EnergyHub> hubContext, ILogger<RealTimeEnergyService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //Simulating energy consumption in kwh as a random value for demo
            double currentEnergyUsage = Math.Round(_random.NextDouble() * 10, 2);

            await _hubContext.Clients.All.SendAsync("EnergyUpdate", new { currentEnergyUsage }, cancellationToken: stoppingToken);

            _logger.LogInformation("Broadcasting current energy usage: {usage} Kwh", currentEnergyUsage);

            await Task.Delay(5000, stoppingToken); // broadcast every 5 seconds
        }
    }
}