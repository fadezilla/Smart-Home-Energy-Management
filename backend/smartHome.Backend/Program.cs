using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Migrations.Operations;


var builder = WebApplication.CreateBuilder(args);

//Add services

//Add DbContext with SQL server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Add controllers
builder.Services.AddControllers();

//Add endpoints, swagger, etc.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

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