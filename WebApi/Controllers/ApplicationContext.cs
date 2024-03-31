using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Conference> Conference { get; set; }
        public DbSet<Activity> Activity { get; set; }
    

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ItConference;Username=postgres;Password=08082000");     
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activity>().HasData
            (
                new Activity { ActivityId = "Report", Description = "Доклад, 35-45 минут" },
                new Activity { ActivityId = "Masterclass", Description = "Мастеркласс, 1-2 часа" },
                new Activity { ActivityId = "Discussion", Description = "Дискуссия / круглый стол, 40-50 минут" }
            );
        }
    }
}
