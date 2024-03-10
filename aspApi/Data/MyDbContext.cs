using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using aspApi.Models;

namespace aspApi.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }
        #region DbSet
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }


        #endregion



    }
}
