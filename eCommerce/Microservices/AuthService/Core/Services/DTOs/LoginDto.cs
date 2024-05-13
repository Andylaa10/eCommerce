using System.ComponentModel.DataAnnotations;

namespace AuthService.Core.Services.DTOs;

public class LoginDto
{
    [EmailAddress] public string Email { get; set; }
    public string Password { get; set; }
}