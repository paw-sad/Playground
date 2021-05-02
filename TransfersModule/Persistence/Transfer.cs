using System;
using System.Collections.Generic;

namespace TransfersModule.Persistence
{
    internal class Transfer
    {
        public Guid Id { get; set; }
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public PlayersContract PlayersContract { get; set; }
        public TransferState State { get; set; }

        public virtual ICollection<TransferInstruction> TransferInstructions { get; set; }
        public TransferType Type { get; internal set; }
        public DateTime CreatedOn { get; internal set; }
    }
}