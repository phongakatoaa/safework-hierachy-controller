using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using sw_control_hierachy.Models;

namespace sw_control_hierachy.Data;

public partial class SafeworkDbContext : DbContext
{
    public SafeworkDbContext(DbContextOptions<SafeworkDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cacontrol> Cacontrols { get; set; }

    public virtual DbSet<Calog> Calogs { get; set; }

    public virtual DbSet<Carisk> Carisks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cacontrol>(entity =>
        {
            entity.ToTable("CAControl", "dbo", tb => tb.HasTrigger("TRI_DICTIONARY_CA_CONTROL"));

            entity.Property(e => e.CacontrolId).HasColumnName("CAControlID");
            entity.Property(e => e.Control).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Enabled).HasColumnName("enabled");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
        });

        modelBuilder.Entity<Calog>(entity =>
        {
            entity.ToTable("CALog", "dbo", tb => tb.HasTrigger("TRI_CALOG"));

            entity.HasIndex(e => new { e.LocationId, e.ClosedDate }, "idx_CALog_LocationIDClosedDate");

            entity.HasIndex(e => new { e.LocationId, e.OpenDate }, "idx_CALog_LocationIDOpenDate");

            entity.Property(e => e.CalogId).HasColumnName("CALogID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.AssignedUserId).HasColumnName("AssignedUserID");
            entity.Property(e => e.BbslogId).HasColumnName("BBSLogID");
            entity.Property(e => e.BehaviorTypeId).HasColumnName("BehaviorTypeID");
            entity.Property(e => e.CacontrolId).HasColumnName("CAControlID");
            entity.Property(e => e.Cadate)
                .HasColumnType("datetime")
                .HasColumnName("CADate");
            entity.Property(e => e.CariskId).HasColumnName("CARiskID");
            entity.Property(e => e.CastatusId).HasColumnName("CAStatusID");
            entity.Property(e => e.ClosedByUserId).HasColumnName("ClosedByUserID");
            entity.Property(e => e.ClosedDate).HasColumnType("datetime");
            entity.Property(e => e.CommitDate).HasColumnType("datetime");
            entity.Property(e => e.CompleteDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.DatlogId).HasColumnName("DATLogID");
            entity.Property(e => e.DistrictId).HasColumnName("DistrictID");
            entity.Property(e => e.GlobalId).HasColumnName("GlobalID");
            entity.Property(e => e.HhlogId).HasColumnName("HHLogID");
            entity.Property(e => e.IncidentLogId).HasColumnName("IncidentLogID");
            entity.Property(e => e.InspectionLogId).HasColumnName("InspectionLogID");
            entity.Property(e => e.IsArchivedNcr).HasColumnName("IsArchivedNCR");
            entity.Property(e => e.IsNcrca).HasColumnName("IsNCRCA");
            entity.Property(e => e.ItquestionCalogId).HasColumnName("ITQuestionCAlogID");
            entity.Property(e => e.JseariskRegistryLogId).HasColumnName("JSEARiskRegistryLogID");
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.LastUpdateByUserId).HasColumnName("LastUpdateByUserID");
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.MoclogId).HasColumnName("MOCLogID");
            entity.Property(e => e.OpenDate).HasColumnType("datetime");
            entity.Property(e => e.QratetingMsg)
                .HasMaxLength(500)
                .HasColumnName("QRatetingMsg");
            entity.Property(e => e.QratingPoint).HasColumnName("QRatingPoint");
            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.SubmitByUserId).HasColumnName("SubmitByUserID");
            entity.Property(e => e.SupervisorId).HasColumnName("SupervisorID");
            entity.Property(e => e.SwalogId).HasColumnName("SWALogID");
            entity.Property(e => e.UserDeviceId).HasColumnName("UserDeviceID");
            entity.Property(e => e.WorksiteId).HasColumnName("WorksiteID");
        });

        modelBuilder.Entity<Carisk>(entity =>
        {
            entity.ToTable("CARisk", "dbo", tb =>
                {
                    tb.HasTrigger("TRI_CARISK");
                    tb.HasTrigger("TRI_DICTIONARY_CA_RISK");
                });

            entity.Property(e => e.CariskId).HasColumnName("CARiskID");
            entity.Property(e => e.ColorCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Risk).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
