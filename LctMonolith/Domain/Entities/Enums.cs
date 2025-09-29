namespace LctMonolith.Domain.Entities;

/// <summary>Mission category taxonomy.</summary>
public enum MissionCategory
{
    Quest = 0,
    Recruiting = 1,
    Lecture = 2,
    Simulator = 3
}

/// <summary>Status of a mission for a specific user.</summary>
public enum MissionStatus
{
    Locked = 0,
    Available = 1,
    InProgress = 2,
    Submitted = 3,
    Completed = 4,
    Rejected = 5
}

/// <summary>Rarity level of an artifact.</summary>
public enum ArtifactRarity
{
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3
}

/// <summary>Type of transactional operation in store.</summary>
public enum TransactionType
{
    Purchase = 0,
    Return = 1,
    Sale = 2
}

/// <summary>Auditable event types enumerated in requirements.</summary>
public enum EventType
{
    SkillProgress = 1,
    MissionStatusChanged = 2,
    RankChanged = 3,
    ItemPurchased = 4,
    ArtifactObtained = 5,
    RewardGranted = 6,
    ProfileChanged = 7,
    AuthCredentialsChanged = 8,
    ItemReturned = 9,
    ItemSold = 10
}

