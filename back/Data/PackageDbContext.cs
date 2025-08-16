using Microsoft.EntityFrameworkCore;

public class PackageDbContext(DbContextOptions<PackageDbContext> options) : DbContext(options)
{
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageStatus> Events => Set<PackageStatus>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Package>().OwnsOne(p => p.Sender);
        b.Entity<Package>().OwnsOne(p => p.Recipient);
        b.Entity<Package>().HasMany(p => p.History).WithOne().HasForeignKey(e => e.PackageId);
        b.Entity<Package>().HasIndex(p => p.TrackingNumber).IsUnique();
    }
}
