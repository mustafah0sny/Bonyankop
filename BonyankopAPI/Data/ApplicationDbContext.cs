using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BonyankopAPI.Models;

namespace BonyankopAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProviderProfile> ProviderProfiles { get; set; }
        public DbSet<PortfolioItem> PortfolioItems { get; set; }
        public DbSet<Diagnostic> Diagnostics { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Role).HasConversion<string>();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ProfilePictureUrl).HasMaxLength(500);
            });

            // Configure ProviderProfile entity
            modelBuilder.Entity<ProviderProfile>(entity =>
            {
                entity.HasKey(e => e.ProviderId);
                entity.Property(e => e.ProviderType).HasConversion<string>();
                entity.Property(e => e.ServicesOffered).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                entity.Property(e => e.Certifications).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                entity.Property(e => e.CoverageAreas).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                
                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<ProviderProfile>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure PortfolioItem entity
            modelBuilder.Entity<PortfolioItem>(entity =>
            {
                entity.HasKey(e => e.PortfolioId);
                entity.Property(e => e.Images).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                
                entity.HasOne(e => e.ProviderProfile)
                    .WithMany()
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Diagnostic entity
            modelBuilder.Entity<Diagnostic>(entity =>
            {
                entity.HasKey(e => e.DiagnosticId);
                entity.Property(e => e.RiskLevel).HasConversion<string>();
                entity.Property(e => e.ProblemCategory).HasConversion<string>();
                entity.Property(e => e.UrgencyLevel).HasConversion<string>();
                
                entity.HasOne(e => e.Citizen)
                    .WithMany()
                    .HasForeignKey(e => e.CitizenId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ServiceRequest entity
            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId);
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.AdditionalImages).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                
                entity.HasOne(e => e.Citizen)
                    .WithMany()
                    .HasForeignKey(e => e.CitizenId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Diagnostic)
                    .WithMany()
                    .HasForeignKey(e => e.DiagnosticId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            // Configure Quote entity
            modelBuilder.Entity<Quote>(entity =>
            {
                entity.HasKey(e => e.QuoteId);
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.Attachments).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                
                entity.HasOne(e => e.ServiceRequest)
                    .WithMany()
                    .HasForeignKey(e => e.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Provider)
                    .WithMany()
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Project entity
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.ProjectId);
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.PaymentStatus).HasConversion<string>();
                entity.Property(e => e.BeforeImages).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                entity.Property(e => e.DuringImages).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                entity.Property(e => e.AfterImages).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                
                entity.HasOne(e => e.ServiceRequest)
                    .WithMany()
                    .HasForeignKey(e => e.RequestId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Quote)
                    .WithMany()
                    .HasForeignKey(e => e.QuoteId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Citizen)
                    .WithMany()
                    .HasForeignKey(e => e.CitizenId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Provider)
                    .WithMany()
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Rating entity
            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => e.RatingId);
                
                entity.HasOne(e => e.Project)
                    .WithMany()
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Citizen)
                    .WithMany()
                    .HasForeignKey(e => e.CitizenId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Provider)
                    .WithMany()
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure AuditLog entity
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.LogId);
                entity.Property(e => e.ActionType).HasMaxLength(100);
                entity.Property(e => e.EntityType).HasMaxLength(100);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.SessionId).HasMaxLength(100);
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
                
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.EntityType, e.EntityId });
                entity.HasIndex(e => e.CreatedAt);
            });

            // Configure SystemSettings entity
            modelBuilder.Entity<SystemSettings>(entity =>
            {
                entity.HasKey(e => e.SettingId);
                entity.Property(e => e.DataType).HasConversion<string>();
                entity.HasIndex(e => e.SettingKey).IsUnique();
                entity.HasIndex(e => e.Category);
            });

            // Rename Identity tables to simpler names (optional)
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        }
    }
}
