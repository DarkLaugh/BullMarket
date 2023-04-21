using System.Threading.Tasks;
using BullMarket.Application.DTOs.Requests;
using BullMarket.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BullMarket.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IOptionsMonitor<JwtBearerOptions> jwtOptions;

        public AuthController(IAuthService authService, IOptionsMonitor<JwtBearerOptions> jwtOptions)
        {
            this.authService = authService;
            this.jwtOptions = jwtOptions;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            string token = await authService.Register(request);

            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            string token = await authService.Login(request);

            return Ok(new { Token = token });
        }

        [AllowAnonymous]
        [HttpGet("refresh-config")]
        public IActionResult RefreshOpenIdConfiguration()
        {
            jwtOptions.Get("BullMarket").ConfigurationManager.RequestRefresh();

            return Ok();
        }
    }
}
