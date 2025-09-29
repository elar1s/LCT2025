namespace LctMonolith.Domain.Entities;

/// <summary>
/// Mission (task) definition configured by HR.
/// </summary>
public class Mission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    /// <summary>Optional branch (path) name for grouping / visualization.</summary>
    public string? Branch { get; set; }
    public MissionCategory Category { get; set; }
    /// <summary>Minimum rank required to access the mission (nullable = available from start).</summary>
    public Guid? MinRankId { get; set; }
    public Rank? MinRank { get; set; }
    /// <summary>Experience reward on completion.</summary>
    public int ExperienceReward { get; set; }
    /// <summary>Mana reward on completion.</summary>
    public int ManaReward { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<MissionCompetencyReward> CompetencyRewards { get; set; } = new List<MissionCompetencyReward>();
    public ICollection<MissionArtifactReward> ArtifactRewards { get; set; } = new List<MissionArtifactReward>();
    public ICollection<UserMission> UserMissions { get; set; } = new List<UserMission>();
    public ICollection<RankRequiredMission> RanksRequiring { get; set; } = new List<RankRequiredMission>();
}

/// <summary>Per-user mission status and progression.</summary>
public class UserMission
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public Guid MissionId { get; set; }
    public Mission Mission { get; set; } = null!;

    public MissionStatus Status { get; set; } = MissionStatus.Available;
    /// <summary>Date/time of last status change.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>Optional submission payload (e.g., link, text, attachments pointer).</summary>
    public string? SubmissionData { get; set; }
}

