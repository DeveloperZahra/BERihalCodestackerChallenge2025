namespace BERihalCodestackerChallenge2025.DTOs
{
    // Used in login requests (Basic Auth may decode from header instead)
    //  DTO for login process
    public class LoginDto
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }

    //  DTO for the registration process (Register)
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User"; 
    }

    public class AuthUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string ClearanceLevel { get; set; }
        public string Token { get; set; }  
    }
}


