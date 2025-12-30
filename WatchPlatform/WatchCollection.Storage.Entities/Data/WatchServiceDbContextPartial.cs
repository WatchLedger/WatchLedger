using System;
using Microsoft.EntityFrameworkCore;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Storage.Entities.Data;

public partial class WatchServiceDbContext : DbContext 
{
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Watch>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                  .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                  .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<WatchImage>(entity =>
        {
            entity.Property(e => e.UploadedAt)
                  .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<Bid>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                  .ValueGeneratedOnAdd();
        });
    }
}
