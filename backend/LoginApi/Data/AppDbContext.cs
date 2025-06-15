using Microsoft.EntityFrameworkCore;
using LoginApi.Models;

namespace LoginApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<FavoritePlace> FavoritePlaces { get; set; }
    }
}