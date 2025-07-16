using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmokeQuit.Repository.LocDPX.Models;
using System;
using System.Collections.Generic;

namespace SmokeQuit.Repository.LocDPX.DBContext;

public partial class SmokeQuitDbContext : DbContext
{
    public SmokeQuitDbContext()
    {
    }

    public SmokeQuitDbContext(DbContextOptions<SmokeQuitDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatsLocDpx> ChatsLocDpxes { get; set; }

    public virtual DbSet<CoachesLocDpx> CoachesLocDpxes { get; set; }

    public virtual DbSet<SystemUserAccount> SystemUserAccounts { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=LOCDANG;Initial Catalog=SU25_PRN232_SE1731_G6_SmokeQuit;User Id=sa;Password=12345;Encrypt=True;TrustServerCertificate=True");

    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatsLocDpx>(entity =>
        {
            entity.HasKey(e => e.ChatsLocDpxid).HasName("PK__ChatsLoc__27DEBDA071FD33DF");

            entity.ToTable("ChatsLocDPX");

            entity.Property(e => e.ChatsLocDpxid).HasColumnName("ChatsLocDPXID");
            entity.Property(e => e.AttachmentUrl).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.MessageType).HasMaxLength(10);
            entity.Property(e => e.ResponseTime).HasColumnType("datetime");
            entity.Property(e => e.SentBy).HasMaxLength(10);

            entity.HasOne(d => d.Coach).WithMany(p => p.ChatsLocDpxes)
                .HasForeignKey(d => d.CoachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatsLocD__Coach__3C69FB99");

            entity.HasOne(d => d.User).WithMany(p => p.ChatsLocDpxes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatsLocD__UserI__3D5E1FD2");
        });

        modelBuilder.Entity<CoachesLocDpx>(entity =>
        {
            entity.HasKey(e => e.CoachesLocDpxid).HasName("PK__CoachesL__24751D181F715113");

            entity.ToTable("CoachesLocDPX");

            entity.HasIndex(e => e.Email, "UQ__CoachesL__A9D10534E02CF93A").IsUnique();

            entity.Property(e => e.CoachesLocDpxid).HasColumnName("CoachesLocDPXID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SystemUserAccount>(entity =>
        {
            entity.HasKey(e => e.UserAccountId);

            entity.ToTable("System.UserAccount");

            entity.Property(e => e.UserAccountId).HasColumnName("UserAccountID");
            entity.Property(e => e.ApplicationCode).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.RequestCode).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
