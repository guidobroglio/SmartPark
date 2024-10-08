using System.ComponentModel.DataAnnotations;

namespace SmartParkCore.Security;

public record Registrazione
{
    [Required]
    public string Nome { get; set; }

    [Required]
    public string Cognome { get; set; }

    [Required]
    public string NumeroCarta { get; set; }

    public List<Veicolo> Veicoli { get; set; } = new List<Veicolo>();

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
