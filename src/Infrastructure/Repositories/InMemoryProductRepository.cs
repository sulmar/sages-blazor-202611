using Domain.Abstractions;
using Domain.Models;

namespace Infrastructure.Repositories;

public class InMemoryProductRepository : InMemoryEntityRepository<Product>, IProductRepository
{
    public InMemoryProductRepository(IEnumerable<Product> entities, InMemoryRepositoryOptions? options = null)
        : base(entities, options)
    {
    }

    public async Task<IEnumerable<Product>> GetByColorAsync(string color)
    {
        if (string.IsNullOrEmpty(color))
            return [];

        await SimulateSearchLatencyAsync();

        return Snapshot()
            .Where(p => string.Equals(p.Color, color, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<IEnumerable<Product>> GetByTextAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Enumerable.Empty<Product>();

        await SimulateSearchLatencyAsync();

        return Snapshot()
            .Where(c => c.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
            || c.Description.Contains(text, StringComparison.OrdinalIgnoreCase) || c.Price.ToString().Contains(text)).ToList();
    }
}
