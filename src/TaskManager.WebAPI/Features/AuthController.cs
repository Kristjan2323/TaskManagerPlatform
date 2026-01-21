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
        public AuthController(IIdentityService identityService)
        {
            
        }
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            return await  Task.FromResult((ActionResult)new OkObjectResult(loginRequest));
        }
    }
}
