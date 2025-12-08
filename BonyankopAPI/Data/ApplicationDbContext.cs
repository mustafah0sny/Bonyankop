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
