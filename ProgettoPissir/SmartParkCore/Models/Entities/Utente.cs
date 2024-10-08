using Microsoft.AspNetCore.Identity;

namespace SmartParkCore.Models.Entities;


public class Utente : IdentityUser<Guid>
{
    public string? Nome { get; set; }
    public string? Cognome { get; set; }
    public string? NumeroCarta { get; set; }
}
