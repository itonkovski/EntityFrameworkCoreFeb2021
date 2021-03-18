using System;
using Microsoft.EntityFrameworkCore;

namespace EFCoreCodeFirstDemo.Models
{
    public class SliDoDbContext : DbContext
    {
        public SliDoDbContext()
        {

        }

        public SliDoDbContext(DbContextOptions dbContextOptions)
            :base(dbContextOptions)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;User Id = SA;Password = Qawsed12;Database=SliDo");
            }
        }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Question> Questions { get; set; }
    }
}
