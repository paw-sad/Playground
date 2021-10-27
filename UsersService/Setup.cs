using System.Reflection;
using Autofac;
using MediatR;
using MongoDB.Driver;
using UsersService.Infrastructure;

namespace UsersService
{
    public static class Setup
    {
        public static IMediator GetClient(string connectionStringMongo = "mongodb://mongo1:30001,mongo2:30002/replicaSet=rs0")
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
            };

            foreach (var mediatrOpenType in mediatrOpenTypes)
            {
                builder
                    .RegisterAssemblyTypes(typeof(Setup).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
            }

            var mongoClient = new MongoClient(connectionStringMongo);
            var transfersDb = mongoClient.GetDatabase("users-module");

            builder
                .Register(ctx => mongoClient)
                .As<IMongoClient>()
                .SingleInstance();
            builder
                .Register(ctx => transfersDb)
                .As<IMongoDatabase>()
                .SingleInstance();

            builder
                .RegisterType<UserRepository>()
                .As<IUserRepository>();

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            var container = builder.Build();

            return container.Resolve<IMediator>();
        }
    }
}

