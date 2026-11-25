using Domain.Models;
using System.Net.Http.Json;

namespace BlazorWasmApp.Services;

public class CustomerService(HttpClient http)
{
    public async Task<IEnumerable<Customer>?> GetAllAsync(CancellationToken cancellationToken = default) => 
        await http.GetFromJsonAsync<IEnumerable<Customer>>("api/customers", cancellationToken);
}
