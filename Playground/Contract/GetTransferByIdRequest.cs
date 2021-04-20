﻿using System;
using System.Collections.Generic;
using System.Text;
using Playground.Persistence;

namespace Playground.Contract
{
    public class GetTransferByIdRequest
    {
        public Guid Id { get; set; }
    }

    public class GetTransferByIdResponse
    {
        public Guid Id { get; set; }
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public decimal PaymentsAmount { get; set; }
        public DateTime TransferDate { get; set; }
        public TransferState State { get; set; }
    }
}
