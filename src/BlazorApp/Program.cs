using BlazorApp.Components;
using BlazorApp.Endpoints;
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


// Rejestracja parametrów kaskadowych globalnie
builder.Services.AddCascadingValue("a", _ => 5); 
builder.Services.AddCascadingValue("b", _ => 1);


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

app.MapCustomer();

app.MapQrCode();

app.Run();
