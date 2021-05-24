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

        public EngageWithTransferAgreementHandler(TransferInstructionRepository transferInstructionRepository, TransferRepository transferRepository)
        {
            _transferInstructionRepository = transferInstructionRepository;
            _transferRepository = transferRepository;
        }

        public async Task<EngageWithTransferAgreement.Response> Handle(EngageWithTransferAgreement.Request request, CancellationToken ct)
        {
            var e = Map(request);
            var transferInstruction = await _transferInstructionRepository.PersistAndGet(e, ct);
            var matchingTransferInstruction = await _transferInstructionRepository.FindMatchingTransferInstruction(e, ct);

            if (matchingTransferInstruction is null)
            {
                return new EngageWithTransferAgreement.Response
                {
                    TransferInstructionId = transferInstruction.Id
                };
            }

            var matchingResult = CompareTransferInstructions(transferInstruction, matchingTransferInstruction);
            var instructionsMatchedEvent =
                Map(request, transferInstruction.Id, matchingTransferInstruction.Id, matchingResult);

            var transferId = await _transferRepository
                                    .Persist(instructionsMatchedEvent, ct);

            return new EngageWithTransferAgreement.Response
            {
                TransferId = transferId
            };
        }


        private bool CompareTransferInstructions(TransferInstruction a, TransferInstruction b)
        {
            return Equals(a, b);
        }

        private EngageWithTransferAgreementInstructionCreatedEvent Map(EngageWithTransferAgreement.Request request)
        {
            return new()
            {
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PlayersContract = PlayerContractMapper.Map(request.PlayersContract)
            };
        }

        private InstructionsMatchedEvent Map(EngageWithTransferAgreement.Request request, Guid engagingInstructionId,
            Guid releasingInstructionId, bool matchingResult)
        {
            return new()
            {
                EngagingInstructionId = engagingInstructionId,
                ReleasingInstructionId = releasingInstructionId,
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PlayersContract = PlayerContractMapper.Map(request.PlayersContract),
                PerfectMatch = matchingResult
            };
        }
    }
}