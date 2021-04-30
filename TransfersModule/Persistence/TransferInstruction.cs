using System;

namespace TransfersModule.Persistence
{
    internal class TransferInstruction
    {
        public Guid Id { get; set; }
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public decimal PaymentsAmount { get; set; }
        public DateTime TransferDate { get; set; }
        public TransferInstructionType Type { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}