using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Contract;
using TransfersModule.Events;
using TransfersModule.Persistence;

namespace TransfersModule.Commands
{
    internal class EngageWithTransferAgreementHandler : IRequestHandler<EngageWithTransferAgreement.Request, EngageWithTransferAgreement.Response>
    {
        private readonly TransferInstructionRepository _transferInstructionRepository;
        private readonly TransferRepository _transferRepository;
        private readonly IMediator _mediator;

        public EngageWithTransferAgreementHandler(TransferInstructionRepository transferInstructionRepository, TransferRepository transferRepository, IMediator mediator)
        {
            _transferInstructionRepository = transferInstructionRepository;
            _transferRepository = transferRepository;
            _mediator = mediator;
        }

        public async Task<EngageWithTransferAgreement.Response> Handle(EngageWithTransferAgreement.Request request, CancellationToken ct)
        {
            var e = Map(request);
            await _mediator.Publish(e, ct);

            return new EngageWithTransferAgreement.Response
            {
                TransferId = e.Id
            };
        }

        private EngageWithTransferAgreementInstructionCreatedEvent Map(EngageWithTransferAgreement.Request request)
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