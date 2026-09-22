using DMS.API.Controllers.Base;
using DMS.API.Services.GitRevisionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    public class HealthController(IGitRevisionService gitRevisionService) : BaseController
    {
        /// <summary>
        /// Health check endpoint.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var commitHash = gitRevisionService.GetCommitHash();
            return Ok(commitHash ?? "Git revision not available.");
        }
    }
}
