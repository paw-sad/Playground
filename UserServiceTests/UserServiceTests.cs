using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mongo2Go;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Shouldly;
using UsersService.Domain.Events;
using UsersService.Features.CreateUser;
using UsersService.Infrastructure;
using DomainUser = UsersService.Domain.Entities.User;
using UserEntity = UsersService.Infrastructure.DatabaseEntities.User;

namespace UsersService.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public async Task TestUsingMediatr()
        {
            var mediator = UsersService.Setup.GetClient();
            var userId = await mediator.Send(new CreateUserCommand
            {
                UserName = "Piotr Sadowski"
            });
            
            userId.ShouldNotBe(default);
        }

        [TestMethod]
        public void UnitTestOnTheDomainUser()
        {
            var userId = Guid.NewGuid();
            var userName = "Pawel Sadowski";

            var user = DomainUser.Create(userId, userName);

            user.Id.ShouldBe(userId);
            user.Name.ShouldBe(userName);
            user.NewEvents.Count.ShouldBe(1);
            var userCreatedEvent = user.NewEvents.First() as UserCreatedEvent;

            userCreatedEvent.Name.ShouldBe(userName);
        }

        [TestMethod]
        public async Task UserRepositoryTestUsingInMemmoryMongoDb()
        {
            using var mongo = MongoDbRunner.Start(singleNodeReplSet: true);

            var client = new MongoClient(mongo.ConnectionString);
            var db = client.GetDatabase("test-user-repo");
            var repo = new UserRepository(db, client);

            var userId = Guid.NewGuid();
            var userName = Guid.NewGuid().ToString();

            var domainUser = DomainUser.Create(userId, userName);

            await repo.AddAsync(domainUser, CancellationToken.None);

            var dbUser = await db.GetCollection<UserEntity>("Users").Find(x => x.Id == domainUser.Id).FirstOrDefaultAsync();

            dbUser.ShouldNotBeNull();
            dbUser.Id.ShouldBe(domainUser.Id);
            dbUser.Name.ShouldBe(domainUser.Name);

            var events = await db.GetCollection<UsersService.Infrastructure.DatabaseEntities.UserEvent>("UserEvents")
                .Find(x => x.UserId == domainUser.Id).ToListAsync();
            events.Count.ShouldBe(1);

            var @event = events.First() as UsersService.Infrastructure.DatabaseEntities.UserCreatedEvent;
            @event.Id.ShouldNotBe(default);
            @event.UserId.ShouldBe(domainUser.Id);
            @event.Name.ShouldBe(userName);
        }

        [TestMethod]
        public void T()
        {
            BsonClassMap.RegisterClassMap<UserCreatedEvent>();
            BsonClassMap.RegisterClassMap<UserDeletedEvent>();
            string connectionStringMongo = "mongodb://mongo1:30001,mongo2:30002/replicaSet=rs0";
            var mongoClient = new MongoClient(connectionStringMongo);
            var db = mongoClient.GetDatabase("users-module");
            var users = db.GetCollection<UserEntity>("Users");
            var newUserId = Guid.NewGuid();
            var newUserName = Guid.NewGuid().ToString();
        }
    }
}
