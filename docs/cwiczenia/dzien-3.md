# Dzień 3: etapy 12-16

Ćwiczenia zakładają etapy z prowadzącym: logowanie, polityki, konfigurację, klienta HTTP, obsługę błędów i SignalR.
Uruchom zaplecze przed startem (`Shop.Api` na porcie 5200, `IdentityProvider.Api` na porcie 5143).
Konta do testów są w [backend.md](../backend.md#konta).

## Ćwiczenie 12: rola w nagłówku (etap 12)

**Czas:** 15 minut. **Cel:** odczytać oświadczenia z tokenu w komponencie i sprawdzić blokadę na stronie.

**Kroki**

1. W układzie `MainLayout.razor`, obok nazwy użytkownika, pokaż jego role (oświadczenie `role`).
   Brak ról pokaż jako `brak ról`.
2. Zaloguj się jako `john` (bez ról i bez działu Sales). Wejdź na `/customers`.
3. Zaloguj się jako `alice` (rola `admin`). Sprawdź, co widzisz w nagłówku.

**Kryteria akceptacji**

- [ ] Nagłówek pokazuje role zalogowanego użytkownika.
- [ ] `john` trafia na `/access-denied` z `/customers`.
- [ ] `alice` widzi swoją rolę `admin`.
- [ ] Umiesz wskazać, skąd komponent bierze role (token, nie formularz).

<details>
<summary>Wskazówka</summary>

`context.User.FindAll("role")` zwraca wszystkie role. W `AuthorizeView` masz dostęp do `context.User`. Przy pustej liście wyświetl `brak ról`.
</details>

## Ćwiczenie 13: nazwa sklepu z konfiguracji (etap 13)

**Czas:** 15 minut. **Cel:** dodać ustawienie do `appsettings.json`, odczytać je przez opcje i sprawdzić walidację przy starcie.

**Kroki**

1. Dodaj do `ShopOptions` właściwość `Tagline` (domyślnie pusty napis). Ustaw ją w `appsettings.json` w sekcji `Shop`.
2. Wyświetl `Tagline` w stopce strony publicznej (`/p/{id}`).
3. Dodaj walidację: `Tagline` nie może mieć więcej niż 80 znaków. Dodaj ją w `Program.cs` przy opcjach `ShopOptions`.
4. Uruchom aplikację z nadpisaniem `Shop__Name=Mój sklep` w zmiennych środowiskowych. Potem ustaw zbyt długi `Tagline` i sprawdź start.

**Kryteria akceptacji**

- [ ] Nazwa sklepu zmienia się przez zmienną środowiskową bez zmiany kodu.
- [ ] Stopka pokazuje `Tagline`.
- [ ] Za długi `Tagline` zatrzymuje start z czytelnym komunikatem.
- [ ] Wiesz, co robi `ValidateOnStart()`.

<details>
<summary>Wskazówka</summary>

Walidacja: `.Validate(shop => shop.Tagline.Length <= 80, "...")` w tym samym łańcuchu co `DiscountRate`. Zmienne środowiskowe używają podwójnego podkreślenia: `Shop__Name`.
</details>

## Ćwiczenie 14: stronicowanie klientów w API (etap 14)

**Czas:** 15 minut. **Cel:** pobierać tylko jedną stronę klientów z Shop.Api zamiast całej listy.

**Kroki**

1. Na liście klientów zamień pobieranie całej listy na `GetPageAsync(skip, take)` z repozytorium klientów.
2. Przelicz `skip` z numeru strony i rozmiaru strony z preferencji. Liczbę stron weź z `TotalCount`.
3. Otwórz narzędzia deweloperskie (F12), zakładka Network, i sprawdź żądanie do `/api/customers/page`.

**Kryteria akceptacji**

- [ ] W sieci widać `skip` i `take` w żądaniu do `/api/customers/page`.
- [ ] Zmiana strony pobiera kolejną stronę z API.
- [ ] Liczba stron w `Pager` zgadza się z `TotalCount`.
- [ ] Wyszukiwanie po tekście nadal działa (kto go nie zmienia, zostaje przy `GetByTextAsync`).

<details>
<summary>Wskazówka</summary>

`Page<T>` ma właściwości `Items` i `TotalCount`. `skip = (page - 1) * pageSize`. Gdy użytkownik wpisuje tekst, przejdź na zwykłe wyszukiwanie: stronicowanie dotyczy tylko listy bez filtra.
</details>

## Ćwiczenie 15: komunikaty przy braku połączenia (etap 15)

**Czas:** 15 minut. **Cel:** sprawdzić, że użytkownik widzi zrozumiały komunikat, a nie treść wyjątku.

**Kroki**

1. Zatrzymaj `Shop.Api` (zostaw `IdentityProvider.Api` uruchomiony).
2. Otwórz listę produktów i szczegóły produktu. Sprawdź komunikaty.
3. Na stronie szczegółów dodaj przycisk `Spróbuj ponownie`, jeśli go nie ma, i uruchom ponownie `Shop.Api`.

**Kryteria akceptacji**

- [ ] Komunikat na liście mówi o braku połączenia z serwisem sklepu.
- [ ] Komunikat na szczegółach jest taki sam jak na liście (ta sama funkcja `ApiErrorMessage`).
- [ ] Nie widać żadnego tekstu wyjątku ani stosu.
- [ ] Po uruchomieniu `Shop.Api` przycisk `Spróbuj ponownie` przywraca dane.

<details>
<summary>Wskazówka</summary>

Ten sam mechanizm co na liście: `ApiErrorMessage.For(ex, "Nie udało się pobrać produktu.")`. Pamiętaj, że brak produktu (404) to nie błąd: `GetByIdAsync` zwraca wtedy `null`.
</details>

## Ćwiczenie 16: baner po zmianie katalogu (etap 16)

**Czas:** 15 minut. **Cel:** zareagować na zdarzenie SignalR na stronie listy produktów.

**Kroki**

1. Na liście produktów podłącz się do `ProductStatsNotifier` (zdarzenie `StatsChanged`) i uruchom połączenie.
2. Po zdarzeniu pokaż baner `Katalog się zmienił - odśwież listę`. Kliknięcie `Odśwież` go zamyka.
3. Otwórz listę w dwóch oknach. W jednym dodaj produkt. W drugim sprawdź baner.

**Kryteria akceptacji**

- [ ] Baner pojawia się w drugim oknie po dodaniu produktu, bez F5.
- [ ] Baner znika po kliknięciu `Odśwież`.
- [ ] Odłączenie strony (przejście na inną) nie zostawia subskrypcji: sprawdź, że przejście na inną stronę nie wywołuje błędu.
- [ ] Zmiana wywołana z wątku SignalR jest przekazana przez `InvokeAsync`.

<details>
<summary>Wskazówka</summary>

Wzór masz na pulpicie (`Home.razor`): `StatsChanged += ...` w `OnInitializedAsync`, `-=` w `DisposeAsync`, zmiana stanu w `InvokeAsync`.
</details>

---

## Wyzwanie dnia 3: Drogie produkty na żywo

**Czas:** 45-60 minut. **Etapy:** 12-16.

**Scenariusz.** Dział sprzedaży monitoruje produkty znacznie droższe od średniej. Lista ma się aktualizować bez odświeżania.

**Wymagania**

1. Strona `/products/premium` jest dostępna tylko dla polityki `Sales`. Pozycja `Drogie produkty` jest w menu, widoczna tylko dla tej polityki.
2. Próg liczony jest jako `Shop:PremiumPriceFactor` (domyślnie `1.5`). Walidacja przy starcie: wartość musi być większa niż 1.
3. Strona pokazuje produkty, których cena jest większa niż średnia cena katalogu razy próg. Lista jest posortowana malejąco po cenie.
4. Dane pobierasz tylko przez `IProductRepository` (Shop.Api). Błąd pokazujesz przez `ApiErrorMessage` z przyciskiem `Spróbuj ponownie`.
5. Na żywo: dodanie lub usunięcie produktu w drugim oknie przeładowuje listę bez F5 (zdarzenie `ProductStatsChanged`).
6. Odpowiedzi, które przychodzą po nowszym żądaniu, odrzucasz (jak `loadVersion` na liście produktów).

**Kryteria akceptacji**

- [ ] `alice` i `bob` widzą stronę i link w menu. `john` trafia na `/access-denied`.
- [ ] Zmiana `PremiumPriceFactor` w konfiguracji zmienia listę po restarcie.
- [ ] Wartość `PremiumPriceFactor` równa 1 albo mniejsza zatrzymuje start.
- [ ] Lista jest posortowana malejąco po cenie.
- [ ] Dodanie produktu w drugim oknie zmienia listę bez odświeżania.
- [ ] Wyłączenie `Shop.Api` daje komunikat z przyciskiem `Spróbuj ponownie`.

**Znane ograniczenie.** Edycja ceny (`PUT`) nie wysyła powiadomienia SignalR. Po zmianie ceny lista odświeża się dopiero po wejściu na stronę. Opisz to w podsumowaniu, nie naprawiaj zaplecza.

**Bonus ★:** wskaźnik `na żywo` (połączony / rozłączony) na podstawie stanu `HubConnection`.

<details>
<summary>Podpowiedź 1</summary>

Polityka na stronie: `@attribute [Authorize(Policy = Policies.Sales)]`, tak jak w katalogu `Customers`. W menu użyj `AuthorizeView Policy="@Policies.Sales"`.
</details>

<details>
<summary>Podpowiedź 2</summary>

Próg dodaj do `ShopOptions` i do walidacji w `Program.cs`, w tym samym miejscu co `DiscountRate`. Strona odczytuje go przez `IOptions<ShopOptions>`.
</details>

<details>
<summary>Podpowiedź 3</summary>

Średnią liczysz z pełnej listy produktów, nie z bieżącej strony. Filtr to `p.Price > srednia * prog`.
</details>

<details>
<summary>Podpowiedź 4</summary>

Subskrypcja jak na pulpicie: `StatsChanged += ...` w `OnInitializedAsync`, a w zdarzeniu `_ = InvokeAsync(LoadAsync)`. Nie zapomnij o `DisposeAsync`.
</details>
