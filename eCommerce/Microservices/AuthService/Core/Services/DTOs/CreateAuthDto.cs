using System.ComponentModel.DataAnnotations;

namespace AuthService.Core.Services.DTOs;

public class CreateAuthDto
{
    [EmailAddress] public string Email { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}