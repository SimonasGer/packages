public static class TrackingNumberGenerator
{
    public static string New() =>
        $"LT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
}