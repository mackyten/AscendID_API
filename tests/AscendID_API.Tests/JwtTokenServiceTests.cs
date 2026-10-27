using AscendID_API.Application.Interfaces.Identity;
using AscendID_API.Domain.Entities.Identity;
using AscendID_API.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AscendID_API.Tests;

public class JwtTokenServiceTests
{
    private const string Secret = "unit-test-secret-key-that-is-at-least-32-chars";

    private static readonly Guid UserId = Guid.NewGuid();

    private static Mock<UserManager<ApplicationUser>> CreateUserManager() =>
        new(Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);

    private static JwtTokenService CreateService(UserManager<ApplicationUser> userManager, string secret = Secret)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = secret,
                ["Jwt:Issuer"] = "tests",
                ["Jwt:Audience"] = "tests-clients",
            })
            .Build();

        return new JwtTokenService(userManager, config, NullLogger<JwtTokenService>.Instance);
    }

    private static JwtTokenService CreateService() => CreateService(CreateUserManager().Object);

    private static Task<TokenResult> IssueTokensAsync(JwtTokenService service) =>
        service.GenerateTokensAsync(UserId, "gelo", "gelo@example.com", new[] { "Admin" }, new[] { "openid" });

    [Fact]
    public async Task GenerateTokens_ReturnsBearerTokens_ThatExpireInTheFuture()
    {
        var tokens = await IssueTokensAsync(CreateService());

        Assert.Equal("Bearer", tokens.TokenType);
        Assert.False(string.IsNullOrEmpty(tokens.AccessToken));
        Assert.False(string.IsNullOrEmpty(tokens.RefreshToken));
        Assert.True(tokens.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task ValidateAccessToken_ReturnsUserRolesAndScopes()
    {
        var service = CreateService();
        var tokens = await IssueTokensAsync(service);

        var result = await service.ValidateAccessTokenAsync(tokens.AccessToken);

        Assert.True(result.IsValid);
        Assert.Equal(UserId, result.UserId);
        Assert.Contains("Admin", result.Roles);
        Assert.Contains("openid", result.Scopes);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-jwt")]
    public async Task ValidateAccessToken_RejectsEmptyAndMalformedTokens(string token)
    {
        var result = await CreateService().ValidateAccessTokenAsync(token);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidateAccessToken_RejectsToken_SignedWithAnotherKey()
    {
        var userManager = CreateUserManager().Object;
        var tokens = await IssueTokensAsync(CreateService(userManager, "a-completely-different-secret-key-0123456789"));

        var result = await CreateService(userManager).ValidateAccessTokenAsync(tokens.AccessToken);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task RevokeToken_MakesAccessTokenInvalid()
    {
        var service = CreateService();
        var tokens = await IssueTokensAsync(service);

        Assert.True(await service.RevokeTokenAsync(tokens.AccessToken));

        var result = await service.ValidateAccessTokenAsync(tokens.AccessToken);
        Assert.False(result.IsValid);
        Assert.Equal("Token has been revoked", result.ErrorMessage);
    }

    [Fact]
    public async Task RevokeToken_ReturnsFalse_ForUnknownToken()
    {
        Assert.False(await CreateService().RevokeTokenAsync("unknown"));
    }

    [Fact]
    public async Task RefreshTokens_IssuesNewPair_AndRevokesTheOldOne()
    {
        var userManager = CreateUserManager();
        var user = new ApplicationUser { Id = UserId, UserName = "gelo", Email = "gelo@example.com" };
        userManager.Setup(m => m.FindByIdAsync(UserId.ToString())).ReturnsAsync(user);
        userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });
        var service = CreateService(userManager.Object);
        var original = await IssueTokensAsync(service);

        var refreshed = await service.RefreshTokensAsync(original.RefreshToken);

        Assert.NotNull(refreshed);
        Assert.NotEqual(original.AccessToken, refreshed!.AccessToken);
        Assert.False((await service.ValidateAccessTokenAsync(original.AccessToken)).IsValid);
        Assert.True((await service.ValidateAccessTokenAsync(refreshed.AccessToken)).IsValid);
    }

    [Fact]
    public async Task RefreshTokens_ReturnsNull_ForUnknownRefreshToken()
    {
        Assert.Null(await CreateService().RefreshTokensAsync("unknown"));
    }

    [Fact]
    public async Task RefreshTokens_ReturnsNull_ForInactiveUser()
    {
        var userManager = CreateUserManager();
        var inactive = new ApplicationUser { Id = UserId, UserName = "gelo", IsActive = false };
        userManager.Setup(m => m.FindByIdAsync(UserId.ToString())).ReturnsAsync(inactive);
        var service = CreateService(userManager.Object);
        var tokens = await IssueTokensAsync(service);

        Assert.Null(await service.RefreshTokensAsync(tokens.RefreshToken));
    }

    [Fact]
    public async Task GetPrincipalFromToken_ReturnsClaims_OrNullForGarbage()
    {
        var service = CreateService();
        var tokens = await IssueTokensAsync(service);

        var principal = service.GetPrincipalFromToken(tokens.AccessToken);

        Assert.NotNull(principal);
        Assert.True(principal!.IsInRole("Admin"));
        Assert.Null(service.GetPrincipalFromToken("garbage"));
    }
}
