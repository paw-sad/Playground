namespace UsersService.Contract
{
    public class GetUserByIdRequest
    {
        public int UserId { get; set; }
    }

    public class GetUserByIdResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ClubId { get; set; }
    }
}
