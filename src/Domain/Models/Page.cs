namespace Domain.Models;

/// <summary>Jedna strona wynikow razem z laczna liczba pozycji.</summary>
public record Page<T>(IReadOnlyList<T> Items, int TotalCount);
