using BlazorWasmApp;
using BlazorWasmApp.Services;
using Domain.Abstractions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);



builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

 builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5200") });


// dotnet add package Microsoft.Extensions.Http 
builder.Services.AddHttpClient<CustomerService>(http => http.BaseAddress = new Uri("http://localhost:5200"));

await builder.Build().RunAsync();
