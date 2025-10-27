namespace BERihalCodestackerChallenge2025.DTOs
{
    // Used in login requests (Basic Auth may decode from header instead)
    public class LoginRequestDto
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
    // Response after authentication
    public class AuthUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string ClearanceLevel { get; set; }
        public string Token { get; set; } // optional if you later add JWT
    }

}
