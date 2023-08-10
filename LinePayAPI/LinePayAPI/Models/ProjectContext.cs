using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LinePayAPI.Models;

public partial class ProjectContext : DbContext
{
    public ProjectContext()
    {
    }

    public ProjectContext(DbContextOptions<ProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookCoach> BookCoaches { get; set; }

    public virtual DbSet<TCoach> TCoaches { get; set; }

    public virtual DbSet<TMember> TMembers { get; set; }

    public virtual DbSet<TOrder> TOrders { get; set; }

    public virtual DbSet<TOrderDetail> TOrderDetails { get; set; }

    public virtual DbSet<TProduct> TProducts { get; set; }

    public virtual DbSet<TShoppingCar> TShoppingCars { get; set; }

    public virtual DbSet<Tmyf> Tmyfs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookCoach>(entity =>
        {
            entity.HasKey(e => e.FId);

            entity.ToTable("BookCoach");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.CDate)
                .HasColumnType("date")
                .HasColumnName("cDate");
            entity.Property(e => e.CId)
                .HasMaxLength(50)
                .HasColumnName("cId");
            entity.Property(e => e.CName)
                .HasMaxLength(50)
                .HasColumnName("cName");
            entity.Property(e => e.CTime).HasColumnName("cTime");
            entity.Property(e => e.FUserId)
                .HasMaxLength(50)
                .HasColumnName("fUserId");
        });

        modelBuilder.Entity<TCoach>(entity =>
        {
            entity.HasKey(e => e.FId).HasName("PK_Coach");

            entity.ToTable("tCoach");

            entity.Property(e => e.FId)
                .ValueGeneratedNever()
                .HasColumnName("fId");
            entity.Property(e => e.CEmail)
                .HasMaxLength(50)
                .HasColumnName("cEmail");
            entity.Property(e => e.CGender)
                .HasMaxLength(50)
                .HasColumnName("cGender");
            entity.Property(e => e.CId)
                .HasMaxLength(50)
                .HasColumnName("cId");
            entity.Property(e => e.CImg)
                .HasMaxLength(50)
                .HasColumnName("cImg");
            entity.Property(e => e.CInterest)
                .HasMaxLength(50)
                .HasColumnName("cInterest");
            entity.Property(e => e.CItem)
                .HasMaxLength(50)
                .HasColumnName("cItem");
            entity.Property(e => e.CLineid)
                .HasMaxLength(50)
                .HasColumnName("cLineid");
            entity.Property(e => e.CName)
                .HasMaxLength(50)
                .HasColumnName("cName");
            entity.Property(e => e.CPhone)
                .HasMaxLength(50)
                .HasColumnName("cPhone");
            entity.Property(e => e.CStar)
                .HasMaxLength(50)
                .HasColumnName("cStar");
            entity.Property(e => e.CYears)
                .HasMaxLength(50)
                .HasColumnName("cYears");
            entity.Property(e => e.FUserId)
                .HasMaxLength(50)
                .HasColumnName("fUserId");
        });

        modelBuilder.Entity<TMember>(entity =>
        {
            entity.HasKey(e => e.FId);

            entity.ToTable("tMember");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FBirthday)
                .HasColumnType("date")
                .HasColumnName("fBirthday");
            entity.Property(e => e.FClass)
                .HasMaxLength(50)
                .HasColumnName("fClass");
            entity.Property(e => e.FEmail)
                .HasMaxLength(50)
                .HasColumnName("fEmail");
            entity.Property(e => e.FName)
                .HasMaxLength(50)
                .HasColumnName("fName");
            entity.Property(e => e.FPwd)
                .HasMaxLength(50)
                .HasColumnName("fPwd");
            entity.Property(e => e.FUserId)
                .HasMaxLength(50)
                .HasColumnName("fUserId");
            entity.Property(e => e.VerificationCode).HasMaxLength(50);
        });

        modelBuilder.Entity<TOrder>(entity =>
        {
            entity.HasKey(e => e.FId).HasName("PK__tOrder__D9F8227CB33FE399");

            entity.ToTable("tOrder");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FAddress)
                .HasMaxLength(50)
                .HasColumnName("fAddress");
            entity.Property(e => e.FDate)
                .HasColumnType("datetime")
                .HasColumnName("fDate");
            entity.Property(e => e.FEmail)
                .HasMaxLength(50)
                .HasColumnName("fEmail");
            entity.Property(e => e.FOrderGuid)
                .HasMaxLength(50)
                .HasColumnName("fOrderGuid");
            entity.Property(e => e.FPaid)
                .HasMaxLength(10)
                .HasColumnName("fPaid");
            entity.Property(e => e.FReceiver)
                .HasMaxLength(50)
                .HasColumnName("fReceiver");
            entity.Property(e => e.FTrancsationId)
                .HasMaxLength(50)
                .HasColumnName("fTrancsationId");
            entity.Property(e => e.FUserId)
                .HasMaxLength(50)
                .HasColumnName("fUserId");
        });

        modelBuilder.Entity<TOrderDetail>(entity =>
        {
            entity.HasKey(e => e.FId).HasName("PK__tOrderDe__D9F8227CF8E9D58C");

            entity.ToTable("tOrderDetail");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FIsApproved)
                .HasMaxLength(10)
                .HasColumnName("fIsApproved");
            entity.Property(e => e.FName)
                .HasMaxLength(50)
                .HasColumnName("fName");
            entity.Property(e => e.FOrderGuid)
                .HasMaxLength(50)
                .HasColumnName("fOrderGuid");
            entity.Property(e => e.FPid)
                .HasMaxLength(50)
                .HasColumnName("fPId");
            entity.Property(e => e.FPrice).HasColumnName("fPrice");
            entity.Property(e => e.FQty).HasColumnName("fQty");
            entity.Property(e => e.FUserId)
                .HasMaxLength(50)
                .HasColumnName("fUserId");
        });

        modelBuilder.Entity<TProduct>(entity =>
        {
            entity.HasKey(e => e.FId);

            entity.ToTable("tProduct");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FDescription)
                .HasMaxLength(200)
                .HasColumnName("fDescription");
            entity.Property(e => e.FImg)
                .HasMaxLength(50)
                .HasColumnName("fImg");
            entity.Property(e => e.FName)
                .HasMaxLength(50)
                .HasColumnName("fName");
            entity.Property(e => e.FPid)
                .HasMaxLength(50)
                .HasColumnName("fPId");
            entity.Property(e => e.FPrice).HasColumnName("fPrice");
        });

        modelBuilder.Entity<TShoppingCar>(entity =>
        {
            entity.HasKey(e => e.Fid);

            entity.ToTable("tShoppingCar");

            entity.Property(e => e.Fid)
                .ValueGeneratedNever()
                .HasColumnName("fid");
            entity.Property(e => e.FIsApproved)
                .HasMaxLength(10)
                .HasColumnName("fIsApproved");
            entity.Property(e => e.FName)
                .HasMaxLength(50)
                .HasColumnName("fName");
            entity.Property(e => e.FPid)
                .HasMaxLength(50)
                .HasColumnName("fPid");
            entity.Property(e => e.FPrice).HasColumnName("fPrice");
            entity.Property(e => e.FQty).HasColumnName("fQty");
            entity.Property(e => e.FUserId)
                .HasMaxLength(50)
                .HasColumnName("fUserId");
        });

        modelBuilder.Entity<Tmyf>(entity =>
        {
            entity.HasKey(e => e.FId);

            entity.ToTable("tmyf");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.CGender)
                .HasMaxLength(50)
                .HasColumnName("cGender");
            entity.Property(e => e.CId)
                .HasMaxLength(50)
                .HasColumnName("cId");
            entity.Property(e => e.CImg)
                .HasMaxLength(50)
                .HasColumnName("cImg");
            entity.Property(e => e.CInterest)
                .HasMaxLength(50)
                .HasColumnName("cInterest");
            entity.Property(e => e.CItem)
                .HasMaxLength(50)
                .HasColumnName("cItem");
            entity.Property(e => e.CName)
                .HasMaxLength(50)
                .HasColumnName("cName");
            entity.Property(e => e.CStar)
                .HasMaxLength(50)
                .HasColumnName("cStar");
            entity.Property(e => e.CUserId)
                .HasMaxLength(50)
                .HasColumnName("cUserId");
            entity.Property(e => e.CYears)
                .HasMaxLength(50)
                .HasColumnName("cYears");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
