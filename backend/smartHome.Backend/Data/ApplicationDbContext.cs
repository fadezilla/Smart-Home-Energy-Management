using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Models;

namespace SmartHome.backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<EnergyData> EnergyData { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<ApartmentComplex> ApartmentComplexes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<DeviceGroup> DeviceGroups { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TPH inheritance
            modelBuilder.Entity<ResidentialUnit>()
                .HasDiscriminator<string>("ResidentialUnitType")
                .HasValue<House>("House")
                .HasValue<Apartment>("Apartment");

            // Relationships
            modelBuilder.Entity<ResidentialUnit>()
                .HasMany(r => r.Devices)
                .WithOne(d => d.ResidentialUnit)
                .HasForeignKey(d => d.ResidentialUnitId);

            modelBuilder.Entity<ApartmentComplex>()
                .HasMany(ac => ac.Apartments)
                .WithOne(a => a.ApartmentComplex)
                .HasForeignKey(a => a.ApartmentComplexId);
             modelBuilder.Entity<Device>()
                .HasMany(d => d.DeviceGroups)
                .WithMany(g => g.Devices)
                .UsingEntity<Dictionary<string, object>>(
                    "DeviceGroupDevices", // join table name
                    dg => dg.HasOne<DeviceGroup>().WithMany().HasForeignKey("DeviceGroupId"),
                    dg => dg.HasOne<Device>().WithMany().HasForeignKey("DevicesId")
                );
            // Make sure email are always unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}