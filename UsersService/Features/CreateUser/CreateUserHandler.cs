using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using UsersService.Domain.Entities;
using UsersService.Infrastructure;

namespace UsersService.Features.CreateUser
{
    internal class CreateUserHandler: IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var newUser = User.Create(Guid.NewGuid(), command.UserName);

            await _userRepository.AddAsync(newUser, cancellationToken);

            return newUser.Id;
        }
    }

    public class CreateUserCommand : IRequest<Guid>
    {
        public string UserName { get; set; }
    }
}
