using App.Domain;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.EF;

public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{

    public DbSet<Company> Companies { get; set; } = default!;
    public DbSet<Leg> Legs { get; set; } = default!;
    public DbSet<FromLocation> FromLocations { get; set; } = default!;
    public DbSet<ToLocation> ToLocations { get; set; } = default!;
    public DbSet<PriceList> PriceLists { get; set; } = default!;
    public DbSet<Provider> Providers { get; set; } = default!;
    public DbSet<RouteInfo> RouteInfos { get; set; } = default!;
    public DbSet<Reservation> Reservations { get; set; } = default!;
    public DbSet<AppRefreshToken> AppRefreshTokens { get; set; } = default!;
    
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // disable cascade delete
        foreach (var foreignKey in builder.Model
                     .GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
            
        }
    }
}