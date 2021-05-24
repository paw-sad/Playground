using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TransfersModule.Events;

namespace TransfersModule.Persistence
{
    internal class TransferRepository
    {
        private readonly IMongoDatabase _db;
        private IMongoCollection<Transfer> _transfers => _db.GetCollection<Transfer>("transfers");
        private IMongoCollection<TransferInstruction> _transferInstructions => _db.GetCollection<TransferInstruction>("transfer-instructions");

        public TransferRepository(IMongoDatabase db)
        {
            _db = db;
        }

        public IMongoCollection<Transfer> Query() => _transfers;

        public void Persist(TransferCreatedEvent transferCreatedEvent)
        {
            var transfer = Map(transferCreatedEvent);
            _transfers.InsertOne(transfer);
        }

        private Transfer Map(TransferCreatedEvent e)
            => new()
            {
                Id = e.TransferId,
                ReleasingClubId = e.ReleasingClubId,
                EngagingClubId = e.EngagingClubId,
                PlayerId = e.PlayerId,
                State = TransferState.Confirmed,
                CreatedOn = new DateTime(),
                PlayersContract = e.PlayersContract,
                Type = e.Type
            };

        public async Task Persist(TransferCompletedEvent e, CancellationToken ct)
            => await _transfers.UpdateOneAsync(x => x.Id == e.TransferId, Builders<Transfer>
                .Update.Set(x => x.State, TransferState.Completed), cancellationToken: ct);

        public async Task<Guid> Persist(InstructionsMatchedEvent instructionsMatchedEvent, CancellationToken ct)
        {
            var transfer = Map(instructionsMatchedEvent);
            using var session = await _db.Client.StartSessionAsync(null, ct);

            await _transferInstructions.DeleteManyAsync(x =>
                x.Id == instructionsMatchedEvent.ReleasingInstructionId
                || x.Id == instructionsMatchedEvent.EngagingInstructionId, ct);
            await _transfers.InsertOneAsync(transfer, null, ct);

            return transfer.Id;
        }

        private Transfer Map(InstructionsMatchedEvent e)
            => new()
            {
                EngagingClubId = e.EngagingClubId,
                ReleasingClubId = e.ReleasingClubId,
                PlayerId = e.PlayerId,
                PlayersContract =  e.PlayersContract,
                CreatedOn = DateTime.Now,
                Type = TransferType.WithTransferAgreement,
                State = e.PerfectMatch? TransferState.Confirmed : TransferState.MatchingException,
                Id = e.Id
            };
    }
}