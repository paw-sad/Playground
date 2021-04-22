using System;
using System.Collections.Generic;
using TransfersModule.Commands;
using TransfersModule.Events;

namespace TransfersModule.Persistence
{
    internal class TransferRepository
    {
        private readonly AppDbContext _db;

        public TransferRepository(AppDbContext db)
        {
            _db = db;
        }

        public Guid Persist(TransferCreatedEvent transferCreatedEvent)
        {
            var transfer = Map(transferCreatedEvent);
            _db.Transfers.Add(transfer);

            return transfer.Id;
        }

        private Transfer Map(TransferCreatedEvent e)
        {
            return new Transfer
            {
                Id = Guid.NewGuid(),
                ReleasingClubId = e.ReleasingClubId,
                EngagingClubId = e.EngagingClubId,
                PaymentsAmount = e.PaymentsAmount,
                PlayerId = e.PlayerId,
                State = TransferState.Confirmed,
                TransferDate = e.TransferDate
            };
        }

        public void Persist(TransferCompletedEvent e)
        {
            var transfer = _db.Transfers.Find(e.TransferId);
            transfer.State = TransferState.Completed;
        }

        public Guid Persist(InstructionsMatchedEvent instructionsMatchedEvent)
        {
            var transfer = Map(instructionsMatchedEvent);
            var engagingInstruction = _db.TransferInstructions.Find(instructionsMatchedEvent.EngagingInstructionId);
            var releasingInstruction = _db.TransferInstructions.Find(instructionsMatchedEvent.ReleasingInstructionId);
            transfer.TransferInstructions.Add(engagingInstruction);
            transfer.TransferInstructions.Add(releasingInstruction);
            _db.Transfers.Add(transfer);

            return transfer.Id;
        }

        private Transfer Map(InstructionsMatchedEvent instructionsMatchedEvent)
        {
            var transfer = new Transfer
            {
                EngagingClubId = instructionsMatchedEvent.EngagingClubId,
                ReleasingClubId = instructionsMatchedEvent.ReleasingClubId,
                PlayerId = instructionsMatchedEvent.PlayerId,
                PaymentsAmount = instructionsMatchedEvent.PaymentsAmount,
                TransferDate = instructionsMatchedEvent.TransferDate,
                TransferInstructions = new List<TransferInstruction>()
            };

            return transfer;
        }
    }
}