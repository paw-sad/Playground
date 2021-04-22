using System;

namespace TransfersModule.Events
{
    internal class TransferCompletedEvent
    {
        public Guid TransferId { get; set; }
    }
}