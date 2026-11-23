namespace BlazorApp.Services;

public sealed class Debouncer : IDisposable
{
    private CancellationTokenSource? cts;

    public async Task DebounceAsync(int milliseconds, Func<CancellationToken, Task> action)
    {
        cts?.Cancel();
        cts?.Dispose();

        cts = new CancellationTokenSource();
        var token = cts.Token;

        try
        {
            await Task.Delay(milliseconds, token);
            await action(token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
    }

    public Task DebounceAsync(int milliseconds, Func<Task> action) => DebounceAsync(milliseconds, _ => action());

    public void Dispose()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
