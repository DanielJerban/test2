using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class EmployementCertificateRepository : Repository<EmployementCertificate>, IEmployementCertificateRepository
{
    public BpmsDbContext _dbContext => Context;

    public EmployementCertificateRepository(BpmsDbContext context) : base(context)
    {

    }

    public List<EmployementCertificateReportViewModel> GetAllCertificates()
    {
        var data = from empcertificates in _dbContext.EmployementCertificate.ToList()
                   join reqs in _dbContext.Requests on empcertificates.RequestId equals reqs.Id
                   join staffs in _dbContext.Staffs on reqs.StaffId equals staffs.Id
                   select new EmployementCertificateReportViewModel()
                   {
                       RequestIntention = empcertificates.RequestIntention == "Bank" ? "بانک" : "سایر موارد",
                       FirstName = staffs.FName,
                       LastName = staffs.LName,
                       PersonelCode = staffs.PersonalCode,
                       RequestNo = reqs.RequestNo,
                       EmployementDate = staffs.EmploymentDate == 0 ? " " : staffs.EmploymentDate.ToString().Insert(4, "/").Insert(7, "/")
                   };
        return data.ToList();
    }
}