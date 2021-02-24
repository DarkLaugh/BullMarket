using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BullMarket.Application.DTOs.Requests;
using BullMarket.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BullMarket.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
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
    }
}
