using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BlazorApp.Security;

/// <summary>
/// Endpointy logowania i wylogowania. Logowanie odbywa sie przez IdentityProvider.Api (serwer-serwer),
/// a wynik zapisujemy w ciasteczku. Ciasteczko mozna ustawic tylko w odpowiedzi HTTP,
/// dlatego formularz nie moze byc komponentem interaktywnym - obsluguje go ten endpoint.
/// </summary>
public static class SecurityExtensions
{
    public static IEndpointRouteBuilder MapSecurityEndpoints(this IEndpointRouteBuilder app)
    {
        // Inna sciezka niz strona /login (Razor), bo strona przyjmuje kazda metode HTTP i POST bylby niejednoznaczny.
        app.MapPost("/account/login", LoginAsync).DisableAntiforgery().ExcludeFromDescription();
        app.MapPost("/account/logout", LogoutAsync).DisableAntiforgery().ExcludeFromDescription();

        return app;
    }

    private static async Task<IResult> LoginAsync(
        HttpContext httpContext,
        IAntiforgery antiforgery,
        IdentityProviderClient identityProvider)
    {
        await antiforgery.ValidateRequestAsync(httpContext);

        var form = await httpContext.Request.ReadFormAsync();
        var username = form["username"].ToString().Trim();
        var password = form["password"].ToString();
        var returnUrl = form["returnUrl"].ToString();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return Results.Redirect(LoginUrl("invalid", returnUrl));

        string? accessToken;

        try
        {
            accessToken = await identityProvider.LoginAsync(username, password, httpContext.RequestAborted);
        }
        catch (HttpRequestException)
        {
            return Results.Redirect(LoginUrl("server", returnUrl));
        }

        if (accessToken is null)
            return Results.Redirect(LoginUrl("invalid", returnUrl));

        var principal = TokenPrincipal.Create(accessToken);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Results.Redirect(SafeLocalUrl(returnUrl));
    }

    private static async Task<IResult> LogoutAsync(HttpContext httpContext, IAntiforgery antiforgery)
    {
        await antiforgery.ValidateRequestAsync(httpContext);
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Results.Redirect("/");
    }

    private static string LoginUrl(string error, string returnUrl) =>
        $"/login?error={error}&returnUrl={Uri.EscapeDataString(SafeLocalUrl(returnUrl))}";

    // Przekierowujemy tylko na adresy w tej samej aplikacji (zapobiega open redirect).
    private static string SafeLocalUrl(string? url) =>
        !string.IsNullOrEmpty(url) && url.StartsWith('/') && !url.StartsWith("//") && !url.StartsWith("/\\")
            ? url
            : "/";
}
