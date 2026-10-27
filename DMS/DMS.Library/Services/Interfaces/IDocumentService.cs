using CSharpFunctionalExtensions;
using DMS.Library.Services.Models.Document.DataOut;
using DMS.Shared.Commons;

namespace DMS.Library.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<Result<DocumentFileDataOut, ResponseError>> Open(Guid documentId);
    }
}
