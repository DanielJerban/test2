using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Global;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class HolydayRepository : Repository<Holyday>, IHolydayRepository
{
    public HolydayRepository(BpmsDbContext context) : base(context)
    {
    }
    public BpmsDbContext _dbContext => Context;

    public HolydayViewModel CreateNewHolyday()
    {
        var startDate = _dbContext.LookUps.FirstOrDefault(d => d.Type == "WorkTime" && d.Code == 1)?.Aux;
        var startDateThr = _dbContext.LookUps.FirstOrDefault(d => d.Type == "WorkTime" && d.Code == 2)?.Aux;
        var endDate = _dbContext.LookUps.FirstOrDefault(d => d.Type == "WorkTime" && d.Code == 1)?.Aux2;
        var endDateThr = _dbContext.LookUps.FirstOrDefault(d => d.Type == "WorkTime" && d.Code == 2)?.Aux2;

        var holyType = _dbContext.LookUps.Where(f => f.Type == "HolydayType").Select(s => new SelectListItem()
        {
            Text = s.Title,
            Value = s.Id.ToString()
        });
        return new HolydayViewModel()
        {
            HolydayTypeSelectItem = holyType,
            StartTime = startDate?.Insert(2, ":"),
            StartTimeThr = startDateThr?.Insert(2, ":"),
            EndTime = endDate?.Insert(2, ":"),
            EndTimeThr = endDateThr?.Insert(2, ":")
        };

    }

    public Result SaveHolydayRecord(HolydayViewModel model)
    {

        var result = new Result() { IsValid = true, Message = "با موفقیت دخیره شد" };
        var holy = _dbContext.Holydays.FirstOrDefault(d => d.Id == model.Id);
        if (holy == null)
        {
            var dat = int.Parse(model.Date.Replace("/", string.Empty));
            var date = _dbContext.Holydays.FirstOrDefault(d => d.Date == dat);
            if (date != null)
            {
                result.IsValid = false;
                result.Message = "این تاریخ قبلا ثبت شده است.";
            }
            _dbContext.Holydays.Add(new Holyday()
            {
                Date = dat,
                Dsr = model.Dsr,
                HolydayTypeId = model.HolydayTypeId
            });
        }
        else
        {
            holy.Date = int.Parse(model.Date.Replace("/", string.Empty));
            holy.Dsr = model.Dsr;
            holy.HolydayTypeId = model.HolydayTypeId;
        }
        return result;
    }

    public void SaveWorkTime(HolydayViewModel model)
    {
        var saturdayToWendsday = _dbContext.LookUps.FirstOrDefault(d => d.Type == "WorkTime" && d.Code == 1);
        var thursday = _dbContext.LookUps.FirstOrDefault(d => d.Type == "WorkTime" && d.Code == 2);
        if (saturdayToWendsday != null)
        {
            saturdayToWendsday.Aux = model.StartTime.Replace(":", string.Empty);
            saturdayToWendsday.Aux2 = model.EndTime.Replace(":", string.Empty);
        }
        else
        {
            _dbContext.LookUps.Add(new LookUp()
            {
                Title = "شنبه تا چهارشنبه",
                Aux = model.StartTime.Replace(":", string.Empty),
                Aux2 = model.EndTime.Replace(":", string.Empty),
                IsActive = true,
                Code = 1,
                Type = "WorkTime"
            });
        }

        if (thursday != null)
        {
            thursday.Aux = model.StartTimeThr.Replace(":", string.Empty);
            thursday.Aux2 = model.EndTimeThr.Replace(":", string.Empty);
        }
        else
        {
            _dbContext.LookUps.Add(new LookUp()
            {
                Title = "پنجشنبه",
                Aux = model.StartTimeThr.Replace(":", string.Empty),
                Aux2 = model.EndTimeThr.Replace(":", string.Empty),
                IsActive = true,
                Code = 2,
                Type = "WorkTime"
            });
        }
    }

    IEnumerable<HolydayViewModel> IHolydayRepository.GetHolydayDay()
    {
        return _dbContext.Holydays.Select(d => new HolydayViewModel()
        {
            Date = d.Date.ToString(),
            HolydayTypeId = d.HolydayTypeId,
            Dsr = d.Dsr,
            HolydayType = d.HolydayType.Title,
            Id = d.Id
        });
    }

    public List<Holyday> GetHolidays()
    {
        return _dbContext.Holydays.ToList();
    }
}