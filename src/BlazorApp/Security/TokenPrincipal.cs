using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Globalization;
using System.Security.Claims;

namespace BlazorApp.Security;



// dotnet add package Microsoft.IdentityModel.JsonWebTokens

/// <summary>Zamienia token JWT na uzytkownika aplikacji i odczytuje z niego dane tokenu.</summary>
public static class TokenPrincipal
{
    /// <summary>Buduje tozsamosc dla ciasteczka logowania z oswiadczen zapisanych w tokenie.</summary>
    public static ClaimsPrincipal Create(string accessToken)
    {
        // Token przyszedl bezposrednio z serwera logowania (wywolanie serwer-serwer),
        // dlatego tylko go odczytujemy. Podpis sprawdza ten, kto dostaje token od klienta - czyli API.
        var token = new JsonWebToken(accessToken);

        // Tablice z tokenu (role, permission) sa tu juz rozbite na osobne oswiadczenia.
        // Sam token dokladamy jako jeszcze jedno oswiadczenie: ciasteczko jest szyfrowane i HttpOnly,
        // wiec token zostaje po stronie serwera, a mamy go pod reka takze w obwodzie.
        var claims = token.Claims.Append(new Claim(AppClaimTypes.AccessToken, accessToken));

        // Wskazujemy, ktore oswiadczenia sa nazwa i rola: dzieki temu dzialaja
        // User.Identity.Name, User.IsInRole() oraz [Authorize(Roles = ...)].
        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme,
            nameType: AppClaimTypes.Name,
            roleType: AppClaimTypes.Role);

        return new ClaimsPrincipal(identity);
    }

    /// <summary>Token dostepu zalogowanego uzytkownika - do wywolan API w jego imieniu.</summary>
    public static string? GetAccessToken(this ClaimsPrincipal user) =>
        user.FindFirstValue(AppClaimTypes.AccessToken);

    /// <summary>Chwila, w ktorej token traci waznosc (oswiadczenie <c>exp</c>: sekundy od 1970 r.).</summary>
    public static DateTimeOffset? GetTokenExpiry(this ClaimsPrincipal user) =>
        long.TryParse(user.FindFirstValue(AppClaimTypes.Expires), NumberStyles.None, CultureInfo.InvariantCulture, out var seconds)
            ? DateTimeOffset.FromUnixTimeSeconds(seconds)
            : null;
}
