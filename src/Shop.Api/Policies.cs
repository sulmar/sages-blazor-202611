namespace Shop.Api;

/// <summary>Te same reguly co w aplikacji Blazor - ale to API jest ostatnia linia obrony.</summary>
public static class Policies
{
    /// <summary>Dodawanie i edycja danych: oswiadczenie <c>permission</c> = <c>canedit</c>.</summary>
    public const string CanEdit = nameof(CanEdit);

    /// <summary>Dane klientow: tylko dzial sprzedazy (oswiadczenie <c>Department</c> = <c>Sales</c>).</summary>
    public const string Sales = nameof(Sales);

    /// <summary>Usuwanie produktow: rola <c>admin</c>.</summary>
    public const string Admin = nameof(Admin);
}
