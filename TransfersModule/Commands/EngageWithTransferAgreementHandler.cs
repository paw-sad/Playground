using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Contract;
using TransfersModule.Events;
using TransfersModule.Persistence;

namespace TransfersModule.Commands
{
    internal class EngageWithTransferAgreementHandler : IRequestHandler<EngageWithTransferAgreementContract.Request, EngageWithTransferAgreementContract.Response>
    {
        private readonly TransferInstructionRepository _transferInstructionRepository;
        private readonly TransferRepository _transferRepository;
        private readonly TransfersDbContext _db;

        public EngageWithTransferAgreementHandler(TransferInstructionRepository transferInstructionRepository, TransferRepository transferRepository, TransfersDbContext db)
        {
            _transferInstructionRepository = transferInstructionRepository;
            _transferRepository = transferRepository;
            _db = db;
        }

        public async Task<EngageWithTransferAgreementContract.Response> Handle(EngageWithTransferAgreementContract.Request request, CancellationToken ct)
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
            return new EngageWithTransferAgreementContract.Response
            {
                TransferInstructionId = transferInstructionId
            };
        }

        private EngageWithTransferAgreementInstructionCreatedEvent Map(EngageWithTransferAgreementContract.Request request)
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

        private InstructionsMatchedEvent Map(EngageWithTransferAgreementContract.Request request, Guid engagingInstructionId, Guid releasingInstructionId)
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
    }
}