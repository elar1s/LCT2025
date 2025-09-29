using LctMonolith.Domain.Entities;
using LctMonolith.Infrastructure.Data;
using LctMonolith.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace LctMonolith.Infrastructure.UnitOfWork;

/// <summary>
/// Unit of Work implementation encapsulating repositories and DB transaction scope.
/// </summary>
public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly AppDbContext _ctx;
    private IDbContextTransaction? _tx;

    public UnitOfWork(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    private IGenericRepository<AppUser>? _users;
    private IGenericRepository<Rank>? _ranks;
    private IGenericRepository<RankRequiredMission>? _rankRequiredMissions;
    private IGenericRepository<RankRequiredCompetency>? _rankRequiredCompetencies;
    private IGenericRepository<Mission>? _missions;
    private IGenericRepository<UserMission>? _userMissions;
    private IGenericRepository<MissionCompetencyReward>? _missionCompetencyRewards;
    private IGenericRepository<MissionArtifactReward>? _missionArtifactRewards;
    private IGenericRepository<Competency>? _competencies;
    private IGenericRepository<UserCompetency>? _userCompetencies;
    private IGenericRepository<Artifact>? _artifacts;
    private IGenericRepository<UserArtifact>? _userArtifacts;
    private IGenericRepository<StoreItem>? _storeItems;
    private IGenericRepository<UserInventoryItem>? _userInventoryItems;
    private IGenericRepository<Transaction>? _transactions;
    private IGenericRepository<EventLog>? _eventLogs;
    private IGenericRepository<RefreshToken>? _refreshTokens;
    private IGenericRepository<Notification>? _notifications;

    public IGenericRepository<AppUser> Users => _users ??= new GenericRepository<AppUser>(_ctx);
    public IGenericRepository<Rank> Ranks => _ranks ??= new GenericRepository<Rank>(_ctx);
    public IGenericRepository<RankRequiredMission> RankRequiredMissions => _rankRequiredMissions ??= new GenericRepository<RankRequiredMission>(_ctx);
    public IGenericRepository<RankRequiredCompetency> RankRequiredCompetencies => _rankRequiredCompetencies ??= new GenericRepository<RankRequiredCompetency>(_ctx);
    public IGenericRepository<Mission> Missions => _missions ??= new GenericRepository<Mission>(_ctx);
    public IGenericRepository<UserMission> UserMissions => _userMissions ??= new GenericRepository<UserMission>(_ctx);
    public IGenericRepository<MissionCompetencyReward> MissionCompetencyRewards => _missionCompetencyRewards ??= new GenericRepository<MissionCompetencyReward>(_ctx);
    public IGenericRepository<MissionArtifactReward> MissionArtifactRewards => _missionArtifactRewards ??= new GenericRepository<MissionArtifactReward>(_ctx);
    public IGenericRepository<Competency> Competencies => _competencies ??= new GenericRepository<Competency>(_ctx);
    public IGenericRepository<UserCompetency> UserCompetencies => _userCompetencies ??= new GenericRepository<UserCompetency>(_ctx);
    public IGenericRepository<Artifact> Artifacts => _artifacts ??= new GenericRepository<Artifact>(_ctx);
    public IGenericRepository<UserArtifact> UserArtifacts => _userArtifacts ??= new GenericRepository<UserArtifact>(_ctx);
    public IGenericRepository<StoreItem> StoreItems => _storeItems ??= new GenericRepository<StoreItem>(_ctx);
    public IGenericRepository<UserInventoryItem> UserInventoryItems => _userInventoryItems ??= new GenericRepository<UserInventoryItem>(_ctx);
    public IGenericRepository<Transaction> Transactions => _transactions ??= new GenericRepository<Transaction>(_ctx);
    public IGenericRepository<EventLog> EventLogs => _eventLogs ??= new GenericRepository<EventLog>(_ctx);
    public IGenericRepository<RefreshToken> RefreshTokens => _refreshTokens ??= new GenericRepository<RefreshToken>(_ctx);
    public IGenericRepository<Notification> Notifications => _notifications ??= new GenericRepository<Notification>(_ctx);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _ctx.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_tx != null) throw new InvalidOperationException("Transaction already started");
        _tx = await _ctx.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_tx == null) return;
        try
        {
            await _ctx.SaveChangesAsync(ct);
            await _tx.CommitAsync(ct);
        }
        catch
        {
            await RollbackAsync(ct);
            throw;
        }
        finally
        {
            await _tx.DisposeAsync();
            _tx = null;
        }
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_tx == null) return;
        await _tx.RollbackAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_tx != null) await _tx.DisposeAsync();
    }
}
