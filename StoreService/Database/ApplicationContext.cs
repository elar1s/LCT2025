using Microsoft.EntityFrameworkCore;
using StoreService.Database.Entities;

namespace StoreService.Database;

/// <summary>
/// Entity Framework Core database context for the Store microservice.
/// Defines DbSets and configures entity relationships & constraints.
/// </summary>
public class ApplicationContext : DbContext
{
    #region Ctor
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }
    #endregion

    #region DbSets
    public DbSet<StoreCategory> StoreCategories => Set<StoreCategory>();
    public DbSet<StoreItem> StoreItems => Set<StoreItem>();
    public DbSet<StoreDiscount> StoreDiscounts => Set<StoreDiscount>();
    public DbSet<StoreDiscountItem> StoreDiscountItems => Set<StoreDiscountItem>();
    public DbSet<StoreOrder> StoreOrders => Set<StoreOrder>();
    public DbSet<StoreOrderItem> StoreOrderItems => Set<StoreOrderItem>();
    public DbSet<StoreOrderItemDiscount> StoreOrderItemDiscounts => Set<StoreOrderItemDiscount>();
    #endregion

    #region ModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // store_category
        modelBuilder.Entity<StoreCategory>(b =>
        {
            b.ToTable("store_category");
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).HasMaxLength(256).IsRequired();
        });

        // store_item
        modelBuilder.Entity<StoreItem>(b =>
        {
            b.ToTable("store_item");
            b.HasKey(x => x.Id);
            b.Property(x => x.ManaBuyPrice).IsRequired();
            b.Property(x => x.ManaSellPrice).IsRequired();
            b.Property(x => x.InventoryLimit).IsRequired();
            b.Property(x => x.UnlimitedPurchase).HasDefaultValue(false);
            b.HasOne(x => x.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(x => x.StoreCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // store_discount
        modelBuilder.Entity<StoreDiscount>(b =>
        {
            b.ToTable("store_discount");
            b.HasKey(x => x.Id);
            b.Property(x => x.Percentage).HasPrecision(5, 2); // up to 100.00
            b.Property(x => x.FromDate).IsRequired();
            b.Property(x => x.UntilDate).IsRequired();
            b.Property(x => x.IsCanceled).HasDefaultValue(false);
        });

        // store_discount_item (many-to-many manual mapping)
        modelBuilder.Entity<StoreDiscountItem>(b =>
        {
            b.ToTable("store_discount_item");
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Discount)
                .WithMany(d => d.DiscountItems)
                .HasForeignKey(x => x.StoreDiscountId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.StoreItem)
                .WithMany(i => i.DiscountItems)
                .HasForeignKey(x => x.StoreItemId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => new { x.StoreDiscountId, x.StoreItemId }).IsUnique();
        });

        // store_order
        modelBuilder.Entity<StoreOrder>(b =>
        {
            b.ToTable("store_order");
            b.HasKey(x => x.Id);
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.CostUpdateDate).IsRequired();
            b.Property(x => x.ItemsRedeemed).HasDefaultValue(false);
        });

        // store_order_item
        modelBuilder.Entity<StoreOrderItem>(b =>
        {
            b.ToTable("store_order_item");
            b.HasKey(x => x.Id);
            b.Property(x => x.CalculatedPrice).IsRequired();
            b.HasOne(x => x.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(x => x.StoreOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.StoreItem)
                .WithMany(i => i.OrderItems)
                .HasForeignKey(x => x.StoreItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // store_order_item_discount
        modelBuilder.Entity<StoreOrderItemDiscount>(b =>
        {
            b.ToTable("store_order_item_discount");
            b.HasKey(x => x.Id);
            b.HasOne(x => x.OrderItem)
                .WithMany(oi => oi.AppliedDiscounts)
                .HasForeignKey(x => x.StoreOrderItemId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Discount)
                .WithMany(d => d.OrderItemDiscounts)
                .HasForeignKey(x => x.StoreDiscountId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new { x.StoreOrderItemId, x.StoreDiscountId }).IsUnique();
        });
    }
    #endregion
}

