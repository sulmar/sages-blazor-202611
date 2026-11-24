using BlazorApp.Components;
using Bogus;
using Domain.Abstractions;
using Domain.Models;
using Infrastructure.Fakers;
using Infrastructure.Repositories;
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

app.MapStaticAssets();
    app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.MapPost("/customers/{Id:int}/edit", (int id, [FromForm] Customer customer) =>
{


    return Results.Redirect("/customers");
}); //.DisableAntiforgery();


// GET api/qrcode?data=
app.MapGet("/api/qrcode", (string data) =>
{
    const int MaxDataLength = 500;

    if (string.IsNullOrWhiteSpace(data) || data.Length > MaxDataLength)
        return Results.BadRequest($"Parametr data jest wymagany i może mieć maksymalną długosc {MaxDataLength} znaków");

    using var generator = new QRCodeGenerator();
    using var qrData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);

    var png = new PngByteQRCode(qrData).GetGraphic(pixelsPerModule: 10);

    return Results.File(png, "image/png");
});

app.Run();
