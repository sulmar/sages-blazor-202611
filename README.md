# Panel sklepu - materiały ze szkolenia Tworzenie aplikacji Blazor

## Wprowadzenie

Witaj w repozytorium z materiałami do szkolenia **Tworzenie aplikacji Blazor**.

Repozytorium zawiera **gotowe zaplecze** - model domenowy, dane testowe, REST API sklepu i serwer wystawiający tokeny JWT. Dzięki temu na szkoleniu zajmujemy się wyłącznie Blazorem, a nie pisaniem od zera rzeczy, które i tak już znasz.

Zaplecza nie zmieniasz. Cały kod piszesz w nowym projekcie `src/BlazorApp`, który utworzysz w kroku 1.

Do rozpoczęcia kursu potrzebujesz:

1. [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (wersja zgodna z `src/global.json`)
2. Edytora: Visual Studio 2026, JetBrains Rider albo Visual Studio Code z rozszerzeniem C# Dev Kit

## Przygotowanie

1. Sklonuj repozytorium Git

```bash
git clone https://github.com/sulmar/sages-blazor-202611.git
cd sages-blazor-202611
```

2. Zaufaj certyfikatowi deweloperskiemu (jednorazowo, potrzebne do HTTPS)

```bash
dotnet dev-certs https --trust
```

3. Zbuduj rozwiązanie

```bash
cd src
dotnet build
```

## Struktura repozytorium

| Katalog | Zawartość |
| --- | --- |
| `src/Domain` | Model domenowy (`Product`, `Customer`, `Page<T>`, `ProductStats`) i interfejsy repozytoriów |
| `src/Infrastructure` | Repozytoria w pamięci, generator danych testowych (Bogus), rejestracja `AddInfrastructure()` |
| `src/Shop.Api` | REST API produktów i klientów, polityki autoryzacji, hub SignalR `/hubs/products` |
| `src/IdentityProvider.Api` | Serwer logowania wystawiający token JWT |
| `src/BlazorCourse.sln` | Rozwiązanie; projekt `BlazorApp` dodasz w kroku 1 |
| `docs` | `backend.md` - kontrakt zaplecza, `cwiczenia/` - ćwiczenia i wyzwania |

## Uruchomienie zaplecza

Dwie usługi uruchamiasz w osobnych terminalach, z katalogu `src`:

```bash
dotnet run --project Shop.Api               # dane sklepu, http://localhost:5200
dotnet run --project IdentityProvider.Api   # logowanie, http://localhost:5143
```

Sprawdź, czy `Shop.Api` działa: `http://localhost:5200/health` zwraca `{"status":"ok"}`.

Dane żyją w pamięci procesu. Restart `Shop.Api` przywraca dane startowe.

### Użytkownicy testowi

| Login | Hasło | Dostęp |
| --- | --- | --- |
| `john` | `123` | odczyt produktów |
| `bob` | `bobsecure` | + dodawanie i edycja produktów i klientów |
| `alice` | `alicepass` | + usuwanie produktów |

### Logowanie

`POST http://localhost:5143/api/login` z ciałem `{ "username": "alice", "password": "alicepass" }` zwraca `200 OK` z polem `accessToken` (JWT, ważny 30 minut) albo `401 Unauthorized`. Token wysyłasz w nagłówku `Authorization: Bearer <accessToken>`.

Szczegóły kont, claimów, polityk i endpointów znajdziesz w [docs/backend.md](docs/backend.md).

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

## Ćwiczenia i wyzwania

Szkolenie trwa trzy dni. Każdy dzień składa się z ćwiczeń etapowych, zadań A-E (dni 1 i 2) i wyzwania na koniec dnia. Pełny opis i kryteria akceptacji są w [docs/cwiczenia/README.md](docs/cwiczenia/README.md).

| Dzień | Zadania | Ćwiczenia | Wyzwanie |
| --- | --- | --- | --- |
| 1 | A (etap 2), B (etap 3), C (etap 5) | [dzien-1.md](docs/cwiczenia/dzien-1.md), etapy 1-5 | [Wygasające produkty](docs/cwiczenia/dzien-1.md#wyzwanie-dnia-1-wygasające-produkty) |
| 2 | D (etap 6), E (etap 9) | [dzien-2.md](docs/cwiczenia/dzien-2.md), etapy 6-11 | [Duplikuj produkt](docs/cwiczenia/dzien-2.md#wyzwanie-dnia-2-duplikuj-produkt) |
| 3 | - | [dzien-3.md](docs/cwiczenia/dzien-3.md), etapy 12-16 | [Drogie produkty na żywo](docs/cwiczenia/dzien-3.md#wyzwanie-dnia-3-drogie-produkty-na-żywo) |

> Gdy coś nie działa, najpierw przeczytaj komunikat w konsoli `dotnet watch` i w konsoli przeglądarki (F12). Sprawdź też, czy działają obie usługi zaplecza.

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
