namespace BlazorApp.Security;

/// <summary>Nazwy polityk autoryzacji - stale zamiast napisow rozsianych po komponentach.</summary>
public static class Policies
{
    /// <summary>Dodawanie i edycja danych: wymaga uprawnienia <c>canedit</c>.</summary>
    public const string CanEdit = nameof(CanEdit);

    /// <summary>Sekcja klientow: tylko pracownicy dzialu sprzedazy (oswiadczenie <c>Department</c>).</summary>
    public const string Sales = nameof(Sales);
}
