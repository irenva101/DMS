using Microsoft.AspNetCore.Http;

namespace DMS.Library.Services.Models.FormTemplate.DataIn
{
    public class CreateFormTemplateDataIn
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public IFormFile File { get; set; }
    }
}
