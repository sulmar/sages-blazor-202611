using Domain.Abstractions;
using Domain.Models;

namespace Shop.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomers(this IEndpointRouteBuilder app)
    {
        // Dane klientow sa chronione w calosci: polityka obejmuje kazdy endpoint grupy.
        var customers = app.MapGroup("/api/customers"); //.RequireAuthorization(Policies.Sales);

        customers.MapGet("/", async (string? text, ICustomerRepository repository) =>
            string.IsNullOrWhiteSpace(text)
                ? await repository.GetAllAsync()
                : await repository.GetByTextAsync(text));

        customers.MapGet("/page", async (int skip, int take, ICustomerRepository repository) =>
            skip < 0 || take is < 1 or > 200
                ? Results.BadRequest("skip >= 0, take od 1 do 200.")
                : Results.Ok(await repository.GetPageAsync(skip, take)));

        customers.MapGet("/{id:int}", async (int id, ICustomerRepository repository) =>
            await repository.GetByIdAsync(id) is { } customer
                ? Results.Ok(customer)
                : Results.NotFound());

        customers.MapPost("/", async (Customer customer, ICustomerRepository repository) =>
        {
            customer.Id = 0;
            await repository.AddAsync(customer);

            return Results.Created($"/api/customers/{customer.Id}", customer);
        }).RequireAuthorization(Policies.CanEdit);

        customers.MapPut("/{id:int}", async (int id, Customer customer, ICustomerRepository repository) =>
        {
            if (await repository.GetByIdAsync(id) is null)
                return Results.NotFound();

            customer.Id = id;
            await repository.UpdateAsync(customer);

            return Results.NoContent();
        }).RequireAuthorization(Policies.CanEdit);

        return app;
    }
}
