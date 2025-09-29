using LctMonolith.Domain.Entities;
using LctMonolith.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace LctMonolith.Services;

/// <summary>
/// Provides read-only access to user-owned inventory (store items and artifacts).
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _uow;

    public InventoryService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<UserInventoryItem>> GetStoreInventoryAsync(Guid userId, CancellationToken ct = default)
    {
        return await _uow.UserInventoryItems
            .Query(ii => ii.UserId == userId, null, ii => ii.StoreItem)
            .OrderByDescending(i => i.AcquiredAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<UserArtifact>> GetArtifactsAsync(Guid userId, CancellationToken ct = default)
    {
        return await _uow.UserArtifacts
            .Query(a => a.UserId == userId, null, a => a.Artifact)
            .OrderByDescending(a => a.ObtainedAt)
            .ToListAsync(ct);
    }
}

