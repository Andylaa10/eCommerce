namespace UserService.Core.Services.DTOs;

public class UpdateUserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}