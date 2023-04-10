using ItPlanet.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Infrastructure.DatabaseContext;

public partial class ApiDbContext : DbContext
{
    public ApiDbContext()
    {
        Database.EnsureCreated();
    }

    public ApiDbContext(DbContextOptions<ApiDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<AnimalType> AnimalTypes { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<AreaPoint> AreaPoints { get; set; }

    public virtual DbSet<LocationPoint> LocationPoints { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<VisitedPoint> VisitedPoints { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Accounts_Email").IsUnique();

            entity.HasIndex(e => e.RoleId, "IX_Accounts_RoleId");

            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasDefaultValueSql("'password'::character varying");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Accounts_Roles_Id_fk");
        });

        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Animal_pk");

            entity.ToTable("Animal");

            entity.HasIndex(e => e.ChipperId, "IX_Animal_ChipperId");

            entity.Property(e => e.ChippingDateTime).HasColumnType("timestamp without time zone");
            entity.Property(e => e.DeathDateTime).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.LifeStatus).HasMaxLength(50);

            entity.HasOne(d => d.Chipper).WithMany(p => p.Animals)
                .HasForeignKey(d => d.ChipperId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Animal_Accounts_null_fk");

            entity.HasOne(d => d.ChippingLocation).WithMany(p => p.Animals)
                .HasForeignKey(d => d.ChippingLocationId)
                .HasConstraintName("Animal_LocationPoints_Id_fk");

            entity.HasMany(d => d.Types).WithMany(p => p.Animals)
                .UsingEntity<Dictionary<string, object>>(
                    "AnimalToType",
                    r => r.HasOne<AnimalType>().WithMany()
                        .HasForeignKey("TypeId")
                        .HasConstraintName("AnimalToTypes_AnimalTypes_null_fk"),
                    l => l.HasOne<Animal>().WithMany()
                        .HasForeignKey("AnimalId")
                        .HasConstraintName("AnimalToTypes_Animal_null_fk"),
                    j =>
                    {
                        j.HasKey("AnimalId", "TypeId").HasName("AnimalToTypes_pk");
                        j.ToTable("AnimalToType");
                        j.HasIndex(new[] { "TypeId" }, "IX_AnimalToTypes_TypeId");
                    });
        });

        modelBuilder.Entity<AnimalType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AnimalTypes_pk");

            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Areas_pk");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<AreaPoint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AreaPoints_pk");

            entity.HasIndex(e => e.AreaId, "IX_AreaPoints_AreaId");

            entity.Property(e => e.Longitude).HasColumnName("Longitude ");

            entity.HasOne(d => d.Area).WithMany(p => p.AreaPoints)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("AreaPoints_Areas_Id_fk");
        });

        modelBuilder.Entity<LocationPoint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LocationPoints_pk");

            entity.Property(e => e.Longitude).HasColumnName("longitude");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Roles_pk");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<VisitedPoint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("VisitedPoints_pk");

            entity.HasIndex(e => e.AnimalId, "IX_VisitedPoints_AnimalId");

            entity.HasIndex(e => e.LocationPointId, "IX_VisitedPoints_LocationPointId");

            entity.Property(e => e.DateTimeOfVisitLocationPoint).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Animal).WithMany(p => p.VisitedPoints)
                .HasForeignKey(d => d.AnimalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("VisitedPoints_Animal_null_fk");

            entity.HasOne(d => d.LocationPoint).WithMany(p => p.VisitedPoints)
                .HasForeignKey(d => d.LocationPointId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("VisitedPoints_LocationPoints_null_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
