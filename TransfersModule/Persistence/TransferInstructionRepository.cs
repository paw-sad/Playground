using System;
using TransfersModule.Events;

namespace TransfersModule.Persistence
{
    internal class TransferInstructionRepository
    {
        private readonly TransfersDbContext _db;

        public TransferInstructionRepository(TransfersDbContext db)
        {
            _db = db;
        }

        public Guid Persist(EngageWithTransferAgreementInstructionCreatedEvent e)
        {
            var entity = Map(e);
            _db.TransferInstructions.Add(entity);
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

        public Guid Persist(ReleasePlayerInstructionCreatedEvent e)
        {
            var entity = Map(e);
            _db.TransferInstructions.Add(entity);
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
