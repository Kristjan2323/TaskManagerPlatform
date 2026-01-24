using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            return await Task.FromResult((ActionResult)new OkObjectResult(loginRequest));
        }

        public async Task<IActionResult> RegisterUser(RegisterRequest registerRequest)
        {
            return await _identityService.RegisterUserAsync(registerRequest)
        }

    }
}
