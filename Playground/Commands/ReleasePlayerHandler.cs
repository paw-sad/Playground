using System;
using System.Linq;
using TransfersModule.Contract;
using TransfersModule.Events;
using TransfersModule.Persistence;

namespace TransfersModule.Commands
{
    internal class ReleasePlayerHandler
    {
        private readonly TransferInstructionRepository _transferInstructionRepository;
        private readonly AppDbContext _db;
        private readonly TransferRepository _transferRepository;

        public ReleasePlayerHandler(TransferInstructionRepository transferInstructionRepository, AppDbContext db, TransferRepository transferRepository)
        {
            _transferInstructionRepository = transferInstructionRepository;
            _db = db;
            _transferRepository = transferRepository;
        }

        public ReleasePlayerResponse Handle(ReleasePlayerRequest request)
        {
            var e = Map(request);
            var transferInstructionId = _transferInstructionRepository.Persist(e);

            var matchingInstruction = _db.TransferInstructions
                .OrderBy(x => x.CreatedOn)
                .FirstOrDefault(i =>
                    i.EngagingClubId == e.EngagingClubId 
                    && i.ReleasingClubId == e.ReleasingClubId
                    && i.PlayerId == e.PlayerId && i.Type == TransferInstructionType.Engaging
                    && i.TransferId == null);

            if (matchingInstruction != null)
            {
                var instructionsMatchedEvent = Map(request, transferInstructionId, matchingInstruction.Id);
                _transferRepository.Persist(instructionsMatchedEvent);
                
                var transferCreatedEvent = new TransferCreatedEvent
                {
                    EngagingClubId = request.EngagingClubId,
                    ReleasingClubId = request.ReleasingClubId,
                    PaymentsAmount = request.PaymentsAmount,
                    PlayerId = request.PlayerId,
                    TransferDate = request.TransferDate
                };

                _transferRepository.Persist(transferCreatedEvent);
            }

            return new ReleasePlayerResponse
            {
                TransferInstructionId = transferInstructionId
            };
        }

        private InstructionsMatchedEvent Map(ReleasePlayerRequest request, Guid engagingInstructionId, Guid releasingInstructionId)
        {
            return new InstructionsMatchedEvent
            {
                EngagingInstructionId = engagingInstructionId,
                ReleasingInstructionId = releasingInstructionId,
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PaymentsAmount = request.PaymentsAmount,
                PlayerId = request.PlayerId,
                TransferDate = request.TransferDate
            };
        }

        private ReleasePlayerInstructionCreatedEvent Map(ReleasePlayerRequest request)
        {
            return new ReleasePlayerInstructionCreatedEvent
            {
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PaymentsAmount = request.PaymentsAmount,
                TransferDate = request.TransferDate
            };
        }
    }
}