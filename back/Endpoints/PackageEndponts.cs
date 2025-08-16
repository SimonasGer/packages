using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

public static class PackageEndpoints
{
    public static void MapPackageEndpoints(this IEndpointRouteBuilder app)
    {
    app.MapGet("/packages", async (PackageDbContext db, string? status) =>
    {
        var query = db.Packages.AsNoTracking();

        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<Status>(status, out var parsedStatus))
            {
                query = query.Where(p => p.CurrentStatus == parsedStatus);
            }
            else
            {
                return Results.Ok(new List<ListDto>());
            }
        }

        var data = await query
            .Select(p => new
            {
                p.Id,
                p.TrackingNumber,
                Sender = p.Sender.Name,
                Recipient = p.Recipient.Name,
                p.CurrentStatus,
                DateCreated = (DateTimeOffset?)p.History.Min(h => h.Date)
            })
            .OrderByDescending(x => x.DateCreated)
            .ToListAsync();

        var result = data.Select(x => new ListDto
        {
            Id = x.Id,
            TrackingNumber = x.TrackingNumber,
            Sender = x.Sender,
            Recipient = x.Recipient,
            CurrentStatus = x.CurrentStatus,
            DateCreated = x.DateCreated,
            AllowedTransitions = StatusRules.AllowedFrom(x.CurrentStatus)
        }).ToList();

        return Results.Ok(result);
    });


        app.MapGet("/packages/{id:guid}", async (Guid id, PackageDbContext db) =>
        {
            var p = await db.Packages
                .AsNoTracking()
                .Include(x => x.History)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p is null) return Results.NotFound();

            var details = new DetailsDto(
                p.Id,
                p.TrackingNumber,
                p.Sender,
                p.Recipient,
                p.CurrentStatus,
                p.History.OrderByDescending(h => h.Date).First().Date,
                p.History.OrderBy(h => h.Date)
                         .Select(h => new HistoryItem(h.Status, h.Date))
                         .ToList(),
                StatusRules.AllowedFrom(p.CurrentStatus)
            );

            return Results.Ok(details);
        });

        app.MapPost("/packages", async (PackageDbContext db, CreatePackageRequest req) =>
        {
            if (string.IsNullOrWhiteSpace(req.SenderName) || string.IsNullOrWhiteSpace(req.RecipientName))
                return Results.BadRequest(new { error = "SenderName and RecipientName are required." });

            var pkg = new Package
            {
                Id = Guid.NewGuid(),
                TrackingNumber = TrackingNumberGenerator.New(),
                Sender = new Person(req.SenderName, req.SenderPhone, req.SenderAddress),
                Recipient = new Person(req.RecipientName, req.RecipientPhone, req.RecipientAddress),
                CurrentStatus = Status.Created,
                History = new List<PackageStatus>()
            };

            pkg.History.Add(new PackageStatus
            {
                PackageId = pkg.Id,
                Status = Status.Created,
                Date = DateTimeOffset.UtcNow
            });

            db.Packages.Add(pkg);
            await db.SaveChangesAsync();

            var details = new DetailsDto(
                pkg.Id,
                pkg.TrackingNumber,
                pkg.Sender,
                pkg.Recipient,
                pkg.CurrentStatus,
                pkg.History.Last().Date,
                pkg.History.Select(h => new HistoryItem(h.Status, h.Date)).ToList(),
                StatusRules.AllowedFrom(pkg.CurrentStatus)
            );

            return Results.Created($"/packages/{pkg.Id}", details);
        });

        app.MapPost("/packages/{id:guid}/status", async (Guid id, PackageDbContext db, ChangeStatusRequest req) =>
        {
            var pkg = await db.Packages
                .Include(p => p.History)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pkg is null) return Results.NotFound();

            if (!StatusRules.IsAllowed(pkg.CurrentStatus, req.NextStatus))
                return Results.BadRequest(new { error = $"Illegal transition {pkg.CurrentStatus} → {req.NextStatus}" });

            pkg.CurrentStatus = req.NextStatus;
            pkg.History.Add(new PackageStatus
            {
                PackageId = pkg.Id,
                Status = req.NextStatus,
                Date = DateTimeOffset.UtcNow
            });

            await db.SaveChangesAsync();

            return Results.Ok(new ChangeStatusResultDto(
                pkg.Id,
                pkg.CurrentStatus,
                StatusRules.AllowedFrom(pkg.CurrentStatus)
            ));
        });
    }
}