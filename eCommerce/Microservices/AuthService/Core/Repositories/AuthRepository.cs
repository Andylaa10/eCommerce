using System.Data;
using AuthService.Core.Entities;
using AuthService.Core.Helpers;
using AuthService.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Core.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly DatabaseContext _context;

    public AuthRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task Register(Auth auth)
    {
        var exist = await DoesAuthExists(auth.Email);

        if (exist)
            throw new DuplicateNameException($"{auth.Email} is already in use");

        await _context.Auths.AddAsync(auth);
        await _context.SaveChangesAsync();
    }

    public async Task<Auth> GetAuthById(int id)
    {
        var auth = await _context.Auths.FirstOrDefaultAsync(a => a.Id == id);
        return auth ?? throw new KeyNotFoundException($"No user with id of {id}");
    }

    public async Task<Auth> GetAuthByEmail(string email)
    {
        var auth = await _context.Auths.FirstOrDefaultAsync(a => a.Email == email);
        return auth ?? throw new KeyNotFoundException($"No user with email of {email}");
    }

    public async Task DeleteAuth(int authId)
    {
        var auth = await GetAuthById(authId);
        _context.Auths.Remove(auth);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DoesAuthExists(string email)
    {
        var auth = await _context.Auths.FirstOrDefaultAsync(a => a.Email == email);

        if (auth is null)
            return await Task.Run(() => false);
        
        return await Task.Run(() => true);
    }
    
    public async Task RebuildDatabase()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
    }
}