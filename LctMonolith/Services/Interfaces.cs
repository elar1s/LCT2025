using LctMonolith.Domain.Entities;
using LctMonolith.Services.Models;

namespace LctMonolith.Services;

/// <summary>Service for issuing JWT and refresh tokens.</summary>
public interface ITokenService
{
    Task<TokenPair> IssueAsync(AppUser user, CancellationToken ct = default);
    Task<TokenPair> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task RevokeAsync(string refreshToken, CancellationToken ct = default);
}

/// <summary>Gamification progression logic (awards, rank upgrade).</summary>
public interface IGamificationService
{
    Task<ProgressSnapshot> GetProgressAsync(Guid userId, CancellationToken ct = default);
    Task ApplyMissionCompletionAsync(Guid userId, Mission mission, CancellationToken ct = default);
    Task EvaluateRankUpgradeAsync(Guid userId, CancellationToken ct = default);
}

/// <summary>Mission management and user mission state transitions.</summary>
public interface IMissionService
{
    Task<Mission> CreateMissionAsync(CreateMissionModel model, CancellationToken ct = default);
    Task<IEnumerable<Mission>> GetAvailableMissionsAsync(Guid userId, CancellationToken ct = default);
    Task<UserMission> UpdateStatusAsync(Guid userId, Guid missionId, MissionStatus status, string? submissionData, CancellationToken ct = default);
}

/// <summary>Store and inventory operations.</summary>
public interface IStoreService
{
    Task<IEnumerable<StoreItem>> GetActiveItemsAsync(CancellationToken ct = default);
    Task<UserInventoryItem> PurchaseAsync(Guid userId, Guid itemId, int quantity, CancellationToken ct = default);
}

/// <summary>User notifications (in-app) management.</summary>
public interface INotificationService
{
    Task<Notification> CreateAsync(Guid userId, string type, string title, string message, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetUnreadAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetAllAsync(Guid userId, int take = 100, CancellationToken ct = default);
    Task MarkReadAsync(Guid userId, Guid notificationId, CancellationToken ct = default);
    Task<int> MarkAllReadAsync(Guid userId, CancellationToken ct = default);
}

/// <summary>Inventory querying (owned artifacts, store items).</summary>
public interface IInventoryService
{
    Task<IEnumerable<UserInventoryItem>> GetStoreInventoryAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<UserArtifact>> GetArtifactsAsync(Guid userId, CancellationToken ct = default);
}

/// <summary>Basic analytics / aggregated metrics.</summary>
public interface IAnalyticsService
{
    Task<AnalyticsSummary> GetSummaryAsync(CancellationToken ct = default);
}
