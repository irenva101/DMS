using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json", "text/plain")]
    [ApiExplorerSettings(GroupName = "public")]
    public class BaseController : BaseProblemDetails
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public Guid? GetUserId()
        {
            var idClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            return Guid.TryParse(idClaim, out Guid ret) ? ret : null;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public Guid? GetRoleId()
        {
            var idClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "roleId")?.Value;
            return Guid.TryParse(idClaim, out Guid ret) ? ret : null;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public Guid? GetUtilityId()
        {
            var idClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "utilityId")?.Value;
            return Guid.TryParse(idClaim, out Guid ret) ? ret : null;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string? GetUtility()
        {
            var idClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "utility")?.Value;
            return idClaim;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public int GetTimeZone()
        {
            var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "timeZone")?.Value;
            return int.TryParse(claim, out var tz) ? tz : 0;
        }

    }
}
