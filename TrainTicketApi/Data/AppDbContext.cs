using Microsoft.EntityFrameworkCore;
using TrainTicketApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TrainTicketApi.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TrainRoute> Routes { get; set; }
    public DbSet<TrainTrip> TrainTrips { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TrainRoute>(entity =>
        {
            entity.ToTable("Routes", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_Routes_DistanceKm", "[DistanceKm] > 0");
            });
            entity.HasKey(e => e.RouteId);
            entity.Property(e => e.DepartureStation).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ArrivalStation).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DistanceKm).HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(30);
        });

        modelBuilder.Entity<TrainTrip>(entity =>
        {
            entity.ToTable("TrainTrips", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_TrainTrips_TotalSeats", "[TotalSeats] > 0");
                tableBuilder.HasCheckConstraint("CK_TrainTrips_AvailableSeats", "[AvailableSeats] >= 0 AND [AvailableSeats] <= [TotalSeats]");
                tableBuilder.HasCheckConstraint("CK_TrainTrips_BaseTicketPrice", "[BaseTicketPrice] > 0");
                tableBuilder.HasCheckConstraint("CK_TrainTrips_ArrivalTime", "[ArrivalTime] > [DepartureTime]");
            });
            entity.HasKey(e => e.TrainTripId);
            entity.Property(e => e.TrainCode).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.TrainCode);
            entity.Property(e => e.TotalSeats).IsRequired();
            entity.Property(e => e.AvailableSeats).IsRequired();
            entity.Property(e => e.BaseTicketPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(30);
            entity.HasOne(e => e.Route)
                .WithMany(r => r.TrainTrips)
                .HasForeignKey(e => e.RouteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(15);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IdentityNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.HasIndex(e => e.PhoneNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.IdentityNumber).IsUnique();
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Tickets", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_Tickets_Price", "[Price] > 0");
            });
            entity.HasKey(e => e.TicketId);
            entity.Property(e => e.TicketCode).IsRequired().HasMaxLength(30);
            entity.HasIndex(e => e.TicketCode).IsUnique();
            entity.Property(e => e.SeatNumber).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.PaymentStatus).IsRequired().HasMaxLength(30);
            entity.Property(e => e.TicketStatus).IsRequired().HasMaxLength(30);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.CancelReason).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.HasIndex(e => new { e.TrainTripId, e.SeatNumber }).IsUnique();


            entity.HasOne(e => e.TrainTrip)
                .WithMany(t => t.Tickets)
                .HasForeignKey(e => e.TrainTripId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Tickets)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(e => e.AuditLogId);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PerformedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.HasIndex(e => e.PerformedAt);
        });
    }
}
