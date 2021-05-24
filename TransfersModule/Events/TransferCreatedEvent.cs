using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Persistence;

namespace TransfersModule.Events
{
    internal class TransferCreatedEvent : INotification
    {
        public Guid TransferId { get; set; }
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public PlayersContract PlayersContract { get; set; }
        public TransferType Type { get; set; }
    }

    internal class TransferCreatedEventHandler : INotificationHandler<TransferCreatedEvent>
    {
        private readonly TransferRepository _transferRepository;
        private readonly IMediator _mediator;

        public TransferCreatedEventHandler(TransferRepository transferRepository, IMediator mediator)
        {
            _transferRepository = transferRepository;
            _mediator = mediator;
        }

        public async Task Handle(TransferCreatedEvent transferCreatedEvent, CancellationToken ct)
        {
            _transferRepository.Persist(transferCreatedEvent);

            if (transferCreatedEvent.PlayersContract.Salary is NoSalary)
            {
                var transferCompletedEvent = new TransferCompletedEvent
                {
                    TransferId = transferCreatedEvent.TransferId
                };

                await _mediator.Publish(transferCompletedEvent, ct);
            }
        }
    }
}