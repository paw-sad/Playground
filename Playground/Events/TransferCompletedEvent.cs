using System;

namespace Playground.Events
{
    internal class TransferCompletedEvent
    {
        public Guid TransferId { get; set; }
    }
}