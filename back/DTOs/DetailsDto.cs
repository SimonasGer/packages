public record DetailsDto
(
    Guid Id,
    string TrackingNumber,
    Person Sender,
    Person Recipient,
    Status CurrentStatus,
    DateTimeOffset CurrentStatusTimestamp,
    IReadOnlyList<HistoryItem> History,
    IReadOnlyList<Status> AllowedTransitions
);