public record ListDto
{
    public Guid Id { get; init; }
    public string TrackingNumber { get; init; } = default!;
    public string Sender { get; init; } = default!;
    public string Recipient { get; init; } = default!;
    public Status CurrentStatus { get; init; }
    public DateTimeOffset? DateCreated { get; init; }
}