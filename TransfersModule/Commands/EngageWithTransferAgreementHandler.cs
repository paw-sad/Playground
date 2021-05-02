using System;
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

        public EngageWithTransferAgreementHandler(TransferInstructionRepository transferInstructionRepository, TransferRepository transferRepository)
        {
            _transferInstructionRepository = transferInstructionRepository;
            _transferRepository = transferRepository;
        }

        public async Task<EngageWithTransferAgreementContract.Response> Handle(EngageWithTransferAgreementContract.Request request, CancellationToken ct)
        {
            var e = Map(request);
            var transferInstructionId = await _transferInstructionRepository.Persist(e, ct);

            var matchingTransferInstructionId = await _transferInstructionRepository.FindMatchingTransferInstructionId(e, ct);

            if (matchingTransferInstructionId == Guid.Empty)
            {
                return new EngageWithTransferAgreementContract.Response
                {
                    TransferInstructionId = transferInstructionId
                };
            }

            var instructionsMatchedEvent = Map(
                request,
                transferInstructionId,
                matchingTransferInstructionId);

            var transferId = await _transferRepository
                                    .Persist(instructionsMatchedEvent, ct);

            return new EngageWithTransferAgreementContract.Response
            {
                TransferId = transferId
            };
        }

        private EngageWithTransferAgreementInstructionCreatedEvent Map(EngageWithTransferAgreementContract.Request request)
        {
            return new EngageWithTransferAgreementInstructionCreatedEvent
            {
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PlayersContract = PlayerContractMapper.Map(request.PlayersContract)
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
                PlayerId = request.PlayerId,
                PlayersContract = PlayerContractMapper.Map(request.PlayersContract)
            };
        }
    }
}