using DMS.Library.Repositories.Models;

namespace DMS.Library.Services.Models.FormTemplate.DataOut
{
    public class FormTemplateDataOut
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        // For api/documents/{id}/download
        public Guid DocumentId { get; set; }
        public string FileName { get; set; }

        public FormTemplateDataOut()
        {

        }

        // Expects Documents loaded without deleted ones; a form always has exactly one file
        public FormTemplateDataOut(FormTemplateEntity x)
        {
            var document = x.Documents.Single();

            Code = x.Code;
            Name = x.Name;
            Category = x.Category;
            DocumentId = document.DmsId;
            FileName = document.OriginalFileName;
        }
    }
}
