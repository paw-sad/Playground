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
        private readonly TransferRepository _transferRepository;

        public EngageWithoutTransferAgreement(TransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public async Task<Contract.EngageWithoutTransferAgreement.Response> Handle(Contract.EngageWithoutTransferAgreement.Request request, CancellationToken ct)
        {
            using (_transferRepository.StartTransaction())
            {
                var transferCreatedEvent = Map(request);
                _transferRepository.Persist(transferCreatedEvent);

                if (transferCreatedEvent.PlayersContract.Salary is NoSalary)
                {
                    var transferCompletedEvent = new TransferCompletedEvent
                    {
                        TransferId = transferCreatedEvent.TransferId
                    };

                    await _transferRepository.Persist(transferCompletedEvent, ct);
                }

                await _transferRepository.CommitTransaction(ct);

                return new Contract.EngageWithoutTransferAgreement.Response
                {
                    TransferId = transferCreatedEvent.TransferId
                };
            }
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