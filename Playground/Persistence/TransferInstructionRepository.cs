using Playground.Events;
using System;

namespace Playground.Persistence
{
    internal class TransferInstructionRepository
    {
        private readonly AppDbContext _db;

        public TransferInstructionRepository(AppDbContext db)
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
