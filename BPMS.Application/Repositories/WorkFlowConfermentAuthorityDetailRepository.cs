using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class WorkFlowConfermentAuthorityDetailRepository : Repository<WorkFlowConfermentAuthorityDetail>, IWorkFlowConfermentAuthorityDetailRepository
{
    public WorkFlowConfermentAuthorityDetailRepository(BpmsDbContext context) : base(context)
    {
    }
    public BpmsDbContext DbContext => Context;

    public IEnumerable<CreateAssignementViewModel> GetDetailsRecords(Guid id)
    {
        return DbContext.WorkFlowConfermentAuthorityDetails.Where(q => q.ConfermentAuthorityId == id)
            .ToList().Select(w => new CreateAssignementViewModel
            {
                Id = w.Id,
                StaffId = w.StaffId,
                UserId = DbContext.Users.Where(s => s.StaffId == w.StaffId).Select(t => t.Id).FirstOrDefault(),
                FullName = w.Staffs.FullName,
                PersonalCode = w.Staffs.PersonalCode,
                OnlyOwnRequest = w.OnlyOwnRequest,
                ToDate = HelperBs.MakeDate(w.ToDate.ToString()),
                FromDate = HelperBs.MakeDate(w.FromDate.ToString()),
                ConfermAuthorityId = w.ConfermentAuthorityId,
                StaffDropDown = new StaffDropDownViewModel()
                {
                    text = w.Staffs.FullName,
                    value = w.StaffId.ToString()
                }
            });
    }
}