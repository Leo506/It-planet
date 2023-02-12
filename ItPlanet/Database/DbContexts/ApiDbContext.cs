using ItPlanet.Models;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Database.DbContexts;

public partial class ApiDbContext : DbContext
{
    public ApiDbContext()
    {
    }

    public ApiDbContext(DbContextOptions<ApiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<AnimalType> AnimalTypes { get; set; }

    public virtual DbSet<LocationPoint> LocationPoints { get; set; }

    public virtual DbSet<VisitedPoint> VisitedPoints { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User Id=admin;Password=password;Host=localhost;Port=5432;Database=Test");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Animal_pk");

            entity.ToTable("Animal");

            entity.Property(e => e.ChippingDateTime).HasColumnType("timestamp without time zone");
            entity.Property(e => e.DeathDateTime).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.LifeStatus).HasMaxLength(50);

            entity.HasMany(d => d.Types).WithMany(p => p.Animals)
                .UsingEntity<Dictionary<string, object>>(
                    "AnimalToType",
                    r => r.HasOne<AnimalType>().WithMany()
                        .HasForeignKey("TypeId")
                        .HasConstraintName("AnimalToTypes_AnimalTypes_null_fk"),
                    l => l.HasOne<Animal>().WithMany()
                        .HasForeignKey("AnimalId")
                        .HasConstraintName("AnimalToTypes_Animal_null_fk"),
                    j => { j.HasKey("AnimalId", "TypeId").HasName("AnimalToTypes_pk"); });
        });

        modelBuilder.Entity<AnimalType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AnimalTypes_pk");

            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
        });

        modelBuilder.Entity<LocationPoint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LocationPoints_pk");

            entity.Property(e => e.Longitude).HasColumnName("longitude");
        });

        modelBuilder.Entity<VisitedPoint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("VisitedPoints_pk");

            entity.Property(e => e.DateTimeOfVisitLocationPoint).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Animal).WithMany(p => p.VisitedPoints)
                .HasForeignKey(d => d.AnimalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("VisitedPoints_Animal_null_fk");

            entity.HasOne(d => d.LocationPoint).WithMany(p => p.VisitedPoints)
                .HasForeignKey(d => d.LocationPointId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("VisitedPoints_LocationPoints_null_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}