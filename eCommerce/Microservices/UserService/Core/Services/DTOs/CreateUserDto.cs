using System.ComponentModel.DataAnnotations;

namespace UserService.Core.Services.DTOs;

public class CreateUserDto
{
    [EmailAddress] public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}