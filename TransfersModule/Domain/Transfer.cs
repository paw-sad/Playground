using System;
using System.Collections.Generic;
using System.Text;
using TransfersService.Commands;
using TransfersService.Events;
using EngageWithoutTransferAgreement = TransfersService.Contract.EngageWithoutTransferAgreement;

namespace TransfersService.Domain
{
    internal class Transfer
    {
        public Guid Id { get; private set; }
        public int EngagingClubId { get; private set; }
        public int ReleasingClubId { get; private set; }
        public int PlayerId { get; private set; }
        public PlayersContract PlayersContract { get; private set; }
        public TransferState State { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public IList<object> Events { get; } = new List<object>();

        internal static Transfer CreateNewEngageWithoutTransferAgreement(EngageWithoutTransferAgreement.Request request)
        {
            var transferCreatedEvent = CreateEvent(request);

            var transfer = new Transfer();
            transfer.Apply(transferCreatedEvent);

            return transfer;
        }

        private void Apply(TransferCreatedEvent @event)
        {
            EngagingClubId = @event.EngagingClubId;
            ReleasingClubId = @event.ReleasingClubId;
            PlayerId = @event.PlayerId;
            PlayersContract = new PlayersContract
            {
                EmploymentContractStart = @event.PlayersContract.EmploymentContractStart,
                EmploymentContractEnd = @event.PlayersContract.EmploymentContractEnd,
                Salary = @event.PlayersContract.Salary
            };
            CreatedOn = DateTime.Now;
            Id = @event.TransferId;
            Events.Add(@event);
        }

        private static TransferCreatedEvent CreateEvent(EngageWithoutTransferAgreement.Request request) =>
            new TransferCreatedEvent
            {
                TransferId = Guid.NewGuid(),
                EngagingClubId = request.EngagingClubId,
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                PlayersContract = PlayerContractMapper.Map(request.PlayersContract),
            };
    }
}
