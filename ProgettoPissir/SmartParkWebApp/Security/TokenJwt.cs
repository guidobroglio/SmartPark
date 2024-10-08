using Azure.Core;
using System.Security.Claims;

public class TokenJwt
{
    private readonly HttpContext _httpContext;

    public TokenJwt(HttpContext httpContext)
    {
        _httpContext = httpContext;
    }

    public Guid EstraiIdUtente()
    {
        var tokenJwt = _httpContext.Request.Cookies["JwtToken"];
        Guid id = Guid.Empty;

        if (tokenJwt != null)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenJwt);

            var stringId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            id = Guid.Parse(stringId);
            
        }
        return id;
    }

    public string EstraiRuolo()
    {
        var tokenJwt = _httpContext.Request.Cookies["JwtToken"];
        var ruolo = "";

        if (tokenJwt != null)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenJwt);

            ruolo = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        return ruolo;
    }
}
