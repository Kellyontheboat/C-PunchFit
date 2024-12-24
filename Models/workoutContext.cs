using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication_punchFit.Models;

public partial class workoutContext : DbContext
{
    public workoutContext(DbContextOptions<workoutContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Exercises> Exercises { get; set; }

    public virtual DbSet<Members> Members { get; set; }

    public virtual DbSet<ModuleItems> ModuleItems { get; set; }

    public virtual DbSet<Modules> Modules { get; set; }

    public virtual DbSet<Parts> Parts { get; set; }

    public virtual DbSet<Sections> Sections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exercises>(entity =>
        {
            entity.HasIndex(e => e.parts_id, "IX_Exercises_Parts");

            entity.Property(e => e.category).HasMaxLength(50);
            entity.Property(e => e.equipment).HasMaxLength(50);
            entity.Property(e => e.force).HasMaxLength(50);
            entity.Property(e => e.level).HasMaxLength(50);
            entity.Property(e => e.mechanic).HasMaxLength(50);
            entity.Property(e => e.name).HasMaxLength(255);

            entity.HasOne(d => d.parts).WithMany(p => p.Exercises)
                .HasForeignKey(d => d.parts_id)
                .HasConstraintName("FK_Exercises_Parts");
        });

        modelBuilder.Entity<Members>(entity =>
        {
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.password).HasMaxLength(255);
            entity.Property(e => e.time)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.username).HasMaxLength(255);
        });

        modelBuilder.Entity<ModuleItems>(entity =>
        {
            entity.HasIndex(e => e.exercise_id, "IX_ModuleItems_exercise_id");

            entity.HasIndex(e => e.module_id, "IX_ModuleItems_module_id");

            entity.Property(e => e.weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.exercise).WithMany(p => p.ModuleItems)
                .HasForeignKey(d => d.exercise_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("moduleitems_ibfk_1");

            entity.HasOne(d => d.module).WithMany(p => p.ModuleItems)
                .HasForeignKey(d => d.module_id)
                .HasConstraintName("moduleitems_ibfk_2");
        });

        modelBuilder.Entity<Modules>(entity =>
        {
            entity.HasIndex(e => e.member_id, "IX_Modules_member_id");

            entity.HasIndex(e => e.section_id, "IX_Modules_section_id");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.module_name).HasMaxLength(255);

            entity.HasOne(d => d.member).WithMany(p => p.Modules)
                .HasForeignKey(d => d.member_id)
                .HasConstraintName("Modules_ibfk_1");

            entity.HasOne(d => d.section).WithMany(p => p.Modules)
                .HasForeignKey(d => d.section_id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Modules_ibfk_2");
        });

        modelBuilder.Entity<Parts>(entity =>
        {
            entity.Property(e => e.part_name).HasMaxLength(255);

            entity.HasOne(d => d.sections).WithMany(p => p.Parts)
                .HasForeignKey(d => d.sections_id)
                .HasConstraintName("FK_Parts_Sections");
        });

        modelBuilder.Entity<Sections>(entity =>
        {
            entity.Property(e => e.section_img).HasMaxLength(255);
            entity.Property(e => e.section_name).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
