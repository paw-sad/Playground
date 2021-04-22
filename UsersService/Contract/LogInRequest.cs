using System;

namespace UsersService.Contract
{
    public class LogInRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }    
    
    public class LogInResponse
    {
        public int UserId { get; set; }
    }
}
