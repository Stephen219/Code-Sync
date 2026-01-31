using FileSync.Domain.entities;
using FileSync.Domain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace FileSync.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Space> Spaces => Set<Space>();
        public DbSet<SpaceFile> SpaceFiles => Set<SpaceFile>();
        public DbSet<FileDrop> Drops => Set<FileDrop>();
        public DbSet<DropFile> DropFiles => Set<DropFile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Space configuration
            modelBuilder.Entity<Space>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.HasMany(e => e.Files)
                      .WithOne(e => e.Space)
                      .HasForeignKey(e => e.SpaceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Drop configuration    file drop  
            modelBuilder.Entity<FileDrop>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.HasMany(e => e.Files)
                      .WithOne(e => e.Drop)
                      .HasForeignKey(e => e.DropId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}


