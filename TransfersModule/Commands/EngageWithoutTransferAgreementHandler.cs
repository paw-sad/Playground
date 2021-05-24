using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Events;
using TransfersModule.Persistence;

namespace TransfersModule.Commands
{
    internal class EngageWithoutTransferAgreement : IRequestHandler<Contract.EngageWithoutTransferAgreement.Request, Contract.EngageWithoutTransferAgreement.Response>
    {
        private readonly IMediator _mediator;

        public EngageWithoutTransferAgreement(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Contract.EngageWithoutTransferAgreement.Response> Handle(Contract.EngageWithoutTransferAgreement.Request request, CancellationToken ct)
        {
            var transferCreatedEvent = Map(request);
            await _mediator.Publish(transferCreatedEvent, ct);

            return new Contract.EngageWithoutTransferAgreement.Response
            {
                TransferId = transferCreatedEvent.TransferId
            };
        }

        private static TransferCreatedEvent Map(Contract.EngageWithoutTransferAgreement.Request request) =>
            new()
            {
                TransferId = Guid.NewGuid(),
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PlayersContract = PlayerContractMapper.Map(request.PlayersContract),
                Type = TransferType.WithoutTransferAgreement,
            };
    }
}