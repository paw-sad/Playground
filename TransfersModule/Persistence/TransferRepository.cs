using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TransfersModule.Events;

namespace TransfersModule.Persistence
{
    internal class TransferRepository
    {
        private readonly IMongoDatabase _db;
        private readonly IMongoClient _client;
        private IClientSessionHandle _openedSession;
        private IMongoCollection<ITransferEvent> _transferEvents => _db.GetCollection<ITransferEvent>("transfer-events");
        private IMongoCollection<Transfer> _transfers => _db.GetCollection<Transfer>("transfers");

        public TransferRepository(IMongoDatabase db, IMongoClient client)
        {
            _db = db;
            _client = client;
        }

        public IMongoCollection<Transfer> Query() => _transfers;

        public IClientSessionHandle StartTransaction()
        {
            _openedSession = _client.StartSession();
            _openedSession.StartTransaction();
            return _client.StartSession();
        }     
        
        public async Task CommitTransaction(CancellationToken ct)
        {
            await _openedSession.CommitTransactionAsync(ct);
        }

        public Guid Persist(TransferCreatedEvent transferCreatedEvent)
        {
            var transfer = Map(transferCreatedEvent);
            _transfers.InsertOne(transfer);
            _transferEvents.InsertOne(transferCreatedEvent);
            return transfer.Id;
        }

        private Transfer Map(TransferCreatedEvent e)
            => new Transfer()
            {
                Id = e.TransferId,
                ReleasingClubId = e.ReleasingClubId,
                EngagingClubId = e.EngagingClubId,
                PlayerId = e.PlayerId,
                State = TransferState.Confirmed,
                CreatedOn = new DateTime(),
                PlayersContract = e.PlayersContract,
                Type = e.Type
            };

        public async Task Persist(TransferCompletedEvent e, CancellationToken ct)
        {
            await _transferEvents.InsertOneAsync(e, cancellationToken: ct);
            await _transfers.UpdateOneAsync(x => x.Id == e.TransferId, Builders<Transfer>
                .Update.Set(x => x.State, TransferState.Completed), cancellationToken: ct);
        }
    }
}