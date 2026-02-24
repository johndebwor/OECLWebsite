using Microsoft.AspNetCore.Identity;
using OECLWebsite.Infrastructure.Identity;

namespace OECLWebsite.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/api/auth");

        auth.MapPost("/login-form", async (
            HttpContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager) =>
        {
            var form = await context.Request.ReadFormAsync();
            var email = form["email"].ToString();
            var password = form["password"].ToString();
            var rememberMe = form["rememberMe"] == "true";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return Results.Redirect("/auth/login?error=Please+provide+email+and+password");
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user is null || !user.IsActive)
            {
                return Results.Redirect("/auth/login?error=Invalid+credentials");
            }

            var result = await signInManager.PasswordSignInAsync(
                user, password, rememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await userManager.UpdateAsync(user);
                return Results.Redirect("/admin/dashboard");
            }

            if (result.IsLockedOut)
            {
                return Results.Redirect("/auth/login?error=Account+is+locked.+Try+again+later");
            }

            return Results.Redirect("/auth/login?error=Invalid+credentials");
        }).DisableAntiforgery();

        auth.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Redirect("/");
        }).DisableAntiforgery();

        auth.MapGet("/logout", async (SignInManager<ApplicationUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Redirect("/");
        });
    }
}
