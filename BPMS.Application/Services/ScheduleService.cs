using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.Services;
using System.Text;
using BPMS.Infrastructure.MainHelpers;

namespace BPMS.Application.Services;

public class SchedulerService : IScheduleService
{
    private readonly CustomExceptionHandler _exceptions = new ();
    private readonly IScheduleLogsService _scheduleLogsService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCacheHelper _cacheHelper;

    public SchedulerService(IUnitOfWork unitOfWork, IScheduleLogsService scheduleLogsService, IDistributedCacheHelper cacheHelper)
    {
        _unitOfWork = unitOfWork;
        _scheduleLogsService = scheduleLogsService;
        _cacheHelper = cacheHelper;
    }

    public List<Schedule> GetSchedules()
    {
        return _cacheHelper.GetOrSet(CacheKeyHelper.GetScheduleCacheKey(), () => _unitOfWork.Schedules.GetActiveSchedules(), TimeSpan.FromDays(45));
    }

    public void ResetCatch()
    {
        _cacheHelper.Remove(CacheKeyHelper.GetScheduleCacheKey());
    }

    public void ExecuteSchedules(Schedule schedule)
    {
        if (!IsTimeToRunSchedule(schedule)) return;

        if (schedule.DailyInterval > 0)
        {
            if (CheckScheduleInterval(schedule.DailyInterval, schedule.Id))
            {
                ExecuteScheduleCode(schedule);
            }
        }
        else if (ShouldScheduleRunToday(schedule))
        {
            ExecuteScheduleCode(schedule);
        }
    }

    private void ExecuteScheduleCode(Schedule schedule)
    {
        if (schedule.Content != null)
        {
            try
            {
                var encoding = new UnicodeEncoding();
                var code = encoding.GetString(schedule.Content);
                // todo: uncomment later 
                // CompileRuntime.ExecuteVoidMethod(code);

                _scheduleLogsService.AddScheduleLog(schedule.Id);
            }
            catch (Exception e)
            {
                _exceptions.HandleException(e);
            }
        }
    }

    private bool ShouldScheduleRunToday(Schedule schedule)
    {
        var dayOfWeek = DateTime.Now.DayOfWeek;
        bool isToday;
        switch (dayOfWeek)
        {
            case DayOfWeek.Sunday:
                isToday = schedule.SunDay;
                break;
            case DayOfWeek.Monday:
                isToday = schedule.MonDay;
                break;
            case DayOfWeek.Tuesday:
                isToday = schedule.TuesDay;
                break;
            case DayOfWeek.Wednesday:
                isToday = schedule.WednesDay;
                break;
            case DayOfWeek.Thursday:
                isToday = schedule.ThursDay;
                break;
            case DayOfWeek.Friday:
                isToday = schedule.Friday;
                break;
            case DayOfWeek.Saturday:
                isToday = schedule.SaturDay;
                break;
            default:
                isToday = false;
                break;
        }

        return isToday;
    }

    private bool IsTimeToRunSchedule(Schedule schedule)
    {
        var date = DateTime.Now;

        int timeInt = HelperBs.GetIntTime(date);
        int runTimeInt = Convert.ToInt32(schedule.RunTime);

        var lastLogDate = _scheduleLogsService.GetScheduleLastLogDate(schedule.Id);
        if (lastLogDate == null)
        {
            return runTimeInt <= timeInt;
        }
        else
        {
            int todayDate = HelperBs.ConvertMiladyToIntShamsi(date);
            bool isDateInRange = schedule.StartDate <= todayDate && schedule.EndDate >= todayDate;
            bool isDayToRun = lastLogDate?.Date < date.Date;
            bool isTimeToRun = runTimeInt <= timeInt;
            return (isDateInRange && isDayToRun && isTimeToRun);
        }
    }

    private bool CheckScheduleInterval(int interval, Guid scheduleId)
    {
        var lastLogDate = _scheduleLogsService.GetScheduleLastLogDate(scheduleId);
        if (lastLogDate == null)
            return true;

        var nextLogDate = lastLogDate?.AddDays(interval).Date;
        var shouldRun = DateTime.Now.Date >= nextLogDate;
        return shouldRun;
    }
}