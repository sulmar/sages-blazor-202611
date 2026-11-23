using Domain.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Pages.Customers;

public partial class List : ComponentBase
{

    // [Inject]
    // private ICustomerRepository Repository { get; set; }

    private IEnumerable<Customer>? customers;

    protected override async Task OnInitializedAsync()
    {
        customers = await Repository.GetAllAsync();
    }
}
