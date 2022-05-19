using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dev_in_house_basic_authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicAuthenticationController : ControllerBase
    {
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return Ok(new { Mensagem = "Basic Authentication Funcionou", Role = "AdminRole" });
        }

        [HttpGet("senai")]
        [Authorize(Roles = "Senai")]
        public IActionResult Senai()
        {
            return Ok(new { Mensagem = "Basic Authentication Funcionou", Role = "SenaiRole" });
        }
    }
}
