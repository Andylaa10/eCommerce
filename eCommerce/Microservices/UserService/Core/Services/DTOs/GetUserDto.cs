namespace UserService.Core.Services.DTOs;

public class GetUserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}