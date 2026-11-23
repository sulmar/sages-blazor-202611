# Panel sklepu - start szkolenia

Zaplecze sklepu (domena, dane w pamięci, REST API, logowanie) jest gotowe. Twoim zadaniem jest panel w Blazorze.
Zaplecza nie zmieniasz. Cały kod piszesz w nowym projekcie `src/BlazorApp`, który utworzysz w kroku 1.

## Wymagania

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (wersja zgodna z `src/global.json`)
- edytor: Visual Studio, Rider albo VS Code z rozszerzeniem C#
- certyfikat deweloperski, jednorazowo: `dotnet dev-certs https --trust`

## Krok 0: uruchom zaplecze

W dwóch osobnych terminalach, z katalogu `src`:

```bash
dotnet run --project Shop.Api               # dane sklepu, http://localhost:5200
dotnet run --project IdentityProvider.Api   # logowanie, http://localhost:5143
```

Sprawdź: `http://localhost:5200/health` zwraca `{"status":"ok"}`.

Konta do logowania i szczegóły endpointów: [docs/backend.md](docs/backend.md).

Ćwiczenia i wyzwania na każdy dzień: [docs/cwiczenia/README.md](docs/cwiczenia/README.md).

| Login | Hasło | Dostęp |
| --- | --- | --- |
| `john` | `123` | odczyt produktów |
| `bob` | `bobsecure` | + dodawanie i edycja produktów i klientów |
| `alice` | `alicepass` | + usuwanie produktów |

## Krok 1: utwórz panel Blazor

W katalogu `src`:

```bash
dotnet new blazor --interactivity Server --all-interactive -n BlazorApp -o BlazorApp
dotnet sln add BlazorApp
dotnet add BlazorApp reference Domain Infrastructure
```

Następnie:

1. W `BlazorApp/Program.cs`, przed `var app = builder.Build();`, dodaj:

   ```csharp
   builder.Services.AddInfrastructure();
   ```

2. W `BlazorApp/Properties/launchSettings.json` ustaw adresy `http://localhost:5221` i `https://localhost:7295`.

3. Uruchom panel z automatycznym odświeżaniem po zmianach:

   ```bash
   dotnet watch --project BlazorApp
   ```

Panel działa pod `http://localhost:5221`. Na razie używa repozytoriów w pamięci, czyli danych z `Infrastructure`.

## Co jest w tym repozytorium

| Projekt | Rola |
| --- | --- |
| `Domain` | modele `Product`, `Customer`, `Page<T>`, `ProductStats` i interfejsy repozytoriów |
| `Infrastructure` | repozytoria w pamięci z danymi testowymi (Bogus), rejestracja `AddInfrastructure()` |
| `Shop.Api` | REST API produktów i klientów, polityki autoryzacji, hub SignalR `/hubs/products` |
| `IdentityProvider.Api` | logowanie, zwraca token JWT |
| `BlazorApp` | twój panel (krok 1) |

## Mapa etapów

Kolejność jest ważna: każdy etap dokłada jeden mechanizm do poprzedniego.

| Etap | Potrzeba | Mechanizm |
| --- | --- | --- |
| 1 | Potrzebujemy panelu sklepu | projekt Blazor Web App, struktura aplikacji, Interactive Server |
| 2 | Chcę zobaczyć produkty, a lista ładuje się 2 s | strona, `partial class`, `[Inject]`, stany ładowania, pustej listy i błędu |
| 3 | Chcę zobaczyć jeden produkt i cenę z rabatem | parametr trasy, `:int`, query string, komponent `QrCode` z endpointem |
| 4 | Lista klientów to kopia listy produktów | komponenty generyczne, `RenderFragment<T>` |
| 5 | 50 pozycji, trzeba szukać i stronicować | `@bind`, `EventCallback`, `ElementReference`, kryteria w adresie |
| 6 | Trzeba poprawić cenę i dodać produkt | `EditForm`, FluentValidation, DataAnnotations |
| 7 | Produkt nie ma zdjęcia | `InputFile`, strumieniowe przesyłanie pliku |
| 8 | Zdjęcie prosto z kamery | `IJSRuntime`, moduły JS, `IJSStreamReference`, `[JSInvokable]` |
| 9 | Strona startowa jest pusta | `InfoCard`, CSS globalny i izolowany |
| 10 | Klient po zeskanowaniu kodu QR widzi stronę produktu | własny układ, `@layout` |
| 11 | Motyw i rozmiar strony giną po F5 | Local Storage, Session Storage, układ zagnieżdżony |
| 12 | Każdy, kto zna adres panelu, może usunąć produkt | logowanie ciasteczkiem, `[Authorize]`, polityki, `AuthorizeView` |
| 13 | Rabat i opóźnienia są zaszyte w kodzie | `appsettings.json`, nadpisania dla środowiska, opcje z walidacją |
| 14 | Panel pobiera dane z osobnego serwisu HTTP | nazwany `HttpClient`, repozytoria wołające API |
| 15 | Użytkownik widzi zrozumiały komunikat zamiast wyjątku | obsługa błędów API |
| 16 | Liczba produktów i średnia cena nie odświeżają się bez F5 | `HubConnection`, `InvokeAsync` + `StateHasChanged` |
