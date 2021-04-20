using System;
using System.Collections.Generic;

namespace Playground.Persistence
{
    internal class Transfer
    {
        public Guid Id { get; set; }
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public decimal PaymentsAmount { get; set; }
        public DateTime TransferDate { get; set; }
        public TransferState State { get; set; }

        public virtual ICollection<TransferInstruction> TransferInstructions { get; set; }
    }
}