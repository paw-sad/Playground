using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using TransfersModule.Infrastructure;
using TransfersModule.Persistence;

namespace TransfersModule
{
    public class TransfersApi
    {
        private readonly IMediator _mediator;

        public TransfersApi(string connectionString = "Data Source=localhost;Initial Catalog=Playground;Integrated Security=True")
        {
            _mediator = BuildContainer(connectionString);
        }

        public async Task<TResponse> Execute<TResponse>(IRequest<TResponse> request)
        {
            return await _mediator.Send(request);
        }

        private static IMediator BuildContainer(string connectionString)
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

            builder.Register(context =>
            {
                return new TransfersDbContext(connectionString);
            }).As<TransfersDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<TransferRepository>().As<TransferRepository>();
            builder.RegisterType<TransferInstructionRepository>().As<TransferInstructionRepository>();

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            builder.RegisterGenericDecorator(typeof(UnitOfWorkCommandHandlerDecorator<,>), typeof(IRequestHandler<,>));
            var container = builder.Build();

            var mediator = container.Resolve<IMediator>();

            return mediator;
        }
    }
}
