using Microsoft.EntityFrameworkCore;

namespace TransfersModule.Persistence
{
    internal class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Data Source=localhost;Initial Catalog=Playground;Integrated Security=True");

        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferInstruction> TransferInstructions { get; set; }
    }
}