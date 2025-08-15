public sealed record ChangeStatusResultDto
(
    Guid Id,
    Status CurrentStatus,
    IReadOnlyList<Status> AllowedTransitions
);