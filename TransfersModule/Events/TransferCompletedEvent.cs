using System;

namespace TransfersService.Events
{
    internal class TransferCompletedEvent: ITransferEvent
    {
        public Guid TransferId { get; set; }
    }
}