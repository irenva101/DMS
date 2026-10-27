using CSharpFunctionalExtensions;
using DMS.Library.Constants;
using DMS.Library.Helpers;
using DMS.Library.Repositories.Models;
using DMS.Library.Repositories.UOWs;
using DMS.Library.Services.Interfaces;
using DMS.Library.Services.Models.FormTemplate.DataIn;
using DMS.Library.Services.Models.FormTemplate.DataOut;
using DMS.Shared.Commons;
using DMS.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DMS.Library.Services.Implementations
{
    public class FormTemplateService(IUnitOfWork _unitOfWork, IFileStorage _fileStorage) : IFormTemplateService
    {
        public async Task<Result<List<FormTemplateDataOut>, ResponseError>> GetAll()
        {
            var forms = await _unitOfWork.GetFormTemplateRepository().FindAsync(
                filter: f => !f.IsDeleted,
                include: q => q.Include(f => f.Documents.Where(d => !d.IsDeleted)),
                orderBy: q => q.OrderBy(f => f.Category).ThenBy(f => f.Name),
                options: new RepositoryOptions(AsNoTracking: true));

            if (forms.IsFailure)
                return forms.Error;

            return forms.Value.Select(f => new FormTemplateDataOut(f)).ToList();
        }

        public async Task<Result<FormTemplateDataOut, ResponseError>> Create(CreateFormTemplateDataIn dataIn)
        {
            if (string.IsNullOrWhiteSpace(dataIn.Name) || string.IsNullOrWhiteSpace(dataIn.Category))
                return new ResponseError(ResponseErrorCode.BadRequest, "Name and category are required.");

            if (dataIn.File == null || dataIn.File.Length == 0)
                return new ResponseError(ResponseErrorCode.BadRequest, "The file is empty.");

            if (dataIn.File.Length > FileRules.MaxFileSizeBytes)
                return new ResponseError(ResponseErrorCode.BadRequest, "The file is larger than 50 MB.");

            if (!FileRules.AllowedExtensions.Contains(Path.GetExtension(dataIn.File.FileName).ToLowerInvariant()))
                return new ResponseError(ResponseErrorCode.BadRequest, "This file type is not allowed.");

            return await _unitOfWork.CreateResilientTransaction().ExecuteAsync<Result<FormTemplateDataOut, ResponseError>>(async () =>
            {
                var sequence = await _unitOfWork.GetCodeSequenceRepository().GetNextValue("OBR", 0);
                if (sequence.IsFailure)
                    return sequence.Error;

                var code = $"OBR-{sequence.Value:000}";
                var name = dataIn.Name.Trim();
                var storageKey = $"Obrasci/{code}_{StorageKey.Sanitize(name)}/{StorageKey.FileName(dataIn.File.FileName)}";

                var form = new FormTemplateEntity
                {
                    Code = code,
                    Name = name,
                    Category = dataIn.Category.Trim(),
                    Documents =
                    [
                        new DocumentEntity
                        {
                            OriginalFileName = dataIn.File.FileName,
                            StorageKey = storageKey,
                            ContentType = dataIn.File.ContentType,
                            SizeBytes = dataIn.File.Length,
                        }
                    ],
                };

                var added = await _unitOfWork.GetFormTemplateRepository().AddAsync(form);
                if (added.IsFailure)
                    return added.Error;

                // Written inside the transaction, so a failed write rolls back the rows
                await using var content = dataIn.File.OpenReadStream();
                var saved = await _fileStorage.SaveAsync(storageKey, content);
                if (saved.IsFailure)
                    return saved.Error;

                return new FormTemplateDataOut(form);
            });
        }

        // Soft delete; the file stays on disk
        public async Task<Result<bool, ResponseError>> Delete(string code)
        {
            var form = await _unitOfWork.GetFormTemplateRepository().FindSingleAsync(
                filter: f => !f.IsDeleted && f.Code == code,
                include: q => q.Include(f => f.Documents.Where(d => !d.IsDeleted)));

            if (form.IsFailure)
                return form.Error;

            _unitOfWork.GetFormTemplateRepository().Remove(form.Value);
            _unitOfWork.GetDocumentRepository().RemoveRange(form.Value.Documents);

            var completed = await _unitOfWork.CompleteAsync();
            if (completed.IsFailure)
                return completed.Error;

            return true;
        }
    }
}
