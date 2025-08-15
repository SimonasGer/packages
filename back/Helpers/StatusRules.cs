public static class StatusRules
{
    private static readonly Dictionary<Status, Status[]> Allowed = new()
    {
        [Status.Created]  = new[] { Status.Sent, Status.Canceled },
        [Status.Sent]     = new[] { Status.Accepted, Status.Returned, Status.Canceled },
        [Status.Returned] = new[] { Status.Sent, Status.Canceled },
        [Status.Accepted] = Array.Empty<Status>(),
        [Status.Canceled] = Array.Empty<Status>()
    };

    public static bool IsAllowed(Status from, Status to) =>
        Allowed.TryGetValue(from, out var next) && next.Contains(to);

    public static Status[] AllowedFrom(Status from) =>
        Allowed.TryGetValue(from, out var next) ? next : Array.Empty<Status>();
}