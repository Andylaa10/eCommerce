using System.Data;
using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;
using UserService.Core.Helpers;
using UserService.Core.Repositories.Interfaces;

namespace UserService.Core.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _context;

    public UserRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserById(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user ?? throw new KeyNotFoundException($"No user with id of {id}");
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user ?? throw new KeyNotFoundException($"No user with email of {email}");
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> AddUser(User user)
    {
        var exist = await DoesUserExist(user.Email);
        if (exist)
            throw new DuplicateNameException($"{user.Email} is already in use");

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<User> UpdateUser(int id, User user)
    {
        if (id != user.Id)
            throw new ArgumentException("Id int the route does not match the id of the user");
        var userUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (userUpdate is null)
            throw new ArgumentException("Could not find the user");

        userUpdate.Email = user.Email;
        userUpdate.UpdatedAt = DateTime.UtcNow;
        userUpdate.Password = user.Password;

        _context.Users.Update(userUpdate);
        await _context.SaveChangesAsync();

        return userUpdate;
    }

    public async Task<User> DeleteUser(int id)
    {
        var user = await GetUserById(id);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DoesUserExist(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return await Task.Run(() => false);
        return await Task.Run(() => true);
    }
    
    public async Task RebuildDatabase()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
    }
}