using Microsoft.JSInterop;

namespace BlazorApp.Services;

public class DialogService(IJSRuntime js)
{
    // Fasade
    public async Task<bool> ConfirmAsync(string message) => await js.InvokeAsync<bool>("confirm", message);
}
