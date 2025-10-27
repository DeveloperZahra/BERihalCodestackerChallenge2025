namespace BERihalCodestackerChallenge2025.DTOs
{
    // DTO for creating or updating a user
    public class UserCreateUpdateDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // Admin | Investigator | Officer
        public string ClearanceLevel { get; set; } // low | medium | high | critical
        public string Password { get; set; } // plain password input only
    }

}
