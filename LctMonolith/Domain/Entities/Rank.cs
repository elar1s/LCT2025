namespace LctMonolith.Domain.Entities;

/// <summary>
/// Linear rank in progression ladder. User must meet XP, key mission and competency requirements.
/// </summary>
public class Rank
{
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>Display name (e.g., "Искатель", "Пилот-кандидат").</summary>
    public string Name { get; set; } = null!;
    /// <summary>Ordering position. Lower value = earlier rank.</summary>
    public int Order { get; set; }
    /// <summary>Required cumulative experience to attain this rank.</summary>
    public int RequiredExperience { get; set; }

    public ICollection<RankRequiredMission> RequiredMissions { get; set; } = new List<RankRequiredMission>();
    public ICollection<RankRequiredCompetency> RequiredCompetencies { get; set; } = new List<RankRequiredCompetency>();
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}

/// <summary>Mapping of rank to required mission.</summary>
public class RankRequiredMission
{
    public Guid RankId { get; set; }
    public Rank Rank { get; set; } = null!;
    public Guid MissionId { get; set; }
    public Mission Mission { get; set; } = null!;
}

/// <summary>Mapping of rank to required competency minimum level.</summary>
public class RankRequiredCompetency
{
    public Guid RankId { get; set; }
    public Rank Rank { get; set; } = null!;
    public Guid CompetencyId { get; set; }
    public Competency Competency { get; set; } = null!;
    /// <summary>Minimum level required for the competency.</summary>
    public int MinLevel { get; set; }
}

