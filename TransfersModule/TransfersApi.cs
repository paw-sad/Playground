﻿using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using MongoDB.Driver;
using TransfersModule.Infrastructure;
using TransfersModule.Persistence;

namespace TransfersModule
{
    public class TransfersApi
    {
        private readonly IMediator _mediator;

        public TransfersApi(string connectionString = "Data Source=localhost;Initial Catalog=Playground;Integrated Security=True", string connectionStringMongo = "mongodb://localhost:27017")
        {
            _mediator = BuildContainer(connectionString, connectionStringMongo);
        }

        public async Task<TResponse> Execute<TResponse>(IRequest<TResponse> request)
        {
            return await _mediator.Send(request);
        }

        private static IMediator BuildContainer(string connectionString, string connectionStringMongo)
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
                    .RegisterAssemblyTypes(typeof(TransfersApi).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
            }

            builder.Register(context => new TransfersDbContext(connectionString))
                .As<TransfersDbContext>()
                .InstancePerLifetimeScope();
            var mongoClient = new MongoClient(connectionStringMongo);
            var transfersDb = mongoClient.GetDatabase("transfers-module");

            builder
                .Register(ctx => mongoClient)
                .As<IMongoClient>()
                .SingleInstance();
            builder
                .Register(ctx => transfersDb)
                .As<IMongoDatabase>()
                .SingleInstance();

            builder
                .RegisterType<TransferRepository>()
                .As<TransferRepository>();

            builder
                .RegisterType<TransferInstructionRepository>()
                .As<TransferInstructionRepository>();

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            var container = builder.Build();

            var mediator = container.Resolve<IMediator>();

            return mediator;
        }
    }
}