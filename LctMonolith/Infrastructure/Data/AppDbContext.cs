using LctMonolith.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LctMonolith.Infrastructure.Data;

/// <summary>
/// Main EF Core database context for gamification module (PostgreSQL provider expected).
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Rank> Ranks => Set<Rank>();
    public DbSet<RankRequiredMission> RankRequiredMissions => Set<RankRequiredMission>();
    public DbSet<RankRequiredCompetency> RankRequiredCompetencies => Set<RankRequiredCompetency>();

    public DbSet<Mission> Missions => Set<Mission>();
    public DbSet<UserMission> UserMissions => Set<UserMission>();
    public DbSet<MissionCompetencyReward> MissionCompetencyRewards => Set<MissionCompetencyReward>();
    public DbSet<MissionArtifactReward> MissionArtifactRewards => Set<MissionArtifactReward>();

    public DbSet<Competency> Competencies => Set<Competency>();
    public DbSet<UserCompetency> UserCompetencies => Set<UserCompetency>();

    public DbSet<Artifact> Artifacts => Set<Artifact>();
    public DbSet<UserArtifact> UserArtifacts => Set<UserArtifact>();

    public DbSet<StoreItem> StoreItems => Set<StoreItem>();
    public DbSet<UserInventoryItem> UserInventoryItems => Set<UserInventoryItem>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public DbSet<EventLog> EventLogs => Set<EventLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Rank required mission composite key
        b.Entity<RankRequiredMission>().HasKey(x => new { x.RankId, x.MissionId });
        b.Entity<RankRequiredMission>()
            .HasOne(x => x.Rank).WithMany(r => r.RequiredMissions).HasForeignKey(x => x.RankId);
        b.Entity<RankRequiredMission>()
            .HasOne(x => x.Mission).WithMany(m => m.RanksRequiring).HasForeignKey(x => x.MissionId);

        // Rank required competency composite key
        b.Entity<RankRequiredCompetency>().HasKey(x => new { x.RankId, x.CompetencyId });
        b.Entity<RankRequiredCompetency>()
            .HasOne(x => x.Rank).WithMany(r => r.RequiredCompetencies).HasForeignKey(x => x.RankId);
        b.Entity<RankRequiredCompetency>()
            .HasOne(x => x.Competency).WithMany(c => c.RankRequirements).HasForeignKey(x => x.CompetencyId);

        // UserMission composite key
        b.Entity<UserMission>().HasKey(x => new { x.UserId, x.MissionId });
        b.Entity<UserMission>()
            .HasOne(x => x.User).WithMany(u => u.Missions).HasForeignKey(x => x.UserId);
        b.Entity<UserMission>()
            .HasOne(x => x.Mission).WithMany(m => m.UserMissions).HasForeignKey(x => x.MissionId);

        // UserCompetency composite key
        b.Entity<UserCompetency>().HasKey(x => new { x.UserId, x.CompetencyId });
        b.Entity<UserCompetency>()
            .HasOne(x => x.User).WithMany(u => u.Competencies).HasForeignKey(x => x.UserId);
        b.Entity<UserCompetency>()
            .HasOne(x => x.Competency).WithMany(c => c.UserCompetencies).HasForeignKey(x => x.CompetencyId);

        // Mission competency reward composite key
        b.Entity<MissionCompetencyReward>().HasKey(x => new { x.MissionId, x.CompetencyId });

        // Mission artifact reward composite key
        b.Entity<MissionArtifactReward>().HasKey(x => new { x.MissionId, x.ArtifactId });

        // UserArtifact composite key
        b.Entity<UserArtifact>().HasKey(x => new { x.UserId, x.ArtifactId });

        // UserInventory composite key
        b.Entity<UserInventoryItem>().HasKey(x => new { x.UserId, x.StoreItemId });

        // Refresh token index unique
        b.Entity<RefreshToken>().HasIndex(x => x.Token).IsUnique();

        // ---------- Added performance indexes ----------
        b.Entity<AppUser>().HasIndex(u => u.RankId);
        b.Entity<Mission>().HasIndex(m => m.MinRankId);
        b.Entity<UserMission>().HasIndex(um => new { um.UserId, um.Status });
        b.Entity<UserCompetency>().HasIndex(uc => uc.CompetencyId); // for querying all users by competency
        b.Entity<EventLog>().HasIndex(e => new { e.UserId, e.Type, e.CreatedAt });
        b.Entity<StoreItem>().HasIndex(i => i.IsActive);
        b.Entity<Transaction>().HasIndex(t => new { t.UserId, t.CreatedAt });
        b.Entity<Notification>().HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt });
    }
}
