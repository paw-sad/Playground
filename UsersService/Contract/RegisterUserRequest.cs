namespace UsersService.Contract
{
    public class RegisterUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int ClubId { get; set; }
    }   
    
    public class RegisterUserResponse
    {
        public int UserId { get; set; }
    }
}