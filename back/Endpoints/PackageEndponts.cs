using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

// NOTE: Assumes you already have these domain types somewhere:
// - enum Status { Created, Sent, Accepted, Returned, Canceled }
// - record Person(string Name, string Phone, string Address);
// - class Package { Guid Id; string TrackingNumber; Person Sender; Person Recipient; Status CurrentStatus; List<PackageEvent> History; }
// - class PackageEvent { int Id; Guid PackageId; Status Status; DateTimeOffset Date; }
// - class PackageDbContext : DbContext { DbSet<Package> Packages; DbSet<PackageEvent> Events; }

public static class PackageEndpoints
{
    public static void MapPackageEndpoints(this IEndpointRouteBuilder app)
    {
        /* =========================
         * GET /packages  (LIST)
         * ========================= */
        app.MapGet("/packages", async (PackageDbContext db) =>
        {
            var data = await db.Packages
                .AsNoTracking()
                .Select(p => new ListDto
                {
                    Id = p.Id,
                    TrackingNumber = p.TrackingNumber,
                    Sender = p.Sender.Name,
                    Recipient = p.Recipient.Name,
                    CurrentStatus = p.CurrentStatus,
                    // creation = earliest event
                    DateCreated = p.History
                        .OrderBy(h => h.Date)
                        .Select(h => (DateTimeOffset?)h.Date)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.DateCreated)
                .ToListAsync();

            return Results.Ok(data);
        });

        /* =========================
         * GET /packages/{id}  (DETAILS)
         * ========================= */
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
                p.History.OrderByDescending(h => h.Date).First().Date, // timestamp of current status
                p.History.OrderBy(h => h.Date)
                         .Select(h => new HistoryItem(h.Status, h.Date))
                         .ToList(),
                StatusRules.AllowedFrom(p.CurrentStatus)
            );

            return Results.Ok(details);
        });

        /* =========================
         * POST /packages  (CREATE)
         * ========================= */
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

            // initial history event
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

        /* ======================================
         * POST /packages/{id}/status  (CHANGE)
         * ====================================== */
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

            // return a small payload (snappy UI updates) + allowed next actions
            return Results.Ok(new ChangeStatusResultDto(
                pkg.Id,
                pkg.CurrentStatus,
                StatusRules.AllowedFrom(pkg.CurrentStatus)
            ));
        });
    }
}

/* =========================
 * DTOs (requests/responses)
 * ========================= */

/* =========================
 * Helpers (endpoint-scoped)
 * ========================= */
