using System.ComponentModel.DataAnnotations;

namespace AuthService.Core.Services.DTOs;

public class CreateAuthDto
{
    [EmailAddress] public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
}