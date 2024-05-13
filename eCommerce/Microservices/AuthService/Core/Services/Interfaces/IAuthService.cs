using AuthService.Core.Services.DTOs;
using Microsoft.AspNetCore.Authentication;

namespace AuthService.Core.Services.Interfaces;

public interface IAuthService
{
    public Task Register(CreateAuthDto auth);
    public Task DeleteAuth(int authId);
    public Task<AuthenticationToken> Login(LoginDto login);
    public Task<AuthenticateResult> ValidateToken(string token);
    public Task RebuildDatabase();
}