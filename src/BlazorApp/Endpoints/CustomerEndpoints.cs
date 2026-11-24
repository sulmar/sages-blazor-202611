using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointConventionBuilder MapCustomer(this IEndpointRouteBuilder app)
    {
        return app.MapPost("/customers/{Id:int}/edit", (int id, [FromForm] Customer customer) =>
        {


            return Results.Redirect("/customers");
        }); //.DisableAntiforgery();

    }
}