using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Abstractions;
using TaskManager.Infrastructure.Abstractions;

namespace TaskManager.WebAPI.Features
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenantResolver _tenantResolver;
        private readonly ITenantProvider _tenantProvider;

        public TenantController(ITenantResolver tenantResolver, ITenantProvider tenantProvider)
        {
            _tenantResolver = tenantResolver;
            _tenantProvider = tenantProvider;
        }
        
        [HttpGet]
        public ActionResult<Guid> GetTenantId()
        {
            return Ok(_tenantProvider.GetTenantId());
        }
    }
}