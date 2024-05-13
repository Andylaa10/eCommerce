using UserService.Core.Entities;

namespace UserService.Core.Repositories.Interfaces;

public interface IUserRepository
{
    public Task<User> GetUserById(int id);
    public Task<User> GetUserByEmail(string email);
    public Task<IEnumerable<User>> GetAllUsers();
    public Task<User> AddUser(User user);
    public Task<User> UpdateUser(int id, User user);
    public Task<User> DeleteUser(int id);
    public Task<bool> DoesUserExist(string email);
    public Task RebuildDatabase();
}