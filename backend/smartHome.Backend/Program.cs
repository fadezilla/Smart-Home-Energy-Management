using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Migrations.Operations;


var builder = WebApplication.CreateBuilder(args);

//Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//Add DbContext with SQL server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Add controllers
builder.Services.AddControllers();

//Add endpoints, swagger, etc.
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
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

//Run migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //Make sure the database is up to date with latest migrations
    DbContext.Database.Migrate();
    //Call the seeder
    DataSeeder.seedData(DbContext);
    Console.WriteLine("Calling DataSeeder...");
}

//Swagger and Api pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
app.Run();