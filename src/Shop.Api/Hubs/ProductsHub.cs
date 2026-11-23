using Microsoft.AspNetCore.SignalR;

namespace Shop.Api.Hubs;

/// <summary>
/// Kanał, którym API informuje panele o zmianie liczby produktów.
/// Hub jest celowo bez autoryzacji: wysyła tylko liczbę, bez danych produktów i klientów.
/// Dla danych wrażliwych trzeba dodać autoryzację (token w połączeniu), zanim skopiujesz ten wzorzec.
/// Klient nic nie wywołuje - tylko nasłuchuje.
/// </summary>
public class ProductsHub : Hub
{
}
