namespace LctMonolith.Domain.Entities;

/// <summary>Artifact definition (unique reward objects).</summary>
public class Artifact
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public ArtifactRarity Rarity { get; set; }

    public ICollection<UserArtifact> Users { get; set; } = new List<UserArtifact>();
    public ICollection<MissionArtifactReward> MissionRewards { get; set; } = new List<MissionArtifactReward>();
}

/// <summary>Mapping artifact to user ownership.</summary>
public class UserArtifact
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public Guid ArtifactId { get; set; }
    public Artifact Artifact { get; set; } = null!;
    public DateTime ObtainedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>Reward mapping: mission grants artifact(s).</summary>
public class MissionArtifactReward
{
    public Guid MissionId { get; set; }
    public Mission Mission { get; set; } = null!;
    public Guid ArtifactId { get; set; }
    public Artifact Artifact { get; set; } = null!;
}

/// <summary>Item in store that can be purchased with mana.</summary>
public class StoreItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Price { get; set; }
    public bool IsActive { get; set; } = true;
    public int? Stock { get; set; }

    public ICollection<UserInventoryItem> UserInventory { get; set; } = new List<UserInventoryItem>();
}

/// <summary>User owned store item record.</summary>
public class UserInventoryItem
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public Guid StoreItemId { get; set; }
    public StoreItem StoreItem { get; set; } = null!;
    public int Quantity { get; set; } = 1;
    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
    public bool IsReturned { get; set; }
}

/// <summary>Transaction record for purchases/returns/sales.</summary>
public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public TransactionType Type { get; set; }
    public Guid? StoreItemId { get; set; }
    public StoreItem? StoreItem { get; set; }
    public int ManaAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>System event log for auditing user actions and progression.</summary>
public class EventLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public EventType Type { get; set; }
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public string? Data { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>Refresh token for JWT auth.</summary>
public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

