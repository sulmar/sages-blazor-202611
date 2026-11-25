using System.Net;

namespace BlazorApp.Security;

/// <summary>
/// Rozmawia z serwerem logowania (IdentityProvider.Api). Wywolanie idzie z serwera aplikacji,
/// a nie z przegladarki, wiec haslo i token nie przechodza przez JavaScript.
/// </summary>
public sealed class IdentityProviderClient(IHttpClientFactory httpClientFactory)
{
    /// <summary>Nazwa klienta HTTP zarejestrowanego w <see cref="SecurityExtensions"/>.</summary>
    public const string HttpClientName = "IdentityProvider";

    /// <summary>
    /// Zwraca token dostepu albo <c>null</c>, gdy login lub haslo sa bledne.
    /// Gdy serwer logowania nie odpowiada, zglasza <see cref="HttpRequestException"/>.
    /// </summary>
    public async Task<string?> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(HttpClientName);

        using var response = await client.PostAsJsonAsync("api/login", new LoginRequest(username, password), cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return null;

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);

        return result?.AccessToken ?? throw new HttpRequestException("Serwer logowania nie zwrocil tokenu.");
    }

    private sealed record LoginRequest(string Username, string Password);

    private sealed record LoginResponse(string? AccessToken);
}
