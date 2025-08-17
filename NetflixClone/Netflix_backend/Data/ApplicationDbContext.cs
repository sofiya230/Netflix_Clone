using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;

namespace NetflixClone.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<MyList> MyLists { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<TwoFactorAuth> TwoFactorAuths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

                    modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
            });

                    modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Profiles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

                    modelBuilder.Entity<Content>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ContentType).IsRequired().HasMaxLength(50);
            });

                    modelBuilder.Entity<Episode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Content)
                    .WithMany(c => c.Episodes)
                    .HasForeignKey(e => e.ContentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

                    modelBuilder.Entity<MyList>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.MyLists)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.MyLists)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Content)
                    .WithMany(c => c.MyList)
                    .HasForeignKey(e => e.ContentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

                    modelBuilder.Entity<WatchHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.WatchHistories)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.WatchHistories)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Content)
                    .WithMany(c => c.WatchHistory)
                    .HasForeignKey(e => e.ContentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Episode)
                    .WithMany()
                    .HasForeignKey(e => e.EpisodeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

                    modelBuilder.Entity<PasswordReset>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ResetCode).IsRequired().HasMaxLength(10);
            });

                    modelBuilder.Entity<TwoFactorAuth>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.VerificationCode).IsRequired().HasMaxLength(10);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.TwoFactorAuths)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
