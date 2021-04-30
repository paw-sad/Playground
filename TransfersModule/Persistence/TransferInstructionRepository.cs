using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
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

        public async Task<Guid> FindMatchingTransferInstructionId(
            EngageWithTransferAgreementInstructionCreatedEvent e, CancellationToken ct)
        {
           return await Query()
                .Find(i =>
                    i.EngagingClubId == e.EngagingClubId
                    && i.ReleasingClubId == e.ReleasingClubId
                    && i.PlayerId == e.PlayerId && i.Type == TransferInstructionType.Releasing)
                .Project(x => x.Id)
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

        public async Task<Guid> Persist(
            EngageWithTransferAgreementInstructionCreatedEvent e, CancellationToken ct)
        {
            var entity = Map(e);
            await _transferInstructions.InsertOneAsync(entity, null, ct);
            return entity.Id;
        }

        private TransferInstruction Map(EngageWithTransferAgreementInstructionCreatedEvent e)
        {
            return new TransferInstruction
            {
                EngagingClubId = e.EngagingClubId,
                ReleasingClubId = e.ReleasingClubId,
                PlayerId = e.PlayerId,
                PaymentsAmount = e.PaymentsAmount,
                TransferDate = e.TransferDate,
                Id = Guid.NewGuid(),
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
            return new TransferInstruction
            {
                EngagingClubId = e.EngagingClubId,
                ReleasingClubId = e.ReleasingClubId,
                PlayerId = e.PlayerId,
                PaymentsAmount = e.PaymentsAmount,
                TransferDate = e.TransferDate,
                Type = TransferInstructionType.Releasing,
                CreatedOn = DateTime.Now
            };
        }
    }
}
