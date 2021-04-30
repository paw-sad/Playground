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
    internal class ReleasePlayerHandler : IRequestHandler<ReleasePlayerContract.Request, ReleasePlayerContract.Response>
    {
        private readonly TransferInstructionRepository _transferInstructionRepository;
        private readonly TransfersDbContext _db;
        private readonly TransferRepository _transferRepository;

        public ReleasePlayerHandler(TransferInstructionRepository transferInstructionRepository, TransfersDbContext db, TransferRepository transferRepository)
        {
            _transferInstructionRepository = transferInstructionRepository;
            _db = db;
            _transferRepository = transferRepository;
        }

        public async Task<ReleasePlayerContract.Response> Handle(ReleasePlayerContract.Request request, CancellationToken ct)
        {
            var e = Map(request);
            var transferInstructionId = await _transferInstructionRepository.Persist(e, ct);

            var matchingTransferInstructionId = await _transferInstructionRepository.FindMatchingTransferInstructionId(e, ct);

            if (matchingTransferInstructionId == Guid.Empty)
            {
                return new ReleasePlayerContract.Response
                {
                    TransferInstructionId = transferInstructionId
                };
            }

            var instructionsMatchedEvent = Map(request,
                transferInstructionId,
                matchingTransferInstructionId);
            var transferId = await _transferRepository.Persist(instructionsMatchedEvent, ct);
            return new ReleasePlayerContract.Response
            {
                TransferId = transferId
            };
        }

        private InstructionsMatchedEvent Map(ReleasePlayerContract.Request request, Guid engagingInstructionId, Guid releasingInstructionId)
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

        private ReleasePlayerInstructionCreatedEvent Map(ReleasePlayerContract.Request request)
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