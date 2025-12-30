using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Entities.Data;

public partial class WatchServiceDbContext : DbContext
{
    public WatchServiceDbContext(DbContextOptions<WatchServiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advertisement> Advertisements { get; set; }

    public virtual DbSet<Bid> Bids { get; set; }

    public virtual DbSet<Watch> Watches { get; set; }

    public virtual DbSet<WatchImage> WatchImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("TR_Advertisements_SetUpdatedAt"));

            entity.HasIndex(e => e.SellerUserId, "IX_Advertisements_SellerUserId");

            entity.HasIndex(e => e.Status, "IX_Advertisements_Status");

            entity.HasIndex(e => e.WatchId, "IX_Advertisements_WatchId");

            entity.Property(e => e.AdvertisementId).ValueGeneratedNever();
            entity.Property(e => e.AskingPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Watch).WithMany(p => p.Advertisements)
                .HasForeignKey(d => d.WatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Advertisements_Watches");
        });

        modelBuilder.Entity<Bid>(entity =>
        {
            entity.Property(e => e.BidId).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Advertisement).WithMany(p => p.Bids)
                .HasForeignKey(d => d.AdvertisementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bids_Advertisements");
        });

        modelBuilder.Entity<Watch>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("TR_Watches_SetUpdatedAt"));

            entity.HasIndex(e => e.Brand, "IX_Watches_Brand");

            entity.HasIndex(e => e.OwnerUserId, "IX_Watches_OwnerUserId");

            entity.Property(e => e.WatchId).ValueGeneratedNever();
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Condition).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Model).HasMaxLength(200);
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReferenceNumber).HasMaxLength(100);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<WatchImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);

            entity.ToTable(tb => tb.HasTrigger("TR_WatchImages_SetUploadedAt"));

            entity.HasIndex(e => e.WatchId, "IX_WatchImages_WatchId");

            entity.Property(e => e.ImageId).ValueGeneratedNever();
            entity.Property(e => e.BlobUrl).HasMaxLength(500);
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Watch).WithMany(p => p.WatchImages)
                .HasForeignKey(d => d.WatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WatchImages_Watches");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
