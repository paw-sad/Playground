using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class TestDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Data Source=localhost;Initial Catalog=Playground;Integrated Security=True");
    }
}