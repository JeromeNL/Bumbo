using System.Diagnostics;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders;
using Bumbo.Data.Seeders.DataSeeders;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data;

public class BumboDbContext : IdentityDbContext<ApplicationUser>
{
    public BumboDbContext(DbContextOptions<BumboDbContext> options) : base(options)
    {
    }

    public DbSet<Address> Addresses { get; set; }
    public DbSet<ApplicationUserDepartment> ApplicationUserDepartments { get; set; }
    public DbSet<SpecialAvailability> SpecialAvailabilities { get; set; }
    public DbSet<StandardAvailability> StandardAvailabilities { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<ClockedHours> ClockedHours { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<OpeningHours> OpeningHours { get; set; }
    public DbSet<Payout> Payouts { get; set; }
    public DbSet<Prognosis> Prognoses { get; set; }
    public DbSet<HistoricalData> HistoricalData { get; set; }
    public DbSet<StandardSchoolHours> StandardSchoolHours { get; set; }
    public DbSet<SpecialSchoolHours> SpecialSchoolHours { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<WorkStandards> WorkStandards { get; set; }
    public DbSet<ExchangeRequest> ExchangeRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity setup
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.RegistrationDate).HasDefaultValueSql("getdate()");

        modelBuilder.Entity<ApplicationUserDepartment>()
            .HasKey(sc => new { sc.EmployeeId, sc.DepartmentId });

        modelBuilder.Entity<ApplicationUserDepartment>()
            .HasOne(sc => sc.Employee)
            .WithMany(s => s.Departments)
            .HasForeignKey(sc => sc.EmployeeId);

        modelBuilder.Entity<ApplicationUserDepartment>()
            .HasOne(sc => sc.Department)
            .WithMany(s => s.Employees)
            .HasForeignKey(sc => sc.DepartmentId);

        modelBuilder.Entity<ExchangeRequest>()
            .HasOne(er => er.Shift)
            .WithOne(s => s.ExchangeRequest)
            .HasForeignKey<ExchangeRequest>(er => er.ShiftId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<WorkStandards>()
            .Property(ws => ws.DateEntered).HasDefaultValueSql("getdate()");
        modelBuilder.Entity<StandardAvailability>()
            .HasKey(sa => new { sa.EmployeeId, sa.DayOfWeek });
        modelBuilder.Entity<StandardSchoolHours>()
            .HasKey(ssh => new { ssh.EmployeeId, ssh.DayOfWeek });

        modelBuilder.Entity<ClockedHours>()
            .ToTable("ClockedHours",
                clockedHoursTable => clockedHoursTable.IsTemporal(
                    temporal =>
                    {
                        temporal.HasPeriodStart("WasValidFrom");
                        temporal.HasPeriodEnd("WasValidTo");
                        temporal.UseHistoryTable("ChangedClockedHours");
                    }));

        // Indexes
        modelBuilder.Entity<Shift>()
            .HasIndex(s => new { s.Start, s.End })
            .IncludeProperties(
                s => new { s.EmployeeId, s.DepartmentId, s.IsPublished, s.IsIll });

        if (IsDebug())
        {
            if (Debugger.IsAttached == false)
                modelBuilder.InitializeSeedData();
        }
        else
        {
            modelBuilder.SeedRoles();
        }
    }

    private static bool IsDebug()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}