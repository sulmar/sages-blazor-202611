namespace BlazorApp.Security;

/// <summary>Typy oswiadczen (claims) w tokenie wystawianym przez IdentityProvider.Api.</summary>
public static class AppClaimTypes
{
    public const string Name = "name";
    public const string Role = "role";
    public const string Permission = "permission";
    public const string Department = "Department";
    public const string Expires = "exp";

    /// <summary>Pod tym typem przechowujemy caly token dostepu.</summary>
    public const string AccessToken = "access_token";
}
