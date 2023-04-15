using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class WorkflowEsbRepository : Repository<WorkflowEsb>, IWorkflowEsbRepository
{
    public WorkflowEsbRepository(BpmsDbContext context) : base(context)
    {
    }

    public BpmsDbContext DbContext => Context;

    // todo: uncomment Later 
    //public JsonResult FillDataForMessages(WorkflowEsbViewModel model)
    //{
    //    var staffs = new List<UserViewModel>();
    //    var charts = new List<ChartViewModel>();
    //    var others = new List<UserViewModel>();
    //    var companies = new List<CompanyViewModel>();
    //    var clients = new List<ClientViewModel>();
    //    var fromForm = model.FromForm;

    //    if (model.Staffs != null)
    //        staffs.AddRange(from guid in model.Staffs
    //                        let staff = DbContext.Staffs.Find(guid)
    //                        select new UserViewModel()
    //                        {
    //                            StaffId = guid,
    //                            FullName = staff.FullName,
    //                            PersonelCode = staff.PersonalCode,
    //                            Email = staff.Email,
    //                            PhoneNumber = staff.PhoneNumber
    //                        });
    //    if (model.Charts != null)
    //        charts.AddRange(from guid in model.Charts
    //                        let chart = DbContext.Charts.Find(guid)
    //                        select new ChartViewModel()
    //                        {
    //                            Id = guid,
    //                            Title = chart.Title,
    //                            ParentTitle = chart.ParentId != null ? chart.ChartParent.Title : ""
    //                        });
    //    if (model.Others != null)
    //        others.AddRange(model.Others.Select(other => new UserViewModel()
    //        {
    //            FullName = other.Name,
    //            Email = other.Email,
    //            PhoneNumber = other.PhoneNumber
    //        }));
    //    if (model.Companies != null)
    //        companies.AddRange(from guid in model.Companies
    //                           let comp = DbContext.Companies.Find(guid)
    //                           select new CompanyViewModel()
    //                           {
    //                               Id = guid,
    //                               Email = comp.Email,
    //                               Name = comp.Name
    //                           });
    //    if (model.Clients != null)
    //        clients.AddRange(from guid in model.Clients
    //                         let cli = DbContext.Clients.Find(guid)
    //                         select new ClientViewModel()
    //                         {
    //                             Id = guid,
    //                             FName = cli.FName,
    //                             LName = cli.LName,
    //                             Email = cli.Email,
    //                             CellPhone = cli.CellPhone
    //                         });

    //    return new JsonResult(new { staffs, charts, others, companies, clients, fromForm });
    //}

    public WorkflowEsb GetWorkflowEsbByWorkFlowNextStepIdAndEventId(Guid workFlowNextStepId, string eventId)
    {
        return DbContext.WorkflowEsbs.FirstOrDefault(d => d.WorkflowNextStepId == workFlowNextStepId && d.EventId == eventId);
    }
}