using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using TransfersModule.Persistence;

namespace TransfersModule.Infrastructure
{
    internal class UnitOfWorkCommandHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _decorated;
        private readonly TransfersDbContext _db;
        private readonly ILifetimeScope _container;

        public UnitOfWorkCommandHandlerDecorator(
            IRequestHandler<TRequest, TResponse> decorated, TransfersDbContext db, ILifetimeScope container)
        {
            _decorated = decorated;
            _db = db;
            _container = container;
        }

        public async Task<TResponse> Handle(TRequest command, CancellationToken cancellationToken)
        {
            await using (_container.BeginLifetimeScope())
            {
                var response = await _decorated.Handle(command, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);
                return response;
            }
        }
    }
}