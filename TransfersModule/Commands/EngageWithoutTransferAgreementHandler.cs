using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Contract;
using TransfersModule.Events;
using TransfersModule.Persistence;

namespace TransfersModule.Commands
{
    internal class EngageWithoutTransferAgreementHandler : IRequestHandler<EngageWithoutTransferAgreementContract.Request, EngageWithoutTransferAgreementContract.Response>
    {
        private readonly TransferRepository _transferRepository;

        public EngageWithoutTransferAgreementHandler(TransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public async Task<EngageWithoutTransferAgreementContract.Response> Handle(EngageWithoutTransferAgreementContract.Request request, CancellationToken ct)
        {
            var transferCreatedEvent = Map(request);
            var transferId = _transferRepository.Persist(transferCreatedEvent);

            if (transferCreatedEvent.PaymentsAmount == 0)
            {
                var transferCompletedEvent = new TransferCompletedEvent
                {
                    TransferId = transferId
                };

                await _transferRepository.Persist(transferCompletedEvent, ct);
            }

            return new EngageWithoutTransferAgreementContract.Response
            {
                TransferId = transferId
            };
        }

        private static TransferCreatedEvent Map(EngageWithoutTransferAgreementContract.Request request)
        {
            return new TransferCreatedEvent
            {
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
            };
        }
    }
}