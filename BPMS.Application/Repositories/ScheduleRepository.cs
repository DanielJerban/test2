using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using System.Text;

namespace BPMS.Application.Repositories;

public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(BpmsDbContext context) : base(context)
    {
    }
    public BpmsDbContext DbContext => Context;

    public IEnumerable<ScheduleViewModel> GetScheduleRecords()
    {
        var encoding = new UnicodeEncoding();

        var query = from schedule in DbContext.Schedules.ToList()
                    select new ScheduleViewModel()
                    {
                        ScheduleId = schedule.Id,
                        IsActive = schedule.IsActive,
                        TitleShchedules = schedule.Title,
                        StartDate = schedule.StartDate.ToString(),
                        RunTime = schedule.RunTime,
                        ThursDay = schedule.ThursDay,
                        IsRunExpireTrigger = schedule.IsRunExpireTrigger,
                        TaskType = schedule.TaskType.Title,
                        RegisterDate = schedule.RegisterDate,
                        EndDate = schedule.EndDate.ToString(),
                        DailyInterval = schedule.DailyInterval,
                        IsDaily = schedule.IsDaily,
                        MonDay = schedule.MonDay,
                        TuesDay = schedule.TuesDay,
                        Friday = schedule.Friday,
                        SaturDay = schedule.SaturDay,
                        SunDay = schedule.SunDay,
                        WednesDay = schedule.WednesDay,
                        TaskTypeId = schedule.TaskTypeId,
                        StartDateInt = schedule.StartDate,
                        EndDateInt = schedule.EndDate,
                        Code = (schedule.Content == null) ? "" : encoding.GetString(schedule.Content)
                    };
        var x = query.ToList();
        return query;
    }

    public void CreateSchedules(ScheduleViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Code))
        {
            //throw new ArgumentException("کد وارد نشده است.");
        }
        var registerDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
        var runTime = model.RunTime.Replace(":", string.Empty);
        var startDate = int.Parse(model.StartDate.Replace("/", string.Empty));
        var endDate = int.Parse(model.EndDate.Replace("/", string.Empty));

        if (endDate < startDate)
        {
            throw new ArgumentException("تاریخ پایان کوچک تر از تاریخ شروع وارد شده است");
        }

        //var code = HttpUtility.UrlDecode(model.Code, Encoding.UTF8);
        //var encoding = new UnicodeEncoding();
        //byte[] bytes = encoding.GetBytes(code);

        var schedule = DbContext.Schedules.SingleOrDefault(s => s.Id == model.ScheduleId);
        if (schedule != null)
        {
            schedule.TaskTypeId = model.TaskTypeId;
            schedule.DailyInterval = model.DailyInterval;
            schedule.EndDate = endDate;
            schedule.IsActive = model.IsActive;
            schedule.IsDaily = model.IsDaily;
            schedule.IsRunExpireTrigger = model.IsRunExpireTrigger;
            schedule.StartDate = startDate;
            schedule.RunTime = runTime;
            schedule.Title = model.TitleShchedules;
            schedule.SaturDay = model.SaturDay;
            schedule.SunDay = model.SunDay;
            schedule.MonDay = model.MonDay;
            schedule.TuesDay = model.TuesDay;
            schedule.WednesDay = model.WednesDay;
            schedule.ThursDay = model.ThursDay;
            schedule.Friday = model.Friday;
            // schedule.Content = bytes;
        }
        else
        {
            schedule = new Schedule()
            {
                DailyInterval = model.DailyInterval,
                EndDate = endDate,
                Friday = model.Friday,
                IsActive = model.IsActive,
                IsDaily = model.IsDaily,
                IsRunExpireTrigger = model.IsRunExpireTrigger,
                MonDay = model.MonDay,
                RegisterDate = registerDate,
                RunTime = runTime,
                SaturDay = model.SaturDay,
                StartDate = startDate,
                SunDay = model.SunDay,
                TaskTypeId = model.TaskTypeId,
                ThursDay = model.ThursDay,
                Title = model.TitleShchedules,
                TuesDay = model.TuesDay,
                WednesDay = model.WednesDay,
                // Content = bytes,
            };
            DbContext.Schedules.Add(schedule);
        }
    }

    public List<Schedule> GetActiveSchedules()
    {
        return DbContext.Schedules.Where(c => c.IsActive).ToList();
    }
}