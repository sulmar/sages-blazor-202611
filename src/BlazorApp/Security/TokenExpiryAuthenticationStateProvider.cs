using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace BlazorApp.Security;

/// <summary>
/// Ciasteczko jest sprawdzane tylko przy zadaniu HTTP, a obwod zyje dalej bez kolejnych zadan.
/// Dlatego co pewien czas sprawdzamy, czy token jest jeszcze wazny; gdy wygasnie,
/// uzytkownik obwodu staje sie anonimowy i trafia na strone logowania.
/// </summary>
public sealed class TokenExpiryAuthenticationStateProvider(ILoggerFactory loggerFactory)
    : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    protected override TimeSpan RevalidationInterval => TimeSpan.FromSeconds(30);

    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken) =>
        Task.FromResult(authenticationState.User.GetTokenExpiry() > DateTimeOffset.UtcNow);
}
