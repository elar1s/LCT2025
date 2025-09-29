using LctMonolith.Domain.Entities;

namespace LctMonolith.Services.Models;

/// <summary>Returned access+refresh token pair with expiry metadata.</summary>
public record TokenPair(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken, DateTime RefreshTokenExpiresAt);

/// <summary>Mission creation request model.</summary>
public class CreateMissionModel
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Branch { get; set; }
    public MissionCategory Category { get; set; }
    public Guid? MinRankId { get; set; }
    public int ExperienceReward { get; set; }
    public int ManaReward { get; set; }
    public List<CompetencyRewardModel> CompetencyRewards { get; set; } = new();
    public List<Guid> ArtifactRewardIds { get; set; } = new();
}

/// <summary>Competency reward definition for mission creation.</summary>
public class CompetencyRewardModel
{
    public Guid CompetencyId { get; set; }
    public int LevelDelta { get; set; }
    public int ProgressPointsDelta { get; set; }
}

/// <summary>Progress snapshot for UI: rank, xp, remaining requirements.</summary>
public class ProgressSnapshot
{
    public int Experience { get; set; }
    public int Mana { get; set; }
    public Guid? CurrentRankId { get; set; }
    public string? CurrentRankName { get; set; }
    public Guid? NextRankId { get; set; }
    public string? NextRankName { get; set; }
    public int? RequiredExperienceForNextRank { get; set; }
    public int? ExperienceRemaining => RequiredExperienceForNextRank.HasValue ? Math.Max(0, RequiredExperienceForNextRank.Value - Experience) : null;
    public List<Guid> OutstandingMissionIds { get; set; } = new();
    public List<OutstandingCompetency> OutstandingCompetencies { get; set; } = new();
}

/// <summary>Competency requirement still unmet for next rank.</summary>
public class OutstandingCompetency
{
    public Guid CompetencyId { get; set; }
    public string? CompetencyName { get; set; }
    public int RequiredLevel { get; set; }
    public int CurrentLevel { get; set; }
}

/// <summary>Request to update mission status with optional submission data.</summary>
public class UpdateMissionStatusRequest
{
    public MissionStatus Status { get; set; }
    public string? SubmissionData { get; set; }
}

/// <summary>Store purchase request.</summary>
public class PurchaseRequest
{
    public Guid ItemId { get; set; }
    public int Quantity { get; set; } = 1;
}

/// <summary>Authentication request (login/register) simple mock.</summary>
public class AuthRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

/// <summary>Refresh token request.</summary>
public class RefreshRequest
{
    public string RefreshToken { get; set; } = null!;
}

/// <summary>Revoke refresh token request.</summary>
public class RevokeRequest
{
    public string RefreshToken { get; set; } = null!;
}

/// <summary>Analytics summary for admin dashboard.</summary>
public class AnalyticsSummary
{
    public int TotalUsers { get; set; }
    public int TotalMissions { get; set; }
    public int CompletedMissions { get; set; }
    public int TotalArtifacts { get; set; }
    public int TotalStoreItems { get; set; }
    public long TotalExperience { get; set; }
    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
}
