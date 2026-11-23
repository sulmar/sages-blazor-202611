using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shop.Api;
using Shop.Api.Endpoints;
using Shop.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Dane sklepu zyja teraz tutaj: korzysta z nich panel, a w przyszlosci takze inne aplikacje.
builder.Services.AddInfrastructure(options => builder.Configuration.GetSection("Repository").Bind(options));

// API nie ufa wywolujacemu: kazdy token sprawdza samo - podpis, wystawce, odbiorce i termin waznosci.
// Klucz i nazwy musza byc takie same jak w IdentityProvider.Api (sekcja JwtSettings).
var jwt = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Zostawiamy krotkie nazwy oswiadczen z tokenu ("role", "name") zamiast dlugich adresow XML.
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                jwt["SecretKey"] ?? throw new InvalidOperationException("Brak ustawienia JwtSettings:SecretKey."))),
            NameClaimType = "name",
            RoleClaimType = "role",
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

// SignalR: API informuje podłączone panele o zmianach (patrz ProductsHub).
builder.Services.AddSignalR();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.CanEdit, policy => policy.RequireClaim("permission", "canedit"))
    .AddPolicy(Policies.Sales, policy => policy.RequireClaim("Department", "Sales"))
    .AddPolicy(Policies.Admin, policy => policy.RequireRole("admin"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapProducts();
app.MapCustomers();
app.MapHub<ProductsHub>("/hubs/products");

app.Run();
