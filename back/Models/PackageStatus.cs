public class PackageStatus
{
    public int Id { get; set; }
    public Guid PackageId { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
}