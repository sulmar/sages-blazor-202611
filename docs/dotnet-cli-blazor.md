# Ściąga: dotnet CLI dla Blazora

## Projekt Blazor (Blazor Web App)

```bash
dotnet new blazor
```

Tworzy projekt w bieżącym katalogu (nazwa projektu = nazwa katalogu).

## Projekt Blazor WebAssembly (PWA)

```bash
dotnet new blazorwasm -n MyApp -f net10.0 --pwa
```

| Opcja | Znaczenie |
| --- | --- |
| `-n MyApp` | nazwa projektu (i katalogu) |
| `-f net10.0` | framework docelowy |
| `--pwa` | dodaje obsługę Progressive Web App (manifest, service worker) |

## Solution

```bash
dotnet new sln
```

Tworzy plik `.sln` w bieżącym katalogu (nazwa = nazwa katalogu).

Dodanie projektu do solution:

```bash
dotnet sln add MyApp/MyApp.csproj
```

Można podać też katalog z projektem lub kilka projektów naraz:

```bash
dotnet sln add MyApp
dotnet sln add MyApp/MyApp.csproj MyApp.Tests/MyApp.Tests.csproj
```
