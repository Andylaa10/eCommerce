using UserService.Core.Services.DTOs;

namespace UserService.Core.Services.Interfaces;

public interface IUserService
{
    public Task<GetUserDto> GetUserById(int id);
    public Task<IEnumerable<GetUserDto>> GetAllUsers();
    public Task AddUser(CreateUserDto user);
    public Task UpdateUser(int id, UpdateUserDto user);
    public Task DeleteUser(int id);
}