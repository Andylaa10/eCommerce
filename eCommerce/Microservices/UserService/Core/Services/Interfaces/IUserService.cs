using UserService.Core.Services.DTOs;

namespace UserService.Core.Services.Interfaces;

public interface IUserService
{
    public Task<GetUserDto> GetUserById(int id);
    public Task<IEnumerable<GetUserDto>> GetAllUsers();
    public Task<GetUserDto> AddUser(CreateUserDto user);
    public Task<GetUserDto> UpdateUser(int id, UpdateUserDto user);
    public Task<GetUserDto> DeleteUser(int id);
    public Task RebuildDatabase();
}