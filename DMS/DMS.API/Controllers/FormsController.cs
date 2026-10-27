using DMS.API.Controllers.Base;
using DMS.Library.Constants;
using DMS.Library.Services.Interfaces;
using DMS.Library.Services.Models.FormTemplate.DataIn;
using DMS.Library.Services.Models.FormTemplate.DataOut;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    // TODO: remove once login exists
    [AllowAnonymous]
    public class FormsController(IFormTemplateService _formTemplateService) : BaseController
    {
        /// <summary>
        /// All form templates, sorted by category and name.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<FormTemplateDataOut>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await _formTemplateService.GetAll());
        }

        /// <summary>
        /// Adds a form template with its file. The code (OBR-001, ...) is generated.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(FileRules.MaxFileSizeBytes)]
        [ProducesResponseType(typeof(FormTemplateDataOut), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromForm] CreateFormTemplateDataIn dataIn)
        {
            return HandleResult(await _formTemplateService.Create(dataIn));
        }

        /// <summary>
        /// Deletes a form template. The file stays on disk.
        /// </summary>
        [HttpDelete("{code}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string code)
        {
            return HandleResult(await _formTemplateService.Delete(code));
        }
    }
}
