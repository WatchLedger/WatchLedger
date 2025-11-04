using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Entities.Data;

public partial class WatchServiceDbContext : DbContext
{
    public WatchServiceDbContext()
    {
    }

    public WatchServiceDbContext(DbContextOptions<WatchServiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advertisement> Advertisements { get; set; }

    public virtual DbSet<Watch> Watches { get; set; }

    public virtual DbSet<WatchImage> WatchImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.HasKey(e => e.AdvertisementId).HasName("PK__Advertis__C4C7F4CD255E4D14");

            entity.HasIndex(e => e.SellerUserId, "IX_Advertisements_SellerUserId");

            entity.HasIndex(e => e.Status, "IX_Advertisements_Status");

            entity.HasIndex(e => e.WatchId, "IX_Advertisements_WatchId");

            entity.Property(e => e.AdvertisementId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AskingPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.PublishedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Watch).WithMany(p => p.Advertisements)
                .HasForeignKey(d => d.WatchId)
                .HasConstraintName("FK_Advertisements_Watches");
        });

        modelBuilder.Entity<Watch>(entity =>
        {
            entity.HasKey(e => e.WatchId).HasName("PK__Watches__3BA3DAA34CEBC149");

            entity.HasIndex(e => e.Brand, "IX_Watches_Brand");

            entity.HasIndex(e => e.IsForSale, "IX_Watches_IsForSale");

            entity.HasIndex(e => e.OwnerUserId, "IX_Watches_OwnerUserId");

            entity.Property(e => e.WatchId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Condition).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.IsForSale).HasDefaultValue(false);
            entity.Property(e => e.Model).HasMaxLength(200);
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReferenceNumber).HasMaxLength(100);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
        });

        modelBuilder.Entity<WatchImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__WatchIma__7516F70CE18A7D7A");

            entity.HasIndex(e => e.WatchId, "IX_WatchImages_WatchId");

            entity.Property(e => e.ImageId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BlobUrl).HasMaxLength(500);
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.Watch).WithMany(p => p.WatchImages)
                .HasForeignKey(d => d.WatchId)
                .HasConstraintName("FK_WatchImages_Watches");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
