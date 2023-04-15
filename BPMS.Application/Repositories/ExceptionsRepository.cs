using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace BPMS.Application.Repositories;

public class ExceptionsRepository : Repository<Exceptions>, IExceptionsRepository
{
    public ExceptionsRepository(BpmsDbContext context) : base(context)
    {

    }

    public async Task<DataSourceResult> GetByPaging(DataSourceRequest request)
    {
        throw new NotImplementedException();
        // TODO: uncomment later 
        //return await Context.Exceptions.ToDataSourceResultAsync(request, exp => new
        //{
        //    Id = exp.Id.ToString(),
        //    Number = exp.Number.ToString(),
        //    exp.UserName,
        //    exp.IpAddress,
        //    CreateDate = exp.CreateDate.ToString("yyyy/MM/dd HH:mm"),
        //    IsRead = exp.IsRead ? "بله" : "خیر"

        //});
    }

    public IEnumerable<ExceptionsViewModel> GetAllExp()
    {
        return Context.Exceptions.Select(exp => new ExceptionsViewModel()
        {
            Id = exp.Id,
            Number = exp.Number,
            UserName = exp.UserName,
            IpAddress = exp.IpAddress,
            CreateDate = exp.CreateDate,
            IsRead = exp.IsRead
        });
    }
}