using System.Collections.Concurrent;
using Domain.Abstractions;
using Domain.Models;

namespace Infrastructure.Repositories;

public abstract class InMemoryEntityRepository<T> : IEntityRepository<T>
    where T : BaseEntity
{
    // ConcurrentDictionary, bo repozytorium jest singletonem i korzysta z niego
    // wielu uzytkownikow (obwodow Blazora) jednoczesnie.
    protected readonly ConcurrentDictionary<int, T> _entities;
    protected readonly InMemoryRepositoryOptions _options;
    private int _lastId;

    public InMemoryEntityRepository(IEnumerable<T> entities, InMemoryRepositoryOptions? options = null)
    {
        _entities = new ConcurrentDictionary<int, T>(entities.ToDictionary(c => c.Id));
        _options = options ?? new InMemoryRepositoryOptions();
        _lastId = _entities.IsEmpty ? 0 : _entities.Keys.Max();
    }

    /// <summary>Udaje czas odpowiedzi serwera przy pobieraniu danych.</summary>
    protected Task SimulateLatencyAsync() => DelayAsync(_options.Latency);

    /// <summary>Udaje czas odpowiedzi serwera przy wyszukiwaniu.</summary>
    protected Task SimulateSearchLatencyAsync() => DelayAsync(_options.SearchLatency);

    // Task.Delay(TimeSpan.Zero) i tak oddaje sterowanie i przerzuca kontynuacje
    // przez scheduler, wiec wylaczone opoznienie skracamy do CompletedTask.
    private static Task DelayAsync(TimeSpan delay) =>
        delay > TimeSpan.Zero ? Task.Delay(delay) : Task.CompletedTask;

    /// <summary>Migawka danych posortowana po Id - bezpieczna do enumeracji.</summary>
    protected List<T> Snapshot() => _entities.Values.OrderBy(e => e.Id).ToList();

    public Task AddAsync(T entity)
    {
        // Nowa encja (Id = 0) dostaje kolejny identyfikator, tak jak w bazie danych.
        if (entity.Id == 0)
            entity.Id = Interlocked.Increment(ref _lastId);

        if (!_entities.TryAdd(entity.Id, entity))
            throw new InvalidOperationException($"{typeof(T).Name} o Id = {entity.Id} juz istnieje.");

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _entities.TryRemove(id, out _);

        return Task.CompletedTask;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        await SimulateLatencyAsync();

        return Snapshot();
    }

    public async Task<Page<T>> GetPageAsync(int skip, int take)
    {
        await SimulateSearchLatencyAsync();

        var all = Snapshot();

        return new Page<T>(all.Skip(skip).Take(take).ToList(), all.Count);
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        await SimulateLatencyAsync();

        return _entities.TryGetValue(id, out var entity) ? entity : null;
    }

    public Task UpdateAsync(T entity)
    {
        if (!_entities.ContainsKey(entity.Id))
            throw new KeyNotFoundException($"{typeof(T).Name} o Id = {entity.Id} nie istnieje.");

        _entities[entity.Id] = entity;

        return Task.CompletedTask;
    }
}
