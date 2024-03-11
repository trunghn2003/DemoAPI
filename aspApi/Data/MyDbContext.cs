using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using aspApi.Models;

namespace aspApi.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamUser>()
                .HasKey(tu => new { tu.TeamId, tu.UserId });

            modelBuilder.Entity<TeamUser>()
                .HasOne(tu => tu.Team)
                .WithMany(t => t.TeamUsers)
                .HasForeignKey(tu => tu.TeamId);

            modelBuilder.Entity<TeamUser>()
                .HasOne(tu => tu.User)
                .WithMany(u => u.TeamUsers)
                .HasForeignKey(tu => tu.UserId);
        }

        #region DbSet
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<TeamUser> TeamUsers { get; set; }  

        #endregion



    }
}
