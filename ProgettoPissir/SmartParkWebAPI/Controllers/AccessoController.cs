using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmartParkCore.Security;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.WebSockets;

namespace SmartParkWebAPI.Controllers;


[ApiController]
[Route("/Api/[controller]")]
//
public class AccessoController : Controller
{
    // UserManager<Utente> è un servizio di Identity che fornisce funzionalità per la gestione degli utenti
    private readonly UserManager<Utente> _userManager; 
    // SignInManager<Utente> è un servizio di Identity che fornisce funzionalità per la gestione dell'accesso
    private readonly SignInManager<Utente> _signInManager;
    // IConfiguration è un servizio di ASP.NET Core che fornisce l'accesso alla configurazione dell'applicazione
    private readonly IConfiguration _configuration;
    // ILogger è un servizio di ASP.NET Core che fornisce la registrazione dei log
    private readonly ILogger<AccessoController> _logger;
    // IGestoreVeicoli è un'interfaccia per la gestione dei veicoli
    private readonly IGestoreVeicoli _gestoreVeicoli;

    // Costruttore del controller: inietta i servizi necessari
    public AccessoController(ILogger<AccessoController> logger, UserManager<Utente> userManager,
        SignInManager<Utente> signInManager, IConfiguration configuration, IGestoreVeicoli gestoreVeicoli)
    {
        _logger = logger; // logger per la registrazione dei log
        _userManager = userManager; // gestione degli utenti
        _signInManager = signInManager; // gestione dell'accesso
        _configuration = configuration; // configurazione dell'applicazione
        _gestoreVeicoli = gestoreVeicoli; // gestione dei veicoli
    }

    // POST: /Api/Accesso/Registrati
    [HttpPost("Registrati")] // specifica il metodo HTTP e il percorso dell'endpoint
    [SwaggerOperation(Summary = "Registra un nuovo utente",
        Description = "Questo endpoint permette di registrare un nuovo utente con i dettagli forniti.")]

    // Definizione del metodo Registrati
    public async Task<IActionResult> Registrati(Registrazione utente)
    {
        // crea un nuovo utente con i dettagli forniti
        var user = new Utente
        {
            UserName = utente.Email, // nome utente
            Email = utente.Email, // email
            Nome = utente.Nome, // nome
            Cognome = utente.Cognome, // cognome
            NumeroCarta = utente.NumeroCarta // numero carta di credito
        };
        var result = await _userManager.CreateAsync(user, utente.Password); // crea l'utente

        // se l'utente è stato creato con successo
        if (result.Succeeded)
        {
            // registra l'utente nel database
            _logger.LogInformation($"Creato nuovo utente {user.Email}.");
            // assegna il ruolo di utente
            var roleResult = await _userManager.AddToRoleAsync(user, Ruolo.Utente);

            // imposta veicoli utente
            user = await _userManager.FindByEmailAsync(utente.Email);
            // assegna l'id dell'utente ai veicoli
            for (int i = 0; i < utente.Veicoli.Count(); i++)
            {
                // assegna l'id dell'utente al veicolo
                utente.Veicoli[i].IdUtente = user.Id;
            }
            // inserisci veicoli associati all'utente
            await _gestoreVeicoli.InserisciVeicoli(utente.Veicoli);
            // se l'utente è stato creato con successo
            if (roleResult.Succeeded)
            {
                // crea un nuovo token per l'utente
                await _signInManager.SignInAsync(user, isPersistent: false);
                // genera il token JWT
                var token = await GeneraTokenJwt(user);
                // registra il token generato
                _logger.LogInformation($"JWT Token generato: {token}");
                // ritorna il token generato
                return Ok(new { token });
            }
            // se non è stato possibile assegnare il ruolo
            else
            {
                return BadRequest("Impossibile creare utente");
            }
        }
        // se non è stato possibile creare l'utente
        else
        {
            return BadRequest("Impossibile creare utente");
        }
    }

    // POST: /Api/Accesso/RegistratiPremium
    [HttpPost("RegistratiPremium")]
    [SwaggerOperation(Summary = "Registra un utente come Premium",
       Description = "Questo endpoint permette di registrare un utente esistente come utente Premium.")]
    public async Task<IActionResult> RegistratiUtentePremium(Accesso utenteLogin)
    {
        // effettua l'accesso con le credenziali fornite
        var result = await _signInManager.PasswordSignInAsync(utenteLogin.Email, utenteLogin.Password, isPersistent: false, lockoutOnFailure: false);
        // se l'accesso è riuscito
        if (result.Succeeded)
        {
            // trova l'utente
            var utente = await _userManager.FindByEmailAsync(utenteLogin.Email);
            // registra l'utente nel database
            _logger.LogInformation($"Trovato utente: {utente.Email}");
            // se l'utente non è stato trovato
            if (utente == null)
            {
                // ritorna un errore
                return Unauthorized("Utente non trovato.");
            }
            // rimuove il ruolo attuale dell'utente
            var ruoloUtente = await _userManager.GetRolesAsync(utente);
            // rimuove il ruolo di utente
            await _userManager.RemoveFromRolesAsync(utente, ruoloUtente);
            // assegna il ruolo di utente premium
            var roleResult = await _userManager.AddToRoleAsync(utente, Ruolo.UtentePremium);
            // se il ruolo è stato assegnato con successo
            if (roleResult.Succeeded)
            {
                // crea un nuovo token per l'utente
                await _signInManager.SignInAsync(utente, isPersistent: false);
                // genera il token JWT
                var token = await GeneraTokenJwt(utente);
                // registra il token generato
                _logger.LogInformation($"JWT Token generato per utente premium: {token}");
                // ritorna il token generato
                return Ok(new { token });
            }
            // se non è stato possibile assegnare il ruolo
            else
            {
                // ritorna un errore
                return BadRequest("Impossibile aggiornare ruolo utente");
            }
        }
        // se l'accesso non è riuscito
        else
        {
            // ritorna un errore
            return Unauthorized("Accesso fallito.");
        }
    }


    // POST: /Api/Accesso/Accedi
    [HttpPost("Accedi")]
    [SwaggerOperation(Summary = "Effettua l'accesso",
        Description = "Questo endpoint permette di effettuare l'accesso con le credenziali fornite.")]
    public async Task<IActionResult> Accedi(Accesso utenteLogin)
    {
        // effettua l'accesso con le credenziali fornite
        var result = await _signInManager.PasswordSignInAsync(utenteLogin.Email, utenteLogin.Password, isPersistent: false, lockoutOnFailure: false);
        // se l'accesso è riuscito
        if (result.Succeeded)
        {
            // trova l'utente
            var utente = await _userManager.FindByEmailAsync(utenteLogin.Email);
            // registra l'utente nel database
            if (utente == null)
            {
                // ritorna un errore
                return Unauthorized("Utente non trovato.");
            }
            // crea un nuovo token per l'utente
            var token = await GeneraTokenJwt(utente);
            // registra il token generato
            _logger.LogInformation($"L'utente {utente.Email} ha effettuato l'accesso.");
            // ritorna il token generato
            return Ok(new { token });
        }
        // se l'accesso non è riuscito
        else
        {
            // ritorna un errore
            return Unauthorized("Accesso fallito.");
        }
    }

    // PUT: /Api/Accesso/Aggiorna
    [HttpPut("Aggiorna")]
    [SwaggerOperation(Summary = "Aggiorna le informazioni dell'utente",
        Description = "Questo endpoint permette di modificare alcune informazioni dell'utente.")]
    public async Task<IActionResult> Aggiorna(Registrazione utenteLogin)
    {              
        // effettua l'accesso con le credenziali fornite
        var result = await _signInManager.PasswordSignInAsync(utenteLogin.Email, utenteLogin.Password, isPersistent: false, lockoutOnFailure: false);
        // se l'accesso è riuscito
        if (result.Succeeded)
        {
            // imposta veicoli utente
            _logger.LogInformation($"Veicolo: {utenteLogin.Veicoli.First().Targa} {utenteLogin.Veicoli.First().CapacitaBatteria}");
            // aggiorna i veicoli
            await _gestoreVeicoli.ModificaVeicoli(utenteLogin.Veicoli);

            // trova utente
            var utente = await _userManager.FindByEmailAsync(utenteLogin.Email);
            // registra l'utente nel database
            _logger.LogInformation($"Trovato utente: {utente.Email}");
            // se l'utente non è stato trovato
            if (utente == null)
            {
                // ritorna un errore
                return Unauthorized("Utente non trovato.");
            }
            // aggiorna informazioni utente
            utente.Nome = utenteLogin.Nome; // nome
            utente.Cognome = utenteLogin.Cognome; // cognome
            utente.NumeroCarta = utenteLogin.NumeroCarta; // numero carta di credito

            // aggiorna utente
            var updateResult = await _userManager.UpdateAsync(utente);
            // se l'aggiornamento è riuscito
            if (updateResult.Succeeded)
            {
                // crea un nuovo token per l'utente
                await _signInManager.SignInAsync(utente, isPersistent: false);
                // genera il token JWT
                var token = await GeneraTokenJwt(utente);
                // registra il token generato
                _logger.LogInformation($"JWT Token generato per utente: {token}");
                // ritorna il token generato
                return Ok(new { token });
            }
            // se l'aggiornamento non è riuscito
            else
            {
                // ritorna un errore
                return BadRequest("Errore aggiornamento utente.");
            }
        }
        // se l'accesso non è riuscito
        else
        {
            // ritorna un errore
            return Unauthorized("Accesso fallito.");
        }
    }

    // GET: Api/Accesso
    [Authorize(Roles = Ruolo.Utente + "," + Ruolo.UtentePremium + "," + Ruolo.Amministratore)]
    [HttpGet]
    [SwaggerOperation(Summary = "Ritorna le informazioni dell'utente",
        Description = "Questo endpoint restituisce alcune informazioni modificabili dell'utente.")]
    public async Task<Registrazione> RichiediUtente(Guid id)
    {
        // trova l'utente con l'id fornito
        var utente = await _userManager.FindByIdAsync(id.ToString());
        // trova i veicoli associati all'utente
        var veicoli = await _gestoreVeicoli.RichiediVeicoli(id);
        // crea un nuovo oggetto Registrazione con le informazioni dell'utente
        var registrazione = new Registrazione
        {
            Nome = utente.Nome, // nome
            Cognome = utente.Cognome, // cognome
            Email = utente.Email, // email
            NumeroCarta = utente.NumeroCarta, // numero carta di credito
            Veicoli = veicoli // veicoli
        };
        // registra l'utente trovato
        _logger.LogInformation($"Trovato utente: {registrazione.Email}");
        // ritorna le informazioni dell'utente
        return registrazione;
    }
    // Genera un token JWT per l'utente
    private async Task<string> GeneraTokenJwt(Utente user)
    {
        // trova il ruolo dell'utente
        var ruolo = await _userManager.GetRolesAsync(user);
        // crea una lista di claim per il token
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email), // email
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // id univoco
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // id utente
            new Claim(ClaimTypes.Name, user.Nome), // nome
            new Claim(ClaimTypes.Role, ruolo.FirstOrDefault()) // ruolo
        };
        // crea una chiave simmetrica per la firma del token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        // crea le credenziali per la firma del token
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // imposta la scadenza del token
        var expires = DateTime.Now.AddHours(1);
        // crea il token JWT
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"], // emittente
            _configuration["Jwt:Audience"], // destinatario
            claims, // claim
            expires: expires, // scadenza
            signingCredentials: creds // credenziali
        );
        // ritorna il token JWT
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}