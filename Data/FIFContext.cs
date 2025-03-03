using FixItFinderDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace FixItFinderDemo.Data
{
    public class FIFContext : DbContext
    {
        public FIFContext(DbContextOptions<FIFContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Service_History> Service_Histories { get; set; }
        public DbSet<Customer_Profile> Customer_Profiles { get; set; }
        public DbSet<Worker_Profile> Worker_Profiles { get; set; }
        public DbSet<Post_Engagement> Post_Engagements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer_Profile>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer_Profile)
                .HasForeignKey<Customer_Profile>(c => c.UserId);

            modelBuilder.Entity<Worker_Profile>()
                .HasOne(w => w.User)
                .WithOne(u => u.Worker_Profile)
                .HasForeignKey<Worker_Profile>(w => w.UserId);

            modelBuilder.Entity<Service_History>()
                .HasOne(s => s.Worker)
                .WithMany(w => w.Service_Histories)
                .HasForeignKey(s => s.WorkerId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service_History>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Service_Histories)
                .HasForeignKey(s => s.CustomerId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Post_Engagement>()
                .HasOne(pe => pe.Post)
                .WithMany(p => p.PostEngagements)
                .HasForeignKey(pe => pe.PostId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Post_Engagement>()
                .HasOne(pe => pe.EngagedUser)
                .WithMany(u => u.PostEngagements)
                .HasForeignKey(pe => pe.EngagedUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
