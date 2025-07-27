using Microsoft.EntityFrameworkCore;
using ChineseAuction.Models.Entities;

namespace ChineseAuction.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Donor> Donors { get; set; }
    public DbSet<Gift> Gifts { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
        });

        // Donor configuration
        modelBuilder.Entity<Donor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        // Gift configuration
        modelBuilder.Entity<Gift>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Donor)
                .WithMany(d => d.Gifts)
                .HasForeignKey(e => e.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Winner)
                .WithMany()
                .HasForeignKey(e => e.WinnerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Purchase configuration
        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PricePaid).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Gift)
                .WithMany(g => g.Purchases)
                .HasForeignKey(e => e.GiftId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CartItem configuration
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Gift)
                .WithMany(g => g.CartItems)
                .HasForeignKey(e => e.GiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure a user can't have the same gift multiple times in cart
            entity.HasIndex(e => new { e.UserId, e.GiftId }).IsUnique();
        });

        // Seed manager user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Manager",
                Email = "manager@auction.com",
                Phone = "555-0001",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                Role = "manager",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}