namespace AuthService.Core.Services.DTOs;

public class GetAuthDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}