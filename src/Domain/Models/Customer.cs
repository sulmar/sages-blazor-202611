using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Customer : BaseEntity
{
    // Reguly walidacji zapisane atrybutami DataAnnotations - odczyta je <DataAnnotationsValidator />.
    [Required(ErrorMessage = "Podaj nazwę klienta.")]
    [StringLength(100, ErrorMessage = "Nazwa może mieć najwyżej 100 znaków.")]
    [MinLength(3)]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Podaj adres e-mail.")]
    [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail.")]
    public required string Email { get; set; }

    public bool IsDeleted { get; set; }

    /// <summary>Kopia do edycji w formularzu - oryginal zmienia sie dopiero po zapisie.</summary>
    public Customer Clone() => (Customer)MemberwiseClone();
}
