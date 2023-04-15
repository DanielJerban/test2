using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Application.Repositories;

public class WorkFlowConfermentAuthorityRepository : Repository<WorkFlowConfermentAuthority>, IWorkFlowConfermentAuthorityRepository
{
    public WorkFlowConfermentAuthorityRepository(BpmsDbContext context) : base(context)
    {
    }

    public List<CreateAssignementViewModel> GetUsersForCartbotAssignment(Guid[] personIds, Guid confirmAuthorityId)
    {
        var staffList = new List<StaffViewModel>();
        var createmodel = new List<CreateAssignementViewModel>();
        foreach (var item in personIds)
        {
            var userIds = Context.Users.Where(u => u.Id == item).ToList().Select(s => new StaffViewModel()
            {
                Id = s.StaffId,
                UserId = s.Id,
                FullName = s.Staff.FullName,
                FName = s.Staff.FName,
                LName = s.Staff.LName,
                PersonalCode = s.Staff.PersonalCode,

            }).FirstOrDefault();
            staffList.Add(userIds);
        }
        foreach (var items in staffList.DistinctBy(u => u.Id))
        {
            var recordindetails = Context.WorkFlowConfermentAuthorityDetails.FirstOrDefault(w => w.StaffId == items.Id && w.ConfermentAuthorityId == confirmAuthorityId);
            if (recordindetails != null)
            {
                var viewmodel = new CreateAssignementViewModel()
                {
                    StaffId = items.Id,
                    FullName = items.FullName,
                    PersonalCode = items.PersonalCode,
                    FromDate = HelperBs.MakeDate(recordindetails.FromDate.ToString()),
                    ToDate = HelperBs.MakeDate(recordindetails.ToDate.ToString()),
                    ConfermAuthorityId = recordindetails.ConfermentAuthorityId,
                    OnlyOwnRequest = recordindetails.OnlyOwnRequest,
                    UserId = items.UserId,
                    Id = recordindetails.Id
                };
                createmodel.Add(viewmodel);
            }
            else
            {
                var viewmodel = new CreateAssignementViewModel()
                {
                    ConfermAuthorityId = confirmAuthorityId,
                    StaffId = items.Id,
                    FullName = items.FullName,
                    PersonalCode = items.PersonalCode,
                    FromDate = "",
                    ToDate = "",
                    OnlyOwnRequest = false,
                    UserId = items.UserId
                };

                createmodel.Add(viewmodel);
            }
        }
        return createmodel;
    }

    public IEnumerable<WorkFlowConfermentAuthorityViewModel> GetMasterGridData(string username)
    {
        var user = Context.Users.Single(c => c.UserName == username);
        var data = Context.WorkFlowConfermentAuthority.Include(x => x.WorkFlowConfermentAuthorityDetail).Include(x => x.Staff)
            .Where(k => k.StaffId == user.StaffId).ToList().Select(s => new WorkFlowConfermentAuthorityViewModel()
            {
                Id = s.Id,
                RegisterDate = s.RegisterDate,
                RequestTypeId = s.RequestTypeId,
                ConfermAuthorityId = s.Id,
                RequestTypeTitle = Context.LookUps.Where(r => r.Id == s.RequestTypeId).Select(c => c.Title).FirstOrDefault(),
                FromDate = HelperBs.MakeDate(s.WorkFlowConfermentAuthorityDetail.Select(x => x.FromDate.ToString()).FirstOrDefault()),
                ToDate = HelperBs.MakeDate(s.WorkFlowConfermentAuthorityDetail.Select(x => x.ToDate.ToString()).FirstOrDefault()),
                FullName = s.WorkFlowConfermentAuthorityDetail.Select(x => x.Staffs.FullName).FirstOrDefault(),
            });
        return data;
    }

    public void CheckInputDates(int fromDate, int toDate)
    {
        var registerDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));

        if (registerDate > fromDate)
        {
            throw new ArgumentException("تاریخ شروع نمی تواند از تاریخ روز کوچکتر باشد");
        }
        if (toDate < fromDate)
        {
            throw new ArgumentException("تاریخ پایان کوچک تر از تاریخ شروع وارد شده است");
        }
    }
}