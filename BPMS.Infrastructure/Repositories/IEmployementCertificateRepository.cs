using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IEmployementCertificateRepository : IRepository<EmployementCertificate>
{
    List<EmployementCertificateReportViewModel> GetAllCertificates();
}