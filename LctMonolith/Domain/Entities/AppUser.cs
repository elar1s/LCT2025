using Microsoft.AspNetCore.Identity;

namespace LctMonolith.Domain.Entities;

/// <summary>
/// Application user (candidate or employee) participating in gamification.
/// Extends IdentityUser with Guid primary key.
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    /// <summary>User given (first) name.</summary>
    public string? FirstName { get; set; }
    /// <summary>User family (last) name.</summary>
    public string? LastName { get; set; }
    /// <summary>Date of birth.</summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>Current accumulated experience points.</summary>
    public int Experience { get; set; }
    /// <summary>Current mana (in-game currency).</summary>
    public int Mana { get; set; }

    /// <summary>Current rank reference.</summary>
    public Guid? RankId { get; set; }
    public Rank? Rank { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserCompetency> Competencies { get; set; } = new List<UserCompetency>();
    public ICollection<UserMission> Missions { get; set; } = new List<UserMission>();
    public ICollection<UserInventoryItem> Inventory { get; set; } = new List<UserInventoryItem>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<EventLog> Events { get; set; } = new List<EventLog>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
