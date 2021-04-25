using System;
using PublicApi.Contract;
using TransfersModule;
using UsersService.Contract;
using LogInRequest = PublicApi.Contract.LogInRequest;

namespace PublicApi
{

    public class Api
    {
        private int? _currentUserId;
        private readonly UsersService.Api _usersService;

        public Api(UsersService.Api usersService)
        {
            _usersService = usersService;
        }

        public EngageWithoutTransferAgreementResponse EngagePlayerWithoutTransferAgreement(EngageWithoutTransferAgreementRequest request)
        {
            if (!_currentUserId.HasValue)
            {
                throw new InvalidOperationException("No access");
            }

            var currentUser = _usersService.GetUserById(new GetUserByIdRequest
            {
                UserId = _currentUserId.Value
            });

            var command = new TransfersModule.Contract.EngageWithoutTransferAgreementContract.Request
            {
                ReleasingClubId = request.ReleasingClubId,
                PlayerId = request.PlayerId,
                EngagingClubId = request.EngagingClubId,
            };

            if (!AuthorizationService.CanExecuteCommand(currentUser, command))
            {
                throw new InvalidOperationException("You can only engage players for your own club");
            }

            var response = new TransfersApi().Execute(command).Result;

            return new EngageWithoutTransferAgreementResponse
            {
                TransferId = response.TransferId
            };
        }

        public ReleasePlayerResponse ReleasePlayer(ReleasePlayerRequest request)
        {
            if (!_currentUserId.HasValue)
            {
                throw new InvalidOperationException("No access");
            }

            var user = _usersService.GetUserById(new GetUserByIdRequest
            {
                UserId = _currentUserId.Value
            });

            var command = new TransfersModule.Contract.ReleasePlayerContract.Request()
            {
                ReleasingClubId = request.ReleasingClubId,
                EngagingClubId = request.EngagingClubId,
                PlayerId = request.PlayerId,
                TransferDate = request.TransferDate,
                PaymentsAmount = request.PaymentsAmount
            };

            if (!AuthorizationService.CanExecuteCommand(user, command))
            {
                throw new InvalidOperationException("You can only release players only from your own club");
            }

            var response = new TransfersApi().Execute(command).Result;

            return new ReleasePlayerResponse
            {
                TransferInstructionId = response.TransferInstructionId
            };
        }


        public void LogIn(LogInRequest request)
        {
            var response = _usersService.Authenticate(new UsersService.Contract.LogInRequest
            {
                UserName = request.UserName,
                Password = request.Password
            });
            _currentUserId = response.UserId;
        }
    }
}
