using intro2NET.API.Models;
using Microsoft.EntityFrameworkCore;

namespace intro2NET.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options){}

        public DbSet<Value> Values { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
