using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Services;
using SmartHome.backend.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

//--------------------------------------
// 1. Build the WebApplication
//--------------------------------------
var builder = WebApplication.CreateBuilder(args);

// 1A: Add CORS Services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal", policy =>
    {
        // Adjust the origin to match your Angular dev server (default: http://localhost:4200)
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

//Add other services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<RealTimeEnergyService>();

//Add DbContext with SQL server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Add controllers
builder.Services.AddControllers();

//Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Smart Home Energy Management API",
        Version = "v1",
        Description = "API for managing houses, apartments, and devices, along with their energy usage.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Harley Seierstad",
            Email = "harleyseierstad@hotmail.com"
        }
    });
    c.EnableAnnotations();
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

//--------------------------------------
// 2. Migrate & Seed Data
//--------------------------------------
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    DataSeeder.seedData(dbContext);
    Console.WriteLine("Calling DataSeeder...");
}

//--------------------------------------
// 3. Configure the Middleware
//--------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3A: Use CORS *before* Authorization or MapControllers
app.UseCors("AllowLocal");

app.UseAuthorization();

// If you have a custom exception middleware
app.UseMiddleware<ExceptionMiddleware>();

// SignalR
app.MapHub<EnergyHub>("/hub/energy");

// Controllers
app.MapControllers();

//--------------------------------------
// 4. Run the App
//--------------------------------------
app.Run();
