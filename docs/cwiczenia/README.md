# Ćwiczenia i wyzwania

Szkolenie trwa trzy dni. Każdy dzień składa się z:

- **ćwiczeń etapowych** - jedno po każdym etapie, 10-15 minut. Sprawdzają, czy rozumiesz mechanizm, który właśnie zobaczyłeś,
- **zadań etapowych** (dni 1 i 2, oznaczone literami A-E) - 25-45 minut. Budujesz samodzielnie stronę lub komponent według opisu, z tym samym mechanizmem co w etapie,
- **wyzwania końcowego** - jedno na koniec dnia, 45-60 minut. Nowa funkcja panelu, zbudowana tylko z tego, czego nauczyłeś się w danym dniu.

| Dzień | Zadania | Ćwiczenia | Wyzwanie |
| --- | --- | --- | --- |
| 1 | A (etap 2), B (etap 3), C (etap 5) | etapy 1-5 | [Wygasające produkty](dzien-1.md#wyzwanie-dnia-1-wygasające-produkty) |
| 2 | D (etap 6), E (etap 9) | etapy 6-11 | [Duplikuj produkt](dzien-2.md#wyzwanie-dnia-2-duplikuj-produkt) |
| 3 | - | etapy 12-16 | [Drogie produkty na żywo](dzien-3.md#wyzwanie-dnia-3-drogie-produkty-na-żywo) |

- Ćwiczenia pierwszego dnia: [dzien-1.md](dzien-1.md)
- Ćwiczenia drugiego dnia: [dzien-2.md](dzien-2.md)
- Ćwiczenia trzeciego dnia: [dzien-3.md](dzien-3.md)

## Jak pracować

1. Zacznij od kroków z prowadzącym. Ćwiczenie zakłada, że etap został zrobiony razem z nim. Zadanie A-E zrób przed ćwiczeniem tego samego etapu, bo ćwiczenie dokłada coś do jego strony.
2. Przy każdym ćwiczeniu sprawdź **kryteria akceptacji**. Ćwiczenie jest skończone dopiero wtedy, gdy wszystkie są spełnione.
3. Podpowiedzi są schowane w sekcjach `Wskazówka`. Najpierw spróbuj sam, dopiero potem je rozwiń.
4. Uruchamiaj aplikację przez `dotnet watch --project BlazorApp`. Po zapisaniu pliku strona odświeży się sama.
5. Gdy zgubisz się w kodzie, poproś prowadzącego o punkt kontrolny: gotowy stan projektu po wybranym etapie. Nie kopiuj go bez zrozumienia, bo ćwiczenia następnego dnia zakładają, że sam go zbudowałeś.

## Kryteria akceptacji

Każde kryterium możesz sprawdzić jako **tak** albo **nie**. Jeśli nie jesteś pewien, czy coś działa, sprawdź to w przeglądarce, a nie w kodzie.

## Gdy coś nie działa

- Najpierw przeczytaj komunikat w konsoli `dotnet watch` i w konsoli przeglądarki (F12).
- Sprawdź, czy działają zaplecza: `http://localhost:5200/health` (Shop.Api) i `http://localhost:5143/` (IdentityProvider.Api).
- Po zmianie w `appsettings.json` lub w pliku `launchSettings.json` uruchom aplikację ponownie.
