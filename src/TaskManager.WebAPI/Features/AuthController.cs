using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Auth.DTOs;
using TaskManager.Application.Features.Auth.Services;

namespace TaskManager.WebAPI.Features
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _identityService.LoginAsync(loginRequest);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto registerRequest)
        {
            var result = await _identityService.RegisterUserAsync(registerRequest);
            return Ok(result);
        }

        [HttpPost("tenant")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto createTenantDto)
        {
            var result = await _identityService.CreateTenantAsync(createTenantDto);
            return Ok(result);
        }
    }
}
