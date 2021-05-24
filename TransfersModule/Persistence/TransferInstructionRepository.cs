using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TransfersModule.Commands;
using TransfersModule.Events;

namespace TransfersModule.Persistence
{
    internal class TransferInstructionRepository
    {
        private readonly IMongoCollection<TransferInstruction> _transferInstructions;

        public TransferInstructionRepository(IMongoDatabase db)
        {
            _transferInstructions = db.GetCollection<TransferInstruction>("transfer-instructions");
        }

        public IMongoCollection<TransferInstruction> Query()
        {
            return _transferInstructions;
        }

        public async Task<TransferInstruction> FindMatchingTransferInstruction(
            EngageWithTransferAgreementInstructionCreatedEvent e, CancellationToken ct)
        {
           return await Query()
                .Find(i =>
                    i.EngagingClubId == e.EngagingClubId
                    && i.ReleasingClubId == e.ReleasingClubId
                    && i.PlayerId == e.PlayerId && i.Type == TransferInstructionType.Releasing)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<Guid> FindMatchingTransferInstructionId(
            ReleasePlayerInstructionCreatedEvent e, CancellationToken ct)
        {
            return await Query()
                .Find(i =>
                    i.EngagingClubId == e.EngagingClubId
                    && i.ReleasingClubId == e.ReleasingClubId
                    && i.PlayerId == e.PlayerId && i.Type == TransferInstructionType.Engaging
                    ).Project(x => x.Id)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<TransferInstruction> PersistAndGet(
            EngageWithTransferAgreementInstructionCreatedEvent e, CancellationToken ct)
        {
            var entity = Map(e);
            await _transferInstructions.InsertOneAsync(entity, null, ct);
            return entity;
        }

        private TransferInstruction Map(EngageWithTransferAgreementInstructionCreatedEvent e)
        {
            return new()
            {
                EngagingClubId = e.EngagingClubId,
                ReleasingClubId = e.ReleasingClubId,
                PlayerId = e.PlayerId,
                PlayersContract = e.PlayersContract,
                Id = e.Id,
                Type = TransferInstructionType.Engaging,
                CreatedOn = DateTime.Now
            };
        }

        public async Task<Guid> Persist(ReleasePlayerInstructionCreatedEvent e, CancellationToken ct)
        {
            var entity = Map(e);
            await _transferInstructions.InsertOneAsync(entity, null, ct);
            return entity.Id;
        }

        private TransferInstruction Map(ReleasePlayerInstructionCreatedEvent e)
        {
            return new()
            {
                EngagingClubId = e.EngagingClubId,
                ReleasingClubId = e.ReleasingClubId,
                PlayerId = e.PlayerId,
                PlayersContract = e.PlayersContract,
                Type = TransferInstructionType.Releasing,
                CreatedOn = DateTime.Now
            };
        }
    }
}
