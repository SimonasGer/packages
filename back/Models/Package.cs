public class Package
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = default!;

    public Person Sender { get; set; } = default!;
    public Person Recipient { get; set; } = default!;

    public Status CurrentStatus { get; set; } = Status.Created;
    public List<PackageStatus> History { get; set; } = [];
}