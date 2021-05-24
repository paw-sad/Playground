using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Persistence;

namespace TransfersModule.Events
{
    internal class TransferCompletedEvent : INotification
    {
        public Guid TransferId { get; set; }
    }

    internal class TransferCompletedEventHandler : INotificationHandler<TransferCompletedEvent>
    {
        private readonly TransferRepository _transferRepository;

        public TransferCompletedEventHandler(TransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public async Task Handle(TransferCompletedEvent notification, CancellationToken ct)
        {
            await _transferRepository.Persist(notification, ct);
        }
    }
}