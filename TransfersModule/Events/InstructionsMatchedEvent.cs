using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Persistence;

namespace TransfersModule.Events
{
    internal class InstructionsMatchedEvent : INotification
    {
        public Guid EngagingInstructionId { get; set; }

        public Guid ReleasingInstructionId { get; set; }

        public int EngagingClubId { get; set; }

        public int ReleasingClubId { get; set; }

        public int PlayerId { get; set; }

        public PlayersContract PlayersContract { get; set; }

        public bool PerfectMatch { get; set; }

        public Guid Id { get; set; }
    }

    internal class InstructionsMatchedEventHandler : INotificationHandler<InstructionsMatchedEvent>
    {
        private readonly TransferRepository _transferRepository;

        public InstructionsMatchedEventHandler(TransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public async Task Handle(InstructionsMatchedEvent e, CancellationToken ct)
        {
            await _transferRepository
                .Persist(e, ct);
        }
    }
}