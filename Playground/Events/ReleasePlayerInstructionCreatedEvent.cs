using System;

namespace Playground.Events
{
    internal class ReleasePlayerInstructionCreatedEvent
    {
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public decimal PaymentsAmount { get; set; }
        public DateTime TransferDate { get; set; }
    }
}