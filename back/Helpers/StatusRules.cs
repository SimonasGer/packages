public static class StatusRules
{
    private static readonly Dictionary<Status, Status[]> Allowed = new()
    {
        [Status.Created] = [Status.Sent, Status.Canceled],
        [Status.Sent] = [Status.Accepted, Status.Returned, Status.Canceled],
        [Status.Returned] = [Status.Sent, Status.Canceled],
        [Status.Accepted] = [],
        [Status.Canceled] = []
    };

    public static bool IsAllowed(Status from, Status to) =>
        Allowed.TryGetValue(from, out var next) && next.Contains(to);

    public static Status[] AllowedFrom(Status from) =>
        Allowed.TryGetValue(from, out var next) ? next : [];
}