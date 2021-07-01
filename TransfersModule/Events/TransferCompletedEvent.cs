using System;
using TransfersModule.Persistence;

namespace TransfersModule.Events
{
    internal class TransferCompletedEvent: ITransferEvent
    {
        public Guid TransferId { get; set; }
    }
}