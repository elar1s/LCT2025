using LctMonolith.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;

namespace LctMonolith.Infrastructure.Data;

/// <summary>
/// Development database seeder for initial ranks, competencies, sample store items.
/// Idempotent: checks existence before inserting.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);

        if (!await db.Ranks.AnyAsync(ct))
        {
            var ranks = new List<Rank>
            {
                new() { Name = "Искатель", Order = 0, RequiredExperience = 0 },
                new() { Name = "Пилот-кандидат", Order = 1, RequiredExperience = 500 },
                new() { Name = "Принятый в экипаж", Order = 2, RequiredExperience = 1500 }
            };
            db.Ranks.AddRange(ranks);
            Log.Information("Seeded {Count} ranks", ranks.Count);
        }

        if (!await db.Competencies.AnyAsync(ct))
        {
            var comps = new[]
            {
                "Вера в дело","Стремление к большему","Общение","Аналитика","Командование","Юриспруденция","Трёхмерное мышление","Базовая экономика","Основы аэронавигации"
            }.Select(n => new Competency { Name = n });
            db.Competencies.AddRange(comps);
            Log.Information("Seeded competencies");
        }

        if (!await db.StoreItems.AnyAsync(ct))
        {
            db.StoreItems.AddRange(new StoreItem { Name = "Футболка Алабуга", Price = 100 }, new StoreItem { Name = "Брелок Буран", Price = 50 });
            Log.Information("Seeded store items");
        }

        await db.SaveChangesAsync(ct);
    }
}
