using LctMonolith.Infrastructure.UnitOfWork;
using LctMonolith.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace LctMonolith.Services;

/// <summary>
/// Provides aggregated analytics metrics for dashboards.
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _uow;
    public AnalyticsService(IUnitOfWork uow) => _uow = uow;

    public async Task<AnalyticsSummary> GetSummaryAsync(CancellationToken ct = default)
    {
        var totalUsers = await _uow.Users.Query().CountAsync(ct);
        var totalMissions = await _uow.Missions.Query().CountAsync(ct);
        var completedMissions = await _uow.UserMissions.Query(um => um.Status == Domain.Entities.MissionStatus.Completed).CountAsync(ct);
        var totalArtifacts = await _uow.Artifacts.Query().CountAsync(ct);
        var totalStoreItems = await _uow.StoreItems.Query().CountAsync(ct);
        var totalExperience = await _uow.Users.Query().SumAsync(u => (long)u.Experience, ct);
        return new AnalyticsSummary
        {
            TotalUsers = totalUsers,
            TotalMissions = totalMissions,
            CompletedMissions = completedMissions,
            TotalArtifacts = totalArtifacts,
            TotalStoreItems = totalStoreItems,
            TotalExperience = totalExperience
        };
    }
}

