using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TransfersService.Persistence.Entities;

namespace TransfersService.Persistence
{
    internal class TransferRepository
    {
        private readonly IMongoDatabase _db;
        private IMongoCollection<Transfer> _transfers => _db.GetCollection<Transfer>("transfers");

        public TransferRepository(IMongoDatabase db)
        {
            _db = db;
        }

        public IMongoCollection<Transfer> Query() => _transfers;

        public async Task Add(Domain.Transfer domainTransfer, CancellationToken ct)
        {
            Transfer transfer = Map(domainTransfer);
            await _transfers.InsertOneAsync(transfer,null, ct);
        }

        private Transfer Map(Domain.Transfer domainTransfer)
        {
            return new Transfer
            {
                Id = domainTransfer.Id,
                EngagingClubId = domainTransfer.EngagingClubId,
                ReleasingClubId = domainTransfer.ReleasingClubId,
                PlayerId = domainTransfer.PlayerId,
                PlayersContract = new PlayersContract
                {
                    EmploymentContractStart = domainTransfer.PlayersContract.EmploymentContractStart,
                    EmploymentContractEnd = domainTransfer.PlayersContract.EmploymentContractEnd,
                    Salary = domainTransfer.PlayersContract.Salary
                },
                State = (Entities.TransferState)domainTransfer.State,
                CreatedOn = domainTransfer.CreatedOn,
                Events = domainTransfer.Events.ToList()
            };
        }
    }
}