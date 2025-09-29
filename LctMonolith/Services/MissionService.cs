using LctMonolith.Domain.Entities;
using LctMonolith.Infrastructure.UnitOfWork;
using LctMonolith.Services.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;
using System.Linq;

namespace LctMonolith.Services;

/// <summary>
/// Mission management and user mission state transitions.
/// </summary>
public class MissionService : IMissionService
{
    private readonly IUnitOfWork _uow;
    private readonly IGamificationService _gamification;

    public MissionService(IUnitOfWork uow, IGamificationService gamification)
    {
        _uow = uow;
        _gamification = gamification;
    }

    public async Task<Mission> CreateMissionAsync(CreateMissionModel model, CancellationToken ct = default)
    {
        var mission = new Mission
        {
            Title = model.Title,
            Description = model.Description,
            Branch = model.Branch,
            Category = model.Category,
            MinRankId = model.MinRankId,
            ExperienceReward = model.ExperienceReward,
            ManaReward = model.ManaReward,
            IsActive = true
        };
        await _uow.Missions.AddAsync(mission, ct);
        foreach (var cr in model.CompetencyRewards)
        {
            await _uow.MissionCompetencyRewards.AddAsync(new MissionCompetencyReward
            {
                MissionId = mission.Id,
                CompetencyId = cr.CompetencyId,
                LevelDelta = cr.LevelDelta,
                ProgressPointsDelta = cr.ProgressPointsDelta
            }, ct);
        }
        foreach (var artId in model.ArtifactRewardIds.Distinct())
        {
            await _uow.MissionArtifactRewards.AddAsync(new MissionArtifactReward
            {
                MissionId = mission.Id,
                ArtifactId = artId
            }, ct);
        }
        await _uow.SaveChangesAsync(ct);
        await LogEventAsync(EventType.MissionStatusChanged, null, new { action = "created", missionId = mission.Id }, ct);
        return mission;
    }

    public async Task<IEnumerable<Mission>> GetAvailableMissionsAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _uow.Users.Query(u => u.Id == userId, null, u => u.Rank).FirstOrDefaultAsync(ct)
                   ?? throw new KeyNotFoundException("User not found");
        var missions = await _uow.Missions.Query(m => m.IsActive, null, m => m.MinRank).ToListAsync(ct);
        var userOrder = user.Rank?.Order ?? int.MinValue;
        return missions.Where(m => m.MinRank == null || m.MinRank.Order <= userOrder + 1);
    }

    public async Task<UserMission> UpdateStatusAsync(Guid userId, Guid missionId, MissionStatus status, string? submissionData, CancellationToken ct = default)
    {
        var mission = await _uow.Missions.Query(m => m.Id == missionId).FirstOrDefaultAsync(ct)
                      ?? throw new KeyNotFoundException("Mission not found");
        var userMission = await _uow.UserMissions.FindAsync(userId, missionId);
        if (userMission == null)
        {
            userMission = new UserMission
            {
                UserId = userId,
                MissionId = missionId,
                Status = MissionStatus.Available
            };
            await _uow.UserMissions.AddAsync(userMission, ct);
        }
        userMission.Status = status;
        userMission.SubmissionData = submissionData;
        userMission.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        await LogEventAsync(EventType.MissionStatusChanged, userId, new { missionId, status }, ct);
        if (status == MissionStatus.Completed)
        {
            await _gamification.ApplyMissionCompletionAsync(userId, mission, ct);
            await LogEventAsync(EventType.RewardGranted, userId, new { missionId, mission.ExperienceReward, mission.ManaReward }, ct);
        }
        return userMission;
    }

    private async Task LogEventAsync(EventType type, Guid? userId, object data, CancellationToken ct)
    {
        if (userId == null && type != EventType.MissionStatusChanged) return;
        var evt = new EventLog
        {
            Type = type,
            UserId = userId ?? Guid.Empty,
            Data = JsonSerializer.Serialize(data)
        };
        await _uow.EventLogs.AddAsync(evt, ct);
        await _uow.SaveChangesAsync(ct);
        Log.Debug("Event {Type} logged {Data}", type, evt.Data);
    }
}
