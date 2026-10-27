using CSharpFunctionalExtensions;
using DMS.Library.Repositories.UOWs;
using DMS.Library.Services.Interfaces;
using DMS.Library.Services.Models.Document.DataOut;
using DMS.Shared.Commons;

namespace DMS.Library.Services.Implementations
{
    public class DocumentService(IUnitOfWork _unitOfWork, IFileStorage _fileStorage) : IDocumentService
    {
        public async Task<Result<DocumentFileDataOut, ResponseError>> Open(Guid documentId)
        {
            var document = await _unitOfWork.GetDocumentRepository().GetByDmsIdAsync(documentId);
            if (document.IsFailure)
                return document.Error;

            var content = _fileStorage.OpenRead(document.Value.StorageKey);
            if (content.IsFailure)
                return content.Error;

            var contentType = string.IsNullOrEmpty(document.Value.ContentType) ? "application/octet-stream" : document.Value.ContentType;
            return new DocumentFileDataOut(content.Value, document.Value.OriginalFileName, contentType);
        }
    }
}
