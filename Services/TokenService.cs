using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace TodoApi.Services;

public class TokenService
{
    private readonly IOpenIddictTokenManager _tokenManager;

    public TokenService(IOpenIddictTokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    public async Task<bool> RevokeTokenAsync(string tokenId)
    {
        var token = await _tokenManager.FindByIdAsync(tokenId);
        if (token is null)
            return false;

        await _tokenManager.DeleteAsync(token);
        return true;
    }

    public async Task RevokeAllUserTokensAsync(string userId)
    {
        await _tokenManager.RevokeAsync(userId, null, null, null);

        var tokens = await _tokenManager.FindAsync(userId, null, null, null).ToListAsync();
        foreach (var token in tokens)
        {
            Console.WriteLine($"Revoking token: {(token as OpenIddictEntityFrameworkCoreToken)?.ConcurrencyToken}");
            // await _tokenManager.DeleteAsync(token);
        }
    }
}