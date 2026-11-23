namespace Domain.Models;

/// <summary>Podsumowanie katalogu wysyłane przez SignalR do paneli: liczba produktów i ich średnia cena.</summary>
public record ProductStats(int Count, decimal AveragePrice);
