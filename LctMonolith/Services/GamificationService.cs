using System.Linq;
using LctMonolith.Domain.Entities;
using LctMonolith.Infrastructure.UnitOfWork;
using LctMonolith.Services.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LctMonolith.Services;

/// <summary>
/// Handles progression logic: mission completion rewards and rank advancement evaluation.
/// </summary>
public class GamificationService : IGamificationService
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notifications;

    public GamificationService(IUnitOfWork uow, INotificationService notifications)
    {
        _uow = uow;
        _notifications = notifications;
    }

    public async Task<ProgressSnapshot> GetProgressAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _uow.Users
            .Query(u => u.Id == userId, null, u => u.Rank, u => u.Competencies)
            .FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException("User not found");

        var ranks = await _uow.Ranks
            .Query(null, q => q.OrderBy(r => r.Order), r => r.RequiredMissions, r => r.RequiredCompetencies)
            .ToListAsync(ct);

        var currentOrder = user.Rank?.Order ?? -1;
        var nextRank = ranks.FirstOrDefault(r => r.Order == currentOrder + 1);
        var snapshot = new ProgressSnapshot
        {
            Experience = user.Experience,
            Mana = user.Mana,
            CurrentRankId = user.RankId,
            CurrentRankName = user.Rank?.Name,
            NextRankId = nextRank?.Id,
            NextRankName = nextRank?.Name,
            RequiredExperienceForNextRank = nextRank?.RequiredExperience
        };
        if (nextRank != null)
        {
            // Outstanding missions
            var userMissionIds = await _uow.UserMissions.Query(um => um.UserId == userId).Select(um => um.MissionId).ToListAsync(ct);
            foreach (var rm in nextRank.RequiredMissions)
            {
                if (!userMissionIds.Contains(rm.MissionId))
                    snapshot.OutstandingMissionIds.Add(rm.MissionId);
            }
            // Outstanding competencies
            foreach (var rc in nextRank.RequiredCompetencies)
            {
                var userComp = user.Competencies.FirstOrDefault(c => c.CompetencyId == rc.CompetencyId);
                var level = userComp?.Level ?? 0;
                if (level < rc.MinLevel)
                {
                    snapshot.OutstandingCompetencies.Add(new OutstandingCompetency
                    {
                        CompetencyId = rc.CompetencyId,
                        CompetencyName = rc.Competency?.Name,
                        RequiredLevel = rc.MinLevel,
                        CurrentLevel = level
                    });
                }
            }
        }
        return snapshot;
    }

    public async Task ApplyMissionCompletionAsync(Guid userId, Mission mission, CancellationToken ct = default)
    {
        var user = await _uow.Users
            .Query(u => u.Id == userId, null, u => u.Competencies, u => u.Rank)
            .FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException("User not found");

        user.Experience += mission.ExperienceReward;
        user.Mana += mission.ManaReward;
        user.UpdatedAt = DateTime.UtcNow;

        // Competency rewards
        var compRewards = await _uow.MissionCompetencyRewards.Query(m => m.MissionId == mission.Id).ToListAsync(ct);
        foreach (var reward in compRewards)
        {
            var uc = user.Competencies.FirstOrDefault(c => c.CompetencyId == reward.CompetencyId);
            if (uc == null)
            {
                uc = new UserCompetency
                {
                    UserId = userId,
                    CompetencyId = reward.CompetencyId,
                    Level = reward.LevelDelta,
                    ProgressPoints = reward.ProgressPointsDelta
                };
                await _uow.UserCompetencies.AddAsync(uc, ct);
            }
            else
            {
                uc.Level += reward.LevelDelta;
                uc.ProgressPoints += reward.ProgressPointsDelta;
            }
        }

        // Artifacts
        var artRewards = await _uow.MissionArtifactRewards.Query(m => m.MissionId == mission.Id).ToListAsync(ct);
        foreach (var ar in artRewards)
        {
            var existing = await _uow.UserArtifacts.FindAsync(userId, ar.ArtifactId);
            if (existing == null)
            {
                await _uow.UserArtifacts.AddAsync(new UserArtifact
                {
                    UserId = userId,
                    ArtifactId = ar.ArtifactId,
                    ObtainedAt = DateTime.UtcNow
                }, ct);
            }
        }

        await _uow.SaveChangesAsync(ct);
        await EvaluateRankUpgradeAsync(userId, ct);
    }

    public async Task EvaluateRankUpgradeAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _uow.Users
            .Query(u => u.Id == userId, null, u => u.Rank, u => u.Competencies)
            .FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException("User not found");

        var ranks = await _uow.Ranks
            .Query(null, q => q.OrderBy(r => r.Order), r => r.RequiredMissions, r => r.RequiredCompetencies)
            .ToListAsync(ct);

        var currentOrder = user.Rank?.Order ?? -1;
        var nextRank = ranks.FirstOrDefault(r => r.Order == currentOrder + 1);
        if (nextRank == null) return;

        if (user.Experience < nextRank.RequiredExperience) return;
        var completedMissionIds = await _uow.UserMissions
            .Query(um => um.UserId == userId && um.Status == MissionStatus.Completed)
            .Select(x => x.MissionId)
            .ToListAsync(ct);
        if (nextRank.RequiredMissions.Any(rm => !completedMissionIds.Contains(rm.MissionId))) return;
        foreach (var rc in nextRank.RequiredCompetencies)
        {
            var uc = user.Competencies.FirstOrDefault(c => c.CompetencyId == rc.CompetencyId);
            if (uc == null || uc.Level < rc.MinLevel)
                return;
        }
        user.RankId = nextRank.Id;
        user.Rank = nextRank;
        user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        Log.Information("User {UserId} promoted to rank {Rank}", userId, nextRank.Name);
        await _notifications.CreateAsync(userId, "rank", "Повышение ранга", $"Вы получили ранг '{nextRank.Name}'", ct);
    }
}
