using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface ICompanyRepository : IRepository<Company>
{
    IEnumerable<CompanyViewModel> GetAllCompanies();

    IEnumerable<CompanyDto> GetCompanies();
    void CreateCompany(CompanyViewModel model);
    void DeleteCompany(Guid id);
    CompanyViewModel GetCompanyDetail(Guid modelId);
    IEnumerable<CompanyViewModel> GetCompaniesHaveEmail();
}