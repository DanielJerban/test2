using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface ILookUpRepository : IRepository<LookUp>
{
    IEnumerable<LookUpViewModel> GetAllLookUpTypeTitles();
    IEnumerable<LookUpViewModel> GetAllLookUpsByTypeTitle(Guid? id, string type);
    IEnumerable<LookUpViewModel> GetActiveLookUpByType(string type);
    IEnumerable<LookUpViewModel> GetLookUpByType(string type);
    Guid GetLookUpByTypeAndCode(int code, string type);
    IEnumerable<LookUpDto> GetOrganizationPostType();
    IEnumerable<LookUpDto> GetOrganizationPostTitle();
    IEnumerable<LookUpViewModel> GetLookupsByIdBasedAux(Guid id);
    IEnumerable<LookUpViewModel> GetLookUpsByAux(string aux);
    void UpdateModifeidLookup(LookUpPhpDto model);
    void UpdateLookUpFromPhp(string type, List<LookUpPhpDto> lookups);
    IEnumerable<SelectListItem> GetWidgetTypeByAccess(string username);
    OneLevelLookUpViewModel BaseInfoOneLevel(string system);
    IEnumerable<LookUpViewModel> GetLookUpBySystem(string bpmsform);
    void CheckForDeleteInOneLevel(string id, string type, string system);
    void DeleteVirtualTable(Guid id);
    IEnumerable<LookUpViewModel> GetRequestTypeHasWorkflow(string username);
    string GetEventsOfDay(DateTime now);
    IEnumerable<LookUpViewModel> GetOneLevelLookup(string sys);
    void ChangeLoginPictureForSchedule();
    void CreateOneLevelLookup(IEnumerable<LookUpViewModel> modelOneLevelCreate, string subAux);
    string EditOneLevelLookup(IEnumerable<LookUpViewModel> modelOneLevelEdit, string modelSystem);
    void DeleteOneLevelLookup(IEnumerable<Guid> modelOneLevelDelete, List<Assingnment> assingnments = null);
    Guid CreateResponseGroup(LookUpViewModel model, string subAux);
    int GetNewCode(string type);
    LookUp GetByTypeAndCode(string type, int code);
}