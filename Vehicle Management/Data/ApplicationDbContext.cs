using Azure.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Vehicle_Management.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Vehicle> Vehicles { get; set; }
		public DbSet<Request> Requests { get; set; }
        public DbSet<RequestStatus> RequestStatuses { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<AssignedVehicle> AssignedVehicles { get; set; }
        public DbSet<RequestMessage> RequestMessages { get; set; }
        public DbSet<RequestHistory> RequestHistories { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehicle>()
                .HasOne(rs => rs.User)
                .WithMany()
                .HasForeignKey(rs => rs.CreatedById)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestStatus>()
                .HasOne(rs => rs.User)
                .WithMany()
                .HasForeignKey(rs => rs.CreatedById)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.RequestStatus)
                .WithOne()
                .HasForeignKey<Request>(r => r.RequestStatusId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.User)
                .WithOne()
                .HasForeignKey<Request>(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Request>()
				.HasOne(r => r.User)
				.WithMany()
				.HasForeignKey(r => r.DriverUserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Request>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.CreatedbyId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Driver>()
                .HasOne(r => r.User)
                .WithOne()
                .HasForeignKey<Driver>(r => r.UserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Driver>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.CreatedById)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedVehicle>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.CreatedById)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedVehicle>()
                .HasOne(r => r.Vehicle)
                .WithMany()
                .HasForeignKey(r => r.VehicleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedVehicle>()
                .HasOne(r => r.RequestStatus)
                .WithMany()
                .HasForeignKey(r => r.RequestStatusId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestMessage>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.CreatedById)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestMessage>()
                .HasOne(r => r.Request)
                .WithOne()
                .HasForeignKey<RequestMessage>(r => r.RequestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestHistory>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.CreatedById)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestHistory>()
                .HasOne(r => r.Request)
                .WithOne()
                .HasForeignKey<RequestHistory>(r => r.RequestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestHistory>()
                .HasOne(r => r.RequestStatus)
                .WithOne()
                .HasForeignKey<RequestHistory>(r => r.RequestStatusId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.CreatedById)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(r => r.Request)
                .WithMany()
                .HasForeignKey(r => r.RequestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}