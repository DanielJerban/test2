using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class AssingnmentRepository : Repository<Assingnment>, IAssingnmentRepository
{
    public AssingnmentRepository(BpmsDbContext context) : base(context)
    {

    }
    public BpmsDbContext DbContext => Context;

    public void ModifyBpmsGroupMapStaffId(IEnumerable<Guid> staffIds, Guid bpmsGroupId)
    {
        var staffId = from id in staffIds
            where !DbContext.Assingnments.Any(a => a.StaffId == id && a.ResponseTypeGroupId == bpmsGroupId)
            select id;


        var rep = DbContext.Assingnments.Where(r => r.ResponseTypeGroupId == bpmsGroupId).ToList()
            .Where(d => staffIds.All(u => u != d.StaffId));

        DbContext.Assingnments.RemoveRange(rep);

        foreach (var item in staffId)
        {
            var assign = new Assingnment()
            {
                StaffId = item,
                ResponseTypeGroupId = bpmsGroupId
            };
            DbContext.Assingnments.Add(assign);
        }
    }

    public IEnumerable<StaffViewModel> StaffsInSpecificBpmnGroup(Guid id)
    {
        var staffs = from dbAssignment in DbContext.Assingnments
            join dbstaffs in DbContext.Staffs on dbAssignment.StaffId equals dbstaffs.Id
            join user in DbContext.Users on dbstaffs.Id equals user.StaffId
            where dbAssignment.ResponseTypeGroupId == id
            select new StaffViewModel()
            {
                Id = dbstaffs.Id,
                FullName = dbstaffs.FName + " " + dbstaffs.LName,
                PersonalCode = dbstaffs.PersonalCode,
                UserName = user.UserName
            };
        return staffs;
    }
}