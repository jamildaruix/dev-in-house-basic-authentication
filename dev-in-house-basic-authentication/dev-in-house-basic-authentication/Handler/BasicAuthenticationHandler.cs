using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace dev_in_house_basic_authentication.Handler
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string header = Request.Headers["Authorization"].ToString();

            if (header.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                //Basic YWRtaW46YWRtaW4=
                string recebeToken = header.Substring("Basic ".Length).Trim();
                
                //YWRtaW46YWRtaW4= em um array de bytes
                byte[] decodeStringBase64 = Convert.FromBase64String(recebeToken);

                //admin:admin
                string credencialStringDecodingBase64 = System.Text.Encoding.UTF8.GetString(decodeStringBase64);

                //["admin"{0}, "admin"{1}] usuário:senha
                string[] credencial = credencialStringDecodingBase64.Split(':');

                if (credencial[0] == "admin" && credencial[1] == "admin")
                {
                    List<Claim> claims = new();

                    //Claim com o type name com o valor "mauro.martins" << usuário
                    claims.Add(new Claim("name", credencial[0]));

                    //Centro de Custo Admintrador
                    //Centro de Custo Financeiro
                    claims.Add(new Claim(ClaimTypes.Role, "Financeiro"));
                    claims.Add(new Claim(ClaimTypes.Country, "México"));

                    ClaimsIdentity identity = new ClaimsIdentity(claims, "Basic");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                    var tickect = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                    return Task.FromResult(AuthenticateResult.Success(tickect));
                }
                else
                {
                    //401 Unauthorized
                    Response.StatusCode = 401;
                    Response.Headers.Add("WWW-Authenticate", "Basic realm=\"dotnetthoughts.net\"");
                    return Task.FromResult(AuthenticateResult.Fail("Invalido acesso"));
                }
            }
            else
            {
                //Bad Request
                Response.StatusCode = 400;
                return Task.FromResult(AuthenticateResult.Fail("Invalido acesso"));
            }
        }
    }
}
