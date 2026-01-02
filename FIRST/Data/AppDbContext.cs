using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FIRST.Models;
using FIRST.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace FIRST.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<User> Users => Set<User>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Subject> Subjects { get; set; }

        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<Message> Messages => Set<Message>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Email)
                        .IsUnique();

             modelBuilder.Entity<RefreshToken>()
                        .HasIndex(r => r.TokenHash)
                        .IsUnique();

            modelBuilder.Entity<Subject>()
                        .HasIndex(s => s.Code)
                        .IsUnique();


            modelBuilder.Entity<Conversation>(e =>
            {
                e.HasIndex(x => new { x.User1Id, x.User2Id }).IsUnique();

                e.HasMany(x => x.Messages)
                .WithOne(m => m.Conversation!)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Message>(e =>
            {
                e.HasIndex(x => new { x.ConversationId, x.SentAt });
                e.HasIndex(x => new { x.RecipientId, x.ReadAt });

                e.Property(x => x.Content).HasMaxLength(4000);
            });
        }

    }
}
