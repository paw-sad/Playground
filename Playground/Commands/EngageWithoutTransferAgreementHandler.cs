using Playground.Contract;
using Playground.Events;
using Playground.Persistence;

namespace Playground.Commands
{
    internal class EngageWithoutTransferAgreementHandler
    {
        private readonly TransferRepository _transferRepository;

        public EngageWithoutTransferAgreementHandler(TransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public EngageWithoutTransferAgreementResponse Handle(EngageWithoutTransferAgreementRequest engageWithoutTransferAgreementRequest)
        {
            var transferCreatedEvent = Map(engageWithoutTransferAgreementRequest);
            var transferId = _transferRepository.Persist(transferCreatedEvent);

            if (transferCreatedEvent.PaymentsAmount == 0)
            {
                var transferCompletedEvent = new TransferCompletedEvent
                {
                    TransferId = transferId
                };

                _transferRepository.Persist(transferCompletedEvent);
            }

            return new EngageWithoutTransferAgreementResponse
            {
                TransferId = transferId
            };
        }

        private static TransferCreatedEvent Map(EngageWithoutTransferAgreementRequest engageWithoutTransferAgreementRequest)
        {
            return new TransferCreatedEvent
            {
                EngagingClubId = engageWithoutTransferAgreementRequest.EngagingClubId,
                ReleasingClubId = engageWithoutTransferAgreementRequest.ReleasingClubId,
                PlayerId = engageWithoutTransferAgreementRequest.PlayerId,
                TransferDate = engageWithoutTransferAgreementRequest.TransferDate,
                PaymentsAmount = engageWithoutTransferAgreementRequest.PaymentsAmount
            };
        }
    }
}