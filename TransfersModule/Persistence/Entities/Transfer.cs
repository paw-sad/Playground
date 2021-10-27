using System;
using System.Collections.Generic;

namespace TransfersService.Persistence.Entities
{
    internal class Transfer
    {
        public Guid Id { get; set; }
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public PlayersContract PlayersContract { get; set; }
        public TransferState State { get; set; }
        public DateTime CreatedOn { get; internal set; }
        public IList<object> Events { get; set; }
    }
}