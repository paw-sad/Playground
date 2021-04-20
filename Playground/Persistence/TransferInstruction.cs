using System;

namespace Playground.Persistence
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

        public Guid? TransferId { get; set; }
        public virtual Transfer Transfer { get; set; }
    }
}