using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using UsersService.Infrastructure.DatabaseEntities;
using DomainUser = UsersService.Domain.Entities.User;
using UserCreatedEvent = UsersService.Domain.Events.UserCreatedEvent;
using UserDeletedEvent = UsersService.Domain.Events.UserDeletedEvent;

namespace UsersService.Infrastructure
{
    internal interface IUserRepository
    {
        Task AddAsync(DomainUser requestUserName, CancellationToken cancellationToken);
    }

    internal class UserRepository : IUserRepository
    {
        private readonly IMongoDatabase _db;
        private readonly IMongoClient _mongoClient;
        private IMongoCollection<UserEvent> _userEvents => _db.GetCollection<UserEvent>("UserEvents");
        private IMongoCollection<User> _usersCollection => _db.GetCollection<User>("Users");

        public UserRepository(IMongoDatabase db, IMongoClient mongoClient)
        {
            _db = db;
            _mongoClient = mongoClient;
        }

        public async Task AddAsync(DomainUser user, CancellationToken cancellationToken)
        {
            var entity = Map(user);
            var eventEntities = user.NewEvents.Select(x => Map(x, user.Id));
            using var session = await _mongoClient.StartSessionAsync(null, cancellationToken);
            session.StartTransaction();
            await _userEvents.InsertManyAsync(eventEntities, null, cancellationToken);
            await _usersCollection.InsertOneAsync(entity, null, cancellationToken);
            await session.CommitTransactionAsync(cancellationToken);
        }

        private UserEvent Map(Domain.Events.IUserEvent @event, Guid userId)
        {
            switch (@event)
            {
                case UserCreatedEvent userCreatedEvent:
                    return new DatabaseEntities.UserCreatedEvent
                    {
                        Id = Guid.NewGuid(),
                        Name = userCreatedEvent.Name,
                        UserId = userId
                    };
                case UserDeletedEvent userDeletedEvent:
                    return new DatabaseEntities.UserDeletedEvent
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(@event));
            }
        }

        private static User Map(DomainUser user)
        {
            return new()
            {
                Id = user.Id,
                Name = user.Name,
            };
        }
    }
}