using System;

namespace TransfersModule.Events
{
    internal class InstructionsMatchedEvent
    {
        public Guid EngagingInstructionId { get; internal set; }
        public Guid ReleasingInstructionId { get; internal set; }
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public decimal PaymentsAmount { get; set; }
        public DateTime TransferDate { get; set; }
    }
}