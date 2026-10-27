using CSharpFunctionalExtensions;
using DMS.Library.Services.Models.FormTemplate.DataIn;
using DMS.Library.Services.Models.FormTemplate.DataOut;
using DMS.Shared.Commons;

namespace DMS.Library.Services.Interfaces
{
    public interface IFormTemplateService
    {
        Task<Result<List<FormTemplateDataOut>, ResponseError>> GetAll();
        Task<Result<FormTemplateDataOut, ResponseError>> Create(CreateFormTemplateDataIn dataIn);
        Task<Result<bool, ResponseError>> Delete(string code);
    }
}
