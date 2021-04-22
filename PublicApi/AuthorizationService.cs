using UsersService.Contract;

namespace PublicApi
{
    public class AuthorizationService
    {
        public static bool CanExecuteCommand(GetUserByIdResponse user, TransfersModule.Contract.EngageWithoutTransferAgreementRequest command)
        {
            return user.ClubId == command.EngagingClubId;
        }

        public static bool CanExecuteCommand(GetUserByIdResponse user, TransfersModule.Contract.ReleasePlayerRequest command)
        {
            return user.ClubId == command.ReleasingClubId;
        }
    }
}
