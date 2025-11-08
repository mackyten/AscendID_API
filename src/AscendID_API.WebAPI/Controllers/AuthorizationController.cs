using System.Security.Claims;
using System.Collections.Immutable;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using AscendID_API.Domain.Entities.Identity;
using AscendID_API.Application.Interfaces.Identity;

namespace AscendID_API.WebAPI.Controllers;

/// <summary>
/// Controller for OpenIddict authorization endpoints
/// </summary>
[ApiController]
[Route("connect")]
public class AuthorizationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserService _userService;

    public AuthorizationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUserService userService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
    }

    /// <summary>
    /// Authorization endpoint for OAuth2/OIDC flows
    /// </summary>
    /// <returns>Authorization response</returns>
    [HttpGet("authorize")]
    [HttpPost("authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the user principal stored in the authentication cookie.
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

        // If the user principal can't be extracted, redirect the user to the login page.
        if (!result.Succeeded)
        {
            // If the client application requested promptless authentication,
            // return an error indicating that the user is not logged in.
            if (request.HasPrompt(OpenIddictConstants.Prompts.None))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                    }));
            }

            return Challenge(
                authenticationSchemes: IdentityConstants.ApplicationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                });
        }

        // Retrieve the profile of the logged in user.
        var user = await _userManager.GetUserAsync(result.Principal) ??
            throw new InvalidOperationException("The user details cannot be retrieved.");

        // Retrieve the application details from the database.
        var application = await HttpContext.RequestServices.GetRequiredService<IOpenIddictApplicationManager>()
            .FindByClientIdAsync(request.ClientId!) ??
            throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

        // Create a new principal
        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        // Set the list of scopes granted to the client application.
        principal.SetScopes(new[]
        {
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles
        }.Intersect(request.GetScopes()));

        // Add claims for the user
        principal.SetClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString());
        
        if (principal.HasScope(OpenIddictConstants.Scopes.Email))
        {
            principal.SetClaim(OpenIddictConstants.Claims.Email, user.Email!);
            principal.SetClaim(OpenIddictConstants.Claims.EmailVerified, user.EmailConfirmed);
        }

        if (principal.HasScope(OpenIddictConstants.Scopes.Profile))
        {
            principal.SetClaim(OpenIddictConstants.Claims.Name, user.FullName);
            principal.SetClaim(OpenIddictConstants.Claims.GivenName, user.FirstName);
            principal.SetClaim(OpenIddictConstants.Claims.FamilyName, user.LastName);
            principal.SetClaim(OpenIddictConstants.Claims.PreferredUsername, user.UserName!);
        }

        if (principal.HasScope(OpenIddictConstants.Scopes.Roles))
        {
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                principal.SetClaim(OpenIddictConstants.Claims.Role, role);
            }
        }

        // Set the list of destinations to return in the access_token.
        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, principal));
        }

        // Update last login timestamp
        await _userService.UpdateLastLoginAsync(user.Id);

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Token endpoint for OAuth2/OIDC flows
    /// </summary>
    /// <returns>Token response</returns>
    [HttpPost("token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsAuthorizationCodeGrantType())
        {
            // Retrieve the claims principal stored in the authorization code.
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Retrieve the user profile corresponding to the authorization code.
            var user = await _userManager.FindByIdAsync(result.Principal!.GetClaim(OpenIddictConstants.Claims.Subject)!);
            if (user is null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                    }));
            }

            // Ensure the user is still allowed to sign in.
            if (!await _signInManager.CanSignInAsync(user))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                    }));
            }

            // Create a new principal
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Set the list of scopes granted to the client application.
            principal.SetScopes(request.GetScopes());

            // Set the list of destinations to return in the access_token.
            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsRefreshTokenGrantType())
        {
            // Retrieve the claims principal stored in the refresh token.
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Retrieve the user profile corresponding to the refresh token.
            var user = await _userManager.FindByIdAsync(result.Principal!.GetClaim(OpenIddictConstants.Claims.Subject)!);
            if (user is null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The refresh token is no longer valid."
                    }));
            }

            // Ensure the user is still allowed to sign in.
            if (!await _signInManager.CanSignInAsync(user))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                    }));
            }

            // Create a new principal
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Set the list of scopes granted to the client application.
            principal.SetScopes(request.GetScopes());

            // Set the list of destinations to return in the access_token.
            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }

    /// <summary>
    /// UserInfo endpoint for OIDC
    /// </summary>
    /// <returns>User information</returns>
    [HttpGet("userinfo")]
    [HttpPost("userinfo")]
    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Userinfo()
    {
        var user = await _userManager.FindByIdAsync(User.GetClaim(OpenIddictConstants.Claims.Subject)!);
        if (user is null)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified access token is bound to an account that no longer exists."
                }));
        }

        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [OpenIddictConstants.Claims.Subject] = user.Id.ToString()
        };

        if (User.HasScope(OpenIddictConstants.Scopes.Email))
        {
            claims[OpenIddictConstants.Claims.Email] = user.Email!;
            claims[OpenIddictConstants.Claims.EmailVerified] = user.EmailConfirmed;
        }

        if (User.HasScope(OpenIddictConstants.Scopes.Profile))
        {
            claims[OpenIddictConstants.Claims.Name] = user.FullName;
            claims[OpenIddictConstants.Claims.GivenName] = user.FirstName;
            claims[OpenIddictConstants.Claims.FamilyName] = user.LastName;
            claims[OpenIddictConstants.Claims.PreferredUsername] = user.UserName!;
        }

        if (User.HasScope(OpenIddictConstants.Scopes.Roles))
        {
            var roles = await _userManager.GetRolesAsync(user);
            claims[OpenIddictConstants.Claims.Role] = roles.ToArray();
        }

        // Note: the complete list of standard claims supported by the OpenID Connect specification
        // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

        return Ok(claims);
    }

    /// <summary>
    /// Logout endpoint for OIDC
    /// </summary>
    /// <returns>Logout response</returns>
    [HttpGet("logout")]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Ask ASP.NET Core Identity to delete the local and external cookies created
        // when the user agent is redirected from the external identity provider
        // after a successful authentication flow (e.g Google or Facebook).
        await _signInManager.SignOutAsync();

        // Returning a SignOutResult will ask OpenIddict to redirect the user agent
        // to the post_logout_redirect_uri specified by the client application or to
        // the RedirectUri specified in the authentication properties if none was set.
        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    }

    private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Name:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(OpenIddictConstants.Scopes.Profile))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Email:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(OpenIddictConstants.Scopes.Email))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Role:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(OpenIddictConstants.Scopes.Roles))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield break;
        }
    }
}