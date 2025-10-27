namespace BERihalCodestackerChallenge2025.DTOs
{
    // Used in login requests (Basic Auth may decode from header instead)
    public class LoginRequestDto
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
