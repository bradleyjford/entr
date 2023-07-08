using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SampleApi.Data;

namespace SampleApi.Security;

public class UserContext : IUserContext<User, UserId>
{
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly SampleApiDbContext _dbContext;

    public UserContext(
        IHttpContextAccessor httpContextAccessor,
        SampleApiDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public Task<UserId?> GetCurrentId()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return Task.FromResult<UserId?>(null);
        }

        var principal = _httpContextAccessor.HttpContext.User;

        var subjectClaimValue = principal.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(subjectClaimValue))
        {
            return Task.FromResult<UserId?>(null);
        }

        return Task.FromResult<UserId?>(new UserId(Guid.Parse(subjectClaimValue)));
    }

    public async Task<User?> GetCurrent()
    {
        var userId = await GetCurrentId();

        return await _dbContext.Users.FindAsync(userId);
    }
}
