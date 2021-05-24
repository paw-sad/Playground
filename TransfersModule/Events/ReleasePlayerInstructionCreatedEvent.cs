using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Persistence;

namespace TransfersModule.Events
{
    internal class ReleasePlayerInstructionCreatedEvent : INotification
    {
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public PlayersContract PlayersContract { get; set; }
        public Guid Id { get; set; }
    }

    internal class ReleasePlayerInstructionCreatedEventHandler : INotificationHandler<ReleasePlayerInstructionCreatedEvent>
    {
        private readonly TransferInstructionRepository _transferInstructionRepository;
        private readonly TransferRepository _transferRepository;
        private readonly IMediator _mediator;

        public ReleasePlayerInstructionCreatedEventHandler(TransferInstructionRepository transferInstructionRepository, TransferRepository transferRepository, IMediator mediator)
        {
            _transferInstructionRepository = transferInstructionRepository;
            _transferRepository = transferRepository;
            _mediator = mediator;
        }

        public async Task Handle(ReleasePlayerInstructionCreatedEvent e, CancellationToken ct)
        {
            var transferInstructionId = await _transferInstructionRepository.Persist(e, ct);

            var matchingTransferInstructionId = await _transferInstructionRepository.FindMatchingTransferInstructionId(e, ct);

            if (matchingTransferInstructionId == Guid.Empty)
            {
                return;
            }

            var instructionsMatchedEvent = Map(e,
                transferInstructionId,
                matchingTransferInstructionId);

            await _mediator.Publish(instructionsMatchedEvent, ct);
        }

        private InstructionsMatchedEvent Map(ReleasePlayerInstructionCreatedEvent request, Guid engagingInstructionId, Guid releasingInstructionId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                EngagingInstructionId = engagingInstructionId,
                ReleasingInstructionId = releasingInstructionId,
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PlayersContract = request.PlayersContract
            };
        }

    }
}