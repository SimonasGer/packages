using Microsoft.EntityFrameworkCore;

public class PackageDbContext : DbContext
{
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageStatus> Events => Set<PackageStatus>();

    public PackageDbContext(DbContextOptions<PackageDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Package>().OwnsOne(p => p.Sender);
        b.Entity<Package>().OwnsOne(p => p.Recipient);
        b.Entity<Package>().HasMany(p => p.History).WithOne().HasForeignKey(e => e.PackageId);
        b.Entity<Package>().HasIndex(p => p.TrackingNumber).IsUnique();
    }
}
