using BlazorApp.Components;
using BlazorApp.Endpoints;
using BlazorApp.Security;
using BlazorApp.Services;
using Bogus;
using Domain.Abstractions;
using Domain.Models;
using Infrastructure.Fakers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddTransient<Faker<Customer>, CustomerFaker>();
builder.Services.AddTransient<IEnumerable<Customer>>(sp => sp.GetRequiredService<Faker<Customer>>().Generate(20));


builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddTransient<Faker<Product>, ProductFaker>();
builder.Services.AddTransient<IEnumerable<Product>>(sp => sp.GetRequiredService<Faker<Product>>().Generate(20));


// Rejestracja parametrów kaskadowych globalnie
builder.Services.AddCascadingValue("a", _ => 5); 
builder.Services.AddCascadingValue("b", _ => 1);

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<LocalStorageService>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
    });

builder.Services.AddAuthorization();

// Komponenty otrzymują użytkownika jako CascadingValue Task<AuthenticationState>:
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, TokenExpiryAuthenticationStateProvider>();

// Nazwany klient HTTP: adres serwera logowania pochodzi z konfiguracji.
builder.Services.AddHttpClient(IdentityProviderClient.HttpClientName, client =>
{
    var baseAddress = builder.Configuration["IdentityProvider:BaseAddress"]
        ?? throw new InvalidOperationException("Brak ustawienia IdentityProvider:BaseAddress.");

    client.BaseAddress = new Uri(baseAddress);
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddScoped<IdentityProviderClient>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
    app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapCustomer();

app.MapQrCode();

app.MapSecurityEndpoints();

app.Run();
