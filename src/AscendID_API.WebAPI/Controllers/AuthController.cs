using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AscendID_API.Application.Interfaces.Identity;
using AscendID_API.WebAPI.ViewModels;

namespace AscendID_API.WebAPI.Controllers;

/// <summary>
/// MVC Controller for authentication views (login, logout pages)
/// </summary>
[Route("Auth")]
public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly AscendID_API.Application.Interfaces.Identity.IAuthenticationService _authService;

    public AuthController(IUserService userService, AscendID_API.Application.Interfaces.Identity.IAuthenticationService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    /// <summary>
    /// Display login page
    /// </summary>
    /// <param name="returnUrl">URL to redirect to after login</param>
    /// <returns>Login view</returns>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (_authService.IsAuthenticated())
        {
            return LocalRedirect(returnUrl ?? "/");
        }

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    /// <summary>
    /// Handle login form submission
    /// </summary>
    /// <param name="model">Login model</param>
    /// <param name="returnUrl">URL to redirect to after login</param>
    /// <returns>Redirect to return URL or login page with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var result = await _authService.SignInAsync(model.Email, model.Password, model.RememberMe);

        if (result.Success)
        {
            return LocalRedirect(returnUrl ?? "/");
        }

        ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid login attempt");
        ViewBag.ReturnUrl = returnUrl;
        return View(model);
    }

    /// <summary>
    /// Handle logout
    /// </summary>
    /// <returns>Redirect to home page</returns>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Display access denied page
    /// </summary>
    /// <returns>Access denied view</returns>
    public IActionResult AccessDenied()
    {
        return View();
    }
}