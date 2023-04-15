using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IFormClassificationRepository : IRepository<FormClassification>
{
    void Create(FormClassificationViewModel formClassification, Dictionary<string, MemoryStream> documents,
        string username, string webRootPath);

    void CreateNewVersion(FormClassificationViewModel formClassification, Dictionary<string, MemoryStream> documents, string username, string webRootPath);
    void SaveFormFiles(string id, Dictionary<string, MemoryStream> documents,string webRootPath);
    void UpdateFormClassificationRelations(Guid currentFormClassificationId, List<Guid> otherFormClassificationIds);
    void CheckClassificationId(string formNo, string editNo);
    bool FormNumberExists(FormClassificationViewModel form);
    List<FormClassificationViewModel> Report_ReadAll();
    List<FormClassificationViewModel> Report_Read(string username);
    List<FormClassificationViewModel> GetAllDocument();
}