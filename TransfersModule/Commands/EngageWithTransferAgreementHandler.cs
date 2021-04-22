using System;
using System.Linq;
using TransfersModule.Contract;
using TransfersModule.Events;
using TransfersModule.Persistence;

namespace TransfersModule.Commands
{
    internal class EngageWithTransferAgreementHandler
    {
        private readonly TransferInstructionRepository _transferInstructionRepository;
        private readonly TransferRepository _transferRepository;
        private readonly AppDbContext _db;

        public EngageWithTransferAgreementHandler(TransferInstructionRepository transferInstructionRepository, TransferRepository transferRepository, AppDbContext db)
        {
            _transferInstructionRepository = transferInstructionRepository;
            _transferRepository = transferRepository;
            _db = db;
        }

        public EngageWithTransferAgreementResponse Handle(EngageWithTransferAgreementRequest request)
        {
            var e = Map(request);
            var transferInstructionId = _transferInstructionRepository.Persist(e);

            var matchingInstruction = _db.TransferInstructions
                .FirstOrDefault(i =>
                    i.EngagingClubId == e.EngagingClubId
                    && i.ReleasingClubId == e.ReleasingClubId
                    && i.PlayerId == e.PlayerId && i.Type == TransferInstructionType.Releasing
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
            return new EngageWithTransferAgreementResponse
            {
                TransferInstructionId = transferInstructionId
            };
        }

        private EngageWithTransferAgreementInstructionCreatedEvent Map(EngageWithTransferAgreementRequest request)
        {
            return new EngageWithTransferAgreementInstructionCreatedEvent
            {
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PaymentsAmount = request.PaymentsAmount,
                TransferDate = request.TransferDate
            };
        }

        private InstructionsMatchedEvent Map(EngageWithTransferAgreementRequest request, Guid engagingInstructionId, Guid releasingInstructionId)
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