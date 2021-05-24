using System;
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
        private readonly TransferRepository _transferRepository;
        private readonly IMediator _mediator;

        public ReleasePlayerHandler(TransferInstructionRepository transferInstructionRepository, TransferRepository transferRepository, IMediator mediator)
        {
            _transferInstructionRepository = transferInstructionRepository;
            _transferRepository = transferRepository;
            _mediator = mediator;
        }

        public async Task<ReleasePlayerContract.Response> Handle(ReleasePlayerContract.Request request, CancellationToken ct)
        {
            var e = Map(request);

            await _mediator.Publish(e, ct);

            return new ReleasePlayerContract.Response
            {
                TransferId = e.Id
            };
        }

        private InstructionsMatchedEvent Map(ReleasePlayerContract.Request request, Guid engagingInstructionId, Guid releasingInstructionId)
        {
            return new()
            {
                EngagingInstructionId = engagingInstructionId,
                ReleasingInstructionId = releasingInstructionId,
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PlayersContract = PlayerContractMapper.Map(request.PlayersContract)
            };
        }

        private ReleasePlayerInstructionCreatedEvent Map(ReleasePlayerContract.Request request)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PlayersContract = PlayerContractMapper.Map(request.PlayersContract)
            };
        }
    }
}