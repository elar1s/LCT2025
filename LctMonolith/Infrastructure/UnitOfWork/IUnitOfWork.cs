using LctMonolith.Domain.Entities;
using LctMonolith.Infrastructure.Repositories;

namespace LctMonolith.Infrastructure.UnitOfWork;

/// <summary>
/// Unit of Work aggregates repositories and transaction boundary.
/// </summary>
public interface IUnitOfWork
{
    IGenericRepository<AppUser> Users { get; }
    IGenericRepository<Rank> Ranks { get; }
    IGenericRepository<RankRequiredMission> RankRequiredMissions { get; }
    IGenericRepository<RankRequiredCompetency> RankRequiredCompetencies { get; }
    IGenericRepository<Mission> Missions { get; }
    IGenericRepository<UserMission> UserMissions { get; }
    IGenericRepository<MissionCompetencyReward> MissionCompetencyRewards { get; }
    IGenericRepository<MissionArtifactReward> MissionArtifactRewards { get; }
    IGenericRepository<Competency> Competencies { get; }
    IGenericRepository<UserCompetency> UserCompetencies { get; }
    IGenericRepository<Artifact> Artifacts { get; }
    IGenericRepository<UserArtifact> UserArtifacts { get; }
    IGenericRepository<StoreItem> StoreItems { get; }
    IGenericRepository<UserInventoryItem> UserInventoryItems { get; }
    IGenericRepository<Transaction> Transactions { get; }
    IGenericRepository<EventLog> EventLogs { get; }
    IGenericRepository<RefreshToken> RefreshTokens { get; }
    IGenericRepository<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
