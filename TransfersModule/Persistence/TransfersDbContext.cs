using Microsoft.EntityFrameworkCore;

namespace TransfersModule.Persistence
{
    internal class TransfersDbContext : DbContext
    {
        private readonly string _connectionString;

        public TransfersDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(_connectionString);

        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferInstruction> TransferInstructions { get; set; }
    }
}