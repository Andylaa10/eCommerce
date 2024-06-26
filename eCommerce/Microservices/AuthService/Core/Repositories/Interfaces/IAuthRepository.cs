﻿using AuthService.Core.Entities;

namespace AuthService.Core.Repositories.Interfaces;

public interface IAuthRepository
{
    public Task<Auth> Register(Auth auth);
    public Task<Auth> GetAuthById(int id);
    public Task<Auth> GetAuthByEmail(string email);
    public Task DeleteAuth(int authId);
    public Task<bool> DoesAuthExists(string email);
    public Task RebuildDatabase();
}