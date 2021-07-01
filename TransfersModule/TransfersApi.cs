using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using MongoDB.Driver;
using TransfersModule.Persistence;

namespace TransfersModule
{
    public class TransfersApi
    {
        private readonly IMediator _mediator;

        public TransfersApi(string connectionStringMongo = "mongodb://mongo1:30001,mongo2:30002/replicaSet=rs0")
        {
            _mediator = BuildContainer(connectionStringMongo);
        }

        public async Task<TResponse> Execute<TResponse>(IRequest<TResponse> request)
        {
            return await _mediator.Send(request);
        }

        private static IMediator BuildContainer(string connectionStringMongo)
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
