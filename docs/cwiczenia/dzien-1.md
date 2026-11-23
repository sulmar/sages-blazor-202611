# Dzień 1: etapy 1-5

Ćwiczenia wymagają etapów z prowadzącym: strona produktów z listą, szczegółami, tabelą i wyszukiwaniem.
Wszystko działa na danych z pamięci (`Infrastructure`), bez logowania.

Dzień 1 zawiera też **zadania A-C**. Budujesz w nich strony samodzielnie, według opisu, zamiast kopiować je z prowadzącego.
Każde zadanie stoi przed ćwiczeniem tego samego etapu, bo ćwiczenie dokłada coś do strony, którą właśnie zbudowałeś.

## Ćwiczenie 1: strona "O sklepie" (etap 1)

**Czas:** 10 minut. **Cel:** dodać stronę i link do niej w menu.

**Kroki**

1. Utwórz `Components/Pages/OSklepie.razor` z dyrektywą `@page "/o-sklepie"` i nagłówkiem `O sklepie`.
2. Dodaj do `Components/Layout/NavMenu.razor` pozycję prowadzącą do `/o-sklepie`, w tym samym stylu co pozostałe.
3. Zmień tytuł karty przeglądarki na `O sklepie - panel`, używając `<PageTitle>`.

**Kryteria akceptacji**

- [ ] Strona `/o-sklepie` otwiera się z menu.
- [ ] Tytuł karty przeglądarki się zgadza.
- [ ] Zmiana tekstu w pliku pojawia się w przeglądarce bez ręcznego restartu.

<details>
<summary>Wskazówka</summary>

Komponent z `@page` staje się stroną. Link w menu to zwykły `NavLink` z `href` bez wiodącego ukośnika.
</details>

## Zadanie A: lista produktów (etap 2)

**Czas:** 25 minut. **Cel:** zbudować stronę z tabelą danych z repozytorium i trzema stanami: ładowanie, pusta lista, błąd.

**Kroki**

1. Utwórz `Components/Pages/Products/List.razor` z dyrektywą `@page "/products"` i tytułem `<PageTitle>Produkty</PageTitle>`.
2. Wstrzyknij `IProductRepository` (namespace `Domain.Abstractions`). W `OnInitializedAsync` pobierz produkty przez `GetAllAsync()`.
3. Wyświetl tabelę z kolumnami `Nazwa`, `Opis`, `Cena`. Cena ma format `123,45 zł` (`ToString("C")`, kultura pl-PL jest ustawiona w `Program.cs`).
4. Dodaj trzy stany: podczas ładowania `Ładowanie produktów...`, dla pustej listy `Brak produktów.`, po błędzie komunikat z `catch`, a nie treść wyjątku.

**Kryteria akceptacji**

- [ ] `/products` pokazuje tabelę produktów z repozytorium.
- [ ] Przez około 2 sekundy widać `Ładowanie produktów...`.
- [ ] Każda cena ma format `123,45 zł`.
- [ ] Pusta lista i błąd mają własne komunikaty.

<details>
<summary>Wskazówka</summary>

Stan trzymaj w polu `List<Product>? products`: `null` oznacza, że dane się ładują. Tabela to zwykły `<table class="table">`. W etapie 4 wyodrębnisz ją do wspólnego komponentu `DataTable`, więc nie musisz na razie dbać o abstrakcję.
</details>

## Ćwiczenie 2: licznik wyników (etap 2)

**Czas:** 15 minut. **Cel:** pokazać liczbę pozycji dopiero wtedy, gdy dane są załadowane.

**Kroki**

1. Na stronie listy produktów, pod nagłówkiem, pokaż `Znaleziono: N`, gdzie `N` to liczba produktów po filtrach.
2. Licznik ma się pojawić dopiero po załadowaniu danych. W trakcie ładowania i przy błędzie nie pokazuj go wcale.
3. Sprawdź pusty stan: wpisz w wyszukiwarkę frazę bez wyników i zobacz, co pokazuje licznik.

**Kryteria akceptacji**

- [ ] Licznik pokazuje liczbę zgodną z tabelą.
- [ ] W trakcie ładowania licznika nie ma.
- [ ] Przy błędzie połączenia (np. przez zmianę adresu źródła danych) licznika nie ma.
- [ ] Dla frazy bez wyników licznik pokazuje `Znaleziono: 0`.

<details>
<summary>Wskazówka</summary>

Wyświetlaj licznik tylko wtedy, gdy `products` ma wartość inną niż `null` i nie ma błędu. Pusta lista to nie to samo co brak danych.
</details>

## Zadanie B: strona produktu i kod QR (etap 3)

**Czas:** 35 minut. **Cel:** zbudować stronę szczegółów z parametrem trasy, ograniczeniem `:int` i parametrem w query string.

**Kroki**

1. Utwórz `Components/Pages/Products/View.razor` z dyrektywą `@page "/products/{Id:int}"`. Parametr `Id` pobierz z adresu.
2. Pobierz produkt przez `IProductRepository.GetByIdAsync(Id)`. Pokaż nazwę (`<h1>`), opis, cenę, kod, kolor (lub `-`), datę ważności (lub `-`) i zdjęcie z `ImageUrl`.
3. Gdy produktu nie ma, pokaż nagłówek `Nie znaleziono produktu` i zdanie `Produkt o identyfikatorze {Id} nie istnieje.`
4. Dodaj parametr `Discount` odczytywany z `?discount=true`. Gdy jest `true`, cena regularna jest przekreślona, a obok pokazana jest cena po rabacie 10%. Stawkę wpisz w kodzie jako stałą (`0.10m`). W etapie 13 przeniesiesz ją do konfiguracji.
5. Dodaj przycisk `Pokaż cenę z rabatem` albo `Pokaż cenę regularną`. Przełącza on parametr `discount` w adresie (`Navigation.GetUriWithQueryParameter`). Id w trasie zostaje bez zmian.

**Kryteria akceptacji**

- [ ] `/products/1` pokazuje produkt z pełnymi danymi i zdjęciem.
- [ ] `/products/1?discount=true` pokazuje cenę po rabacie, a `/products/1` cenę regularną.
- [ ] Przycisk zmienia tylko parametr `discount`. Id w adresie zostaje.
- [ ] `/products/9999` pokazuje komunikat `Nie znaleziono produktu`.

**Bonus ★ (kod QR):** jeśli prowadzący pokazał w etapie 3 endpoint `/api/qrcode`, dodaj komponent `Components/Shared/QrCode.razor` z parametrami `Value` (wymagany) i `Size` (domyślnie `200`). Komponent składa adres `api/qrcode?data=...` w `<img>`, sam kodu nie generuje. Pod opisem pokaż kod z adresem strony produktu (`Navigation.ToAbsoluteUri($"products/{Id}")`).

<details>
<summary>Wskazówka</summary>

Parametr trasy: `[Parameter] public int Id { get; set; }`. Parametr z query string: `[SupplyParameterFromQuery] public bool Discount { get; set; }`.

Pobieranie danych zrób w `OnParametersSetAsync`, a nie w `OnInitializedAsync`. Przejście z `/products/1` na `/products/2` używa tej samej instancji komponentu i zmienia tylko parametr.

Dla QR: `Uri.EscapeDataString(wartosc)` w adresie obrazka. Zewnętrzne API (`https://api.qrserver.com/v1/create-qr-code/?data=...&size=...`) też działa, ale wysyła adres produktu poza aplikację.
</details>

## Ćwiczenie 3: nie znaleziono produktu (etap 3)

**Czas:** 10 minut. **Cel:** zrozumieć ograniczenie `:int` w adresie i obsługę braku produktu.

**Kroki**

1. Otwórz `/products/9999`. Zobacz komunikat `Nie znaleziono produktu`.
2. Dodaj pod tym komunikatem link `Wróć do listy` prowadzący do `/products`.
3. Otwórz `/products/abc`. Strona nie trafi do szczegółów, bo `:int` odrzuca tekst. Sprawdź, co wtedy się pokazuje.

**Kryteria akceptacji**

- [ ] `/products/9999` pokazuje komunikat i działający link powrotu.
- [ ] `/products/abc` nie pokazuje strony produktu, tylko stronę nie znaleziono.
- [ ] Wiesz, dlaczego te dwa adresy dają różne wyniki.

<details>
<summary>Wskazówka</summary>

Ograniczenie `:int` działa już na poziomie routingu. Strona produktu nie dostaje żądania dla `abc` i nie musi tego sprawdzać sama.
</details>

## Ćwiczenie 4: stopka tabeli (etap 4)

**Czas:** 10 minut. **Cel:** użyć `Footer` we wspólnym komponencie `DataTable`.

**Kroki**

1. Na liście produktów dodaj do `DataTable` fragment `<Footer>` z sumą cen widocznych wierszy (bieżąca strona, nie cała lista).
2. Sprawdź, czy lista klientów nadal działa bez żadnej zmiany w `DataTable`.

**Kryteria akceptacji**

- [ ] Stopka pokazuje sumę cen z bieżącej strony.
- [ ] Zmiana strony zmienia sumę.
- [ ] Lista klientów wygląda tak jak wcześniej.
- [ ] Nie zmieniłeś kodu `DataTable.razor`.

<details>
<summary>Wskazówka</summary>

`DataTable` ma parametr `Items` i fragment `Footer`. Suma to `Items.Sum(p => p.Price)`, a formatowanie `ToString("C")`.
</details>

## Zadanie C: filtr koloru i szczegóły (etap 5)

**Czas:** 30 minut. **Cel:** zapisać filtr koloru w adresie i prowadzić z listy do szczegółów z parametrami.

**Kroki**

1. Na stronie `/products` dodaj listę wyboru `Wszystkie kolory`, `Red`, `Green`, `Blue`.
2. Wybrany kolor zapisz w adresie jako `color`: `[SupplyParameterFromQuery] public string? Color { get; set; }`. Zmiana listy ma zaktualizować adres, a nie tylko zmienną.
3. Lista pokazuje tylko produkty w wybranym kolorze, bez rozróżniania wielkości liter. Adres `/products?color=red` pokazuje czerwone produkty.
4. Przy każdym wierszu dodaj dwa linki: `Szczegóły` (`products/{id}`) i `Szczegóły z rabatem` (`products/{id}?discount=true`).

**Kryteria akceptacji**

- [ ] Wybór koloru zmienia adres, np. `/products?color=Red`.
- [ ] Po odświeżeniu strony wybrany kolor zostaje.
- [ ] `/products?color=red` pokazuje tylko czerwone produkty.
- [ ] Linki prowadzą do `/products/{id}` z właściwym parametrem `discount`.

<details>
<summary>Wskazówka</summary>

`Navigation.GetUriWithQueryParameter("color", wartosc)` buduje nowy adres, a `null` usuwa parametr (tak jak w etapie 5 z prowadzącym). Filtruj po `GetAllAsync()` przez `string.Equals(p.Color, Color, StringComparison.OrdinalIgnoreCase)`. Repozytorium ma też `GetByColorAsync`, ale wtedy wielkość liter zależy od danych.

Linki w tabeli zapisz bez wiodącego ukośnika (`href="products/@product.Id"`), jak w ćwiczeniu 1.
</details>

## Ćwiczenie 5: filtr ceny w adresie (etap 5)

**Czas:** 15 minut. **Cel:** zapisać filtr w adresie, żeby przeżył odświeżenie.

**Kroki**

1. Dodaj pole `Cena maksymalna` obok wyszukiwarki i kolorów.
2. Zapisz wartość w adresie jako `maxPrice` (`[SupplyParameterFromQuery]`). Zmiana pola ma zaktualizować adres, a nie tylko zmienną.
3. Lista pokazuje tylko produkty z ceną nie większą niż `maxPrice`. Bez parametru filtr nie działa.

**Kryteria akceptacji**

- [ ] Po wpisaniu ceny w adresie pojawia się `maxPrice=...`.
- [ ] Lista pokazuje tylko produkty spełniające warunek.
- [ ] Po odświeżeniu strony filtr zostaje.
- [ ] Usunięcie wartości z pola usuwa filtr.

<details>
<summary>Wskazówka</summary>

`Navigation.GetUriWithQueryParameter("maxPrice", wartosc)` buduje nowy adres. Wartość `null` usuwa parametr.
</details>

---

## Wyzwanie dnia 1: Wygasające produkty

**Czas:** 45-60 minut. **Etapy:** 1-5.

**Scenariusz.** Magazynier chce wiedzieć, które produkty trzeba szybko sprzedać, zanim minie termin ważności.

**Wymagania**

1. Strona `/products/expiring` z listą produktów, których data ważności mieści się w wybranym okresie.
2. Okres podajesz w adresie: `?days=` (domyślnie 30). Do wyboru są przyciski 7, 30, 90 i 365 dni. Wybór zapisuje się w adresie.
3. Lista jest posortowana rosnąco po dacie ważności i pokazuje kolumny: nazwa, kolor, cena, data ważności.
4. Nowy komponent `ExpiryBadge` przyjmuje datę (`DateOnly`) i pokazuje, ile dni zostało:
   - do 7 dni: czerwony,
   - do 30 dni: żółty,
   - reszta: szary.
5. Strona ma stany ładowania, pustej listy i błędu. Pusta lista to `Brak produktów wygasających w tym okresie.`
6. W menu jest link `Wygasające`.

**Kryteria akceptacji**

- [ ] `/products/expiring` działa i nie koliduje z `/products/{id}`.
- [ ] Przyciski zmieniają `days` w adresie i listę.
- [ ] Odświeżenie strony zachowuje wybrany okres.
- [ ] `ExpiryBadge` ma trzy kolory zgodne z progami.
- [ ] Każdy stan (ładowanie, pusta lista, błąd) ma własny komunikat.
- [ ] Link jest w menu.

**Bonus ★:** stronicowanie listy (`Pager`) i licznik `Wygasa w ciągu N dni: X produktów`.

<details>
<summary>Podpowiedź 1</summary>

Strona z `@page "/products/expiring"` nie koliduje z `@page "/products/{Id:int}"`, bo drugi adres wymaga liczby. Uważaj jednak na adresy bez ograniczenia typu, np. `products/{slug}`.
</details>

<details>
<summary>Podpowiedź 2</summary>

Parametr z adresu: `[SupplyParameterFromQuery] public int? Days { get; set; }`. Dla `null` albo wartości spoza listy przyjmij 30.
</details>

<details>
<summary>Podpowiedź 3</summary>

Filtrowanie: `DateOnly.FromDateTime(DateTime.Today)` jako dziś, `AddDays(Days)` jako granica. Produkt bez daty ważności (`null`) pomijasz.
</details>
