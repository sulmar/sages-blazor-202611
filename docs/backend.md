# Zaplecze: kontrakt dla panelu

Ta strona opisuje, co zaplecze udostępnia panelowi BlazorApp: adresy, konta, tokeny, endpointy, polityki i hub SignalR.
Dzięki niej nie trzeba czytać kodu `Shop.Api` ani `IdentityProvider.Api`, żeby zrealizować etapy 12, 14 i 16.

## Uruchomienie

| Usługa | Adres | Sprawdzenie |
| --- | --- | --- |
| `Shop.Api` | `http://localhost:5200` | `GET /health` zwraca `{"status":"ok"}` |
| `IdentityProvider.Api` | `http://localhost:5143` | `GET /` zwraca `Hello Auth.Api` |

W trybie `Development` (ustawiane w `launchSettings.json`) `Shop.Api` dokłada opóźnienie 2 s na listę
i 0,5 s na wyszukiwanie. Opóźnienia wyłączysz w `Shop.Api/appsettings.Development.json`.

Dane żyją w pamięci procesu. Restart `Shop.Api` przywraca dane startowe.

## Konta

| Login | Hasło | Dział | Role | Uprawnienia | Może |
| --- | --- | --- | --- | --- | --- |
| `john` | `123` | Development | - | - | odczyt produktów; klientów nie zobaczy (dział inny niż Sales) |
| `bob` | `bobsecure` | Sales | `dev`, `support` | `canedit` | odczyt produktów i klientów; dodawanie i edycja produktów i klientów |
| `alice` | `alicepass` | Sales | `admin` | `canprint`, `canedit` | wszystko z `bob` + usuwanie produktów |

## Logowanie

`POST http://localhost:5143/api/login`

```json
{ "username": "alice", "password": "alicepass" }
```

- `200 OK` - `{ "accessToken": "..." }`. Token jest podpisany HS256 i ważny 30 minut.
- `401 Unauthorized` - zły login lub hasło (bez treści).

Token wysyłasz w nagłówku `Authorization: Bearer <accessToken>`.

Oświadczenia (claims) w tokenie:

| Claim | Przykład | Do czego służy |
| --- | --- | --- |
| `name` | `alice` | nazwa użytkownika (`NameClaimType`) |
| `role` | `["admin"]` | tablica ról; polityka `Admin` |
| `permission` | `["canprint","canedit"]` | tablica uprawnień; polityka `CanEdit` wymaga wartości `canedit` |
| `Department` | `Sales` | dział; polityka `Sales` wymaga wartości `Sales` |
| `given_name`, `family_name`, `email`, `phone_number`, `birthdate` | `Alice`, `Johnson`, ... | dane osobowe, na razie niewykorzystywane przez panel |

`Shop.Api` sprawdza podpis, wystawcę (`https://domain.pl`), odbiorcę (`https://example.com`) i termin ważności.

## Polityki

| Polityka | Warunek | Gdzie |
| --- | --- | --- |
| `CanEdit` | oświadczenie `permission` zawiera `canedit` | dodawanie i edycja produktów i klientów |
| `Sales` | oświadczenie `Department` = `Sales` | cała grupa `/api/customers` |
| `Admin` | rola `admin` | usuwanie produktów |

Brak tokenu daje `401`. Token bez wymaganej polityki daje `403`.

## Produkty

Wszystkie adresy zaczynają się od `http://localhost:5200`.

| Metoda | Adres | Wymaga | Odpowiedź |
| --- | --- | --- | --- |
| `GET` | `/api/products?text=&color=` | nic | `200` - lista produktów; `text` szuka po tekście, `color` filtruje kolor (np. `Red`) |
| `GET` | `/api/products/page?skip=0&take=10` | nic | `200` - `{ "items": [...], "totalCount": 50 }`; `take` od 1 do 200, inaczej `400` |
| `GET` | `/api/products/{id}` | nic | `200` lub `404` |
| `POST` | `/api/products` | `CanEdit` | `201 Created`, `Location` i produkt z nadanym `id` (wartość `id` z zapytania jest ignorowana) |
| `PUT` | `/api/products/{id}` | `CanEdit` | `204 No Content` albo `404` |
| `DELETE` | `/api/products/{id}` | `Admin` | `204 No Content` |

Model produktu (JSON, camelCase): `id`, `name`, `description`, `price`, `code`, `color`, `expiryDate` (`yyyy-MM-dd`), `imageUrl`.

## Klienci

Cała grupa wymaga polityki `Sales`. Klientów nie usuwamy.

| Metoda | Adres | Wymaga | Odpowiedź |
| --- | --- | --- | --- |
| `GET` | `/api/customers?text=` | `Sales` | `200` - lista klientów |
| `GET` | `/api/customers/page?skip=0&take=10` | `Sales` | `200` - `{ "items": [...], "totalCount": 50 }` |
| `GET` | `/api/customers/{id}` | `Sales` | `200` lub `404` |
| `POST` | `/api/customers` | `Sales` + `CanEdit` | `201 Created` |
| `PUT` | `/api/customers/{id}` | `Sales` + `CanEdit` | `204 No Content` albo `404` |

Model klienta: `id`, `name` (do 100 znaków), `email`, `isDeleted`.

## Hub SignalR: liczba i średnia cena produktów

- Adres: `http://localhost:5200/hubs/products`. Hub nie wymaga tokenu i nie przyjmuje żadnych wywołań od klienta.
- Zdarzenie: `ProductStatsChanged`.
- Payload: `{ "count": 50, "averagePrice": 123.45 }`.
- Wysyłane po dodaniu i po usunięciu produktu (`POST` i `DELETE` na `/api/products`).

## Błędy

| Kod | Kiedy |
| --- | --- |
| `400` | nieprawidłowe `skip` lub `take` w `/page` |
| `401` | brak tokenu albo token nieważny |
| `403` | token ważny, ale brakuje polityki |
| `404` | rekord o podanym `id` nie istnieje |

Panel zamienia te kody na zrozumiałe komunikaty (`ShopApiException`, `ApiErrorMessage`).

## Które etapy z czego korzystają

| Etap | Endpoint lub mechanizm |
| --- | --- |
| 2, 3 | `GET /api/products`, `GET /api/products/{id}` |
| 5 | `GET /api/products/page`, parametry `text` i `color` |
| 6 | `POST` i `PUT` produktów i klientów |
| 12 | logowanie, polityki `CanEdit`, `Sales`, `Admin` |
| 14 | wszystkie endpointy przez nazwany `HttpClient` |
| 15 | błędy `401`, `403`, `404`, brak połączenia |
| 16 | hub `/hubs/products`, zdarzenie `ProductStatsChanged` |
