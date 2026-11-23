using Domain.Abstractions;
using Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Shop.Api.Hubs;

namespace Shop.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProducts(this IEndpointRouteBuilder app)
    {
        // Grupa: wspolny prefiks adresu dla wszystkich endpointow produktow.
        var products = app.MapGroup("/api/products");

        // Odczyt jest publiczny - z katalogu korzysta tez strona produktu dla klientow.
        products.MapGet("/", async (string? text, string? color, IProductRepository repository) =>
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var found = await repository.GetByTextAsync(text);

                return found.Where(p => string.IsNullOrEmpty(color) || string.Equals(p.Color, color, StringComparison.OrdinalIgnoreCase));
            }

            return string.IsNullOrEmpty(color)
                ? await repository.GetAllAsync()
                : await repository.GetByColorAsync(color);
        });

        products.MapGet("/page", async (int skip, int take, IProductRepository repository) =>
            skip < 0 || take is < 1 or > 200
                ? Results.BadRequest("skip >= 0, take od 1 do 200.")
                : Results.Ok(await repository.GetPageAsync(skip, take)));

        products.MapGet("/{id:int}", async (int id, IProductRepository repository) =>
            await repository.GetByIdAsync(id) is { } product
                ? Results.Ok(product)
                : Results.NotFound());

        // Zmiany wymagaja tokenu z odpowiednim uprawnieniem.
        products.MapPost("/", async (Product product, IProductRepository repository, IHubContext<ProductsHub> hub) =>
        {
            // Identyfikator nadaje serwer - wartosc przyslana przez klienta ignorujemy.
            product.Id = 0;
            await repository.AddAsync(product);
            await NotifyStatsChangedAsync(repository, hub);

            return Results.Created($"/api/products/{product.Id}", product);
        }).RequireAuthorization(Policies.CanEdit);

        products.MapPut("/{id:int}", async (int id, Product product, IProductRepository repository) =>
        {
            if (await repository.GetByIdAsync(id) is null)
                return Results.NotFound();

            // O tym, ktory produkt zmieniamy, decyduje adres, a nie tresc zadania.
            product.Id = id;
            await repository.UpdateAsync(product);

            return Results.NoContent();
        }).RequireAuthorization(Policies.CanEdit);

        products.MapDelete("/{id:int}", async (int id, IProductRepository repository, IHubContext<ProductsHub> hub) =>
        {
            await repository.DeleteAsync(id);
            await NotifyStatsChangedAsync(repository, hub);

            return Results.NoContent();
        }).RequireAuthorization(Policies.Admin);

        return app;
    }

    /// <summary>Wysyla panelom liczbe produktow i ich srednia cene.</summary>
    private static async Task NotifyStatsChangedAsync(IProductRepository repository, IHubContext<ProductsHub> hub)
    {
        var products = (await repository.GetAllAsync()).ToList();
        var stats = new ProductStats(products.Count, products.Count > 0 ? products.Average(p => p.Price) : 0m);

        await hub.Clients.All.SendAsync("ProductStatsChanged", stats);
    }
}
