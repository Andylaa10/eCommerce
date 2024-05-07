using System.ComponentModel.DataAnnotations;

namespace AuthService.Core.Entities;

public class Auth
{
    public int Id { get; set; }
    [EmailAddress] public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
}