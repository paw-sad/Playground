using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Persistence;

namespace TransfersModule.Events
{
    internal class EngageWithTransferAgreementInstructionCreatedEvent : INotification
    {
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public PlayersContract PlayersContract { get; set; }
        public Guid Id { get; set; }
    }

    internal class EngageWithTransferAgreementInstructionCreatedEventHandler : INotificationHandler<
            EngageWithTransferAgreementInstructionCreatedEvent>
    {
        private readonly TransferInstructionRepository _transferInstructionRepository;
        private readonly IMediator _mediator;

        public EngageWithTransferAgreementInstructionCreatedEventHandler(TransferInstructionRepository transferInstructionRepository, IMediator mediator)
        {
            _transferInstructionRepository = transferInstructionRepository;
            _mediator = mediator;
        }

        public async Task Handle(EngageWithTransferAgreementInstructionCreatedEvent e, CancellationToken ct)
        {
            var transferInstruction = await _transferInstructionRepository.PersistAndGet(e, ct);
            var matchingTransferInstruction = await _transferInstructionRepository.FindMatchingTransferInstruction(e, ct);

            if (matchingTransferInstruction is null)
            {
                return;
            }

            var matchingResult = CompareTransferInstructions(transferInstruction, matchingTransferInstruction);

            var instructionsMatchedEvent =
                Map(transferInstruction,
                    transferInstruction.Id,
                    matchingTransferInstruction.Id,
                    matchingResult);

            await _mediator.Publish(instructionsMatchedEvent, ct);
        }

        private bool CompareTransferInstructions(TransferInstruction a, TransferInstruction b) => Equals(a, b);

        private InstructionsMatchedEvent Map(TransferInstruction request, Guid engagingInstructionId,
            Guid releasingInstructionId, bool matchingResult)
        {
            return new()
            {
                EngagingInstructionId = engagingInstructionId,
                ReleasingInstructionId = releasingInstructionId,
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PlayersContract = request.PlayersContract,
                PerfectMatch = matchingResult
            };
        }
    }
}