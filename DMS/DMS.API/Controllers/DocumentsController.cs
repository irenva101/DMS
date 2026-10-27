using DMS.API.Controllers.Base;
using DMS.Library.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    // TODO: remove once login exists
    [AllowAnonymous]
    public class DocumentsController(IDocumentService _documentService) : BaseController
    {
        /// <summary>
        /// Downloads any document (form, contract, bill or step file).
        /// </summary>
        [HttpGet("{id}/download")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK, "application/octet-stream")]
        public async Task<IActionResult> Download(Guid id)
        {
            var file = await _documentService.Open(id);
            if (file.IsFailure)
                return HandleResult(file);

            return File(file.Value.Content, file.Value.ContentType, file.Value.FileName);
        }
    }
}
