using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Entities.Data;

public partial class WatchServiceDbContext : DbContext 
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Watch>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                  .ValueGeneratedOnAdd()
                  .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            entity.Property(e => e.UpdatedAt)
                  .ValueGeneratedOnAddOrUpdate()
                  .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        });

        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                  .ValueGeneratedOnAdd()
                  .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            entity.Property(e => e.UpdatedAt)
                  .ValueGeneratedOnAddOrUpdate()
                  .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        });

        modelBuilder.Entity<WatchImage>(entity =>
        {
            entity.Property(e => e.UploadedAt)
                  .ValueGeneratedOnAddOrUpdate()
                  .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        });

        modelBuilder.Entity<Bid>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                  .ValueGeneratedOnAdd()
                  .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        });

         modelBuilder.Entity<Advertisement>()
            .HasOne(a => a.Watch)
            .WithMany(w => w.Advertisements)
            .HasForeignKey(a => a.WatchId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Advertisements_Watches");

        modelBuilder.Entity<WatchImage>()
            .HasOne(wi => wi.Watch)
            .WithMany(w => w.WatchImages)
            .HasForeignKey(wi => wi.WatchId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_WatchImages_Watches");

        modelBuilder.Entity<Bid>()
            .HasOne(b => b.Advertisement)
            .WithMany(a => a.Bids)
            .HasForeignKey(b => b.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Bids_Advertisements");
    }
}
