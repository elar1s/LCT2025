namespace LctMonolith.Domain.Entities;

/// <summary>
/// Competency (skill) that can be progressed by completing missions.
/// </summary>
public class Competency
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<UserCompetency> UserCompetencies { get; set; } = new List<UserCompetency>();
    public ICollection<MissionCompetencyReward> MissionRewards { get; set; } = new List<MissionCompetencyReward>();
    public ICollection<RankRequiredCompetency> RankRequirements { get; set; } = new List<RankRequiredCompetency>();
}

/// <summary>Per-user competency level.</summary>
public class UserCompetency
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public Guid CompetencyId { get; set; }
    public Competency Competency { get; set; } = null!;
    /// <summary>Current level (integer simple scale).</summary>
    public int Level { get; set; }
    /// <summary>Optional numeric progress inside level (e.g., partial points).</summary>
    public int ProgressPoints { get; set; }
}

/// <summary>Reward mapping: mission increases competency level points.</summary>
public class MissionCompetencyReward
{
    public Guid MissionId { get; set; }
    public Mission Mission { get; set; } = null!;
    public Guid CompetencyId { get; set; }
    public Competency Competency { get; set; } = null!;
    /// <summary>Increment value in levels (could be 0 or 1) or points depending on design.</summary>
    public int LevelDelta { get; set; }
    public int ProgressPointsDelta { get; set; }
}

