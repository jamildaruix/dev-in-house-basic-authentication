using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace dev_in_house_basic_authentication.Handler
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
        ) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Basic ".Length).Trim();
                System.Console.WriteLine(token);
                var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialstring.Split(':');

                //Simulando um retorno do banco de dados para validar o acesso 
                if (credentials[0] == "admin" && credentials[1] == "admin")
                {
                    //Claims pode ser considerada uma estrutura de validação ou armazenamento para acessos ao sistema \ aplicação
                    var claims = new[] { 
                        new Claim("name", credentials[0]), 
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim(ClaimTypes.Country, "México")
                    };
                    
                    var identity = new ClaimsIdentity(claims, "Basic");
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
                }

                //Simulando um retorno do banco de dados para validar o acesso 
                if (credentials[0] == "senai" && credentials[1] == "senai")
                {
                    //Claims pode ser considerada uma estrutura de validação ou armazenamento para acessos ao sistema \ aplicação
                    var claims = new[] { 
                            new Claim("name", credentials[0]), 
                            new Claim(ClaimTypes.Role, "Senai") ,
                            new Claim(ClaimTypes.Country, "Brasil")
                    };
                    var identity = new ClaimsIdentity(claims, "Basic");
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
                }

                Response.StatusCode = 401;
                Response.Headers.Add("WWW-Authenticate", "Basic realm=\"dotnetthoughts.net\"");
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
            else
            {
                Response.StatusCode = 400;
                //Response.Headers.Add("WWW-Authenticate", "Basic realm=\"dotnetthoughts.net\"");
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }
    }
}
