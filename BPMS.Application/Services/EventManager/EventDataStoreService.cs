using BPMS.Application.Services.SupportCenter;
using BPMS.Domain.Common.Dtos;
using Newtonsoft.Json;

namespace BPMS.Application.Services.EventManager;

public sealed class EventDataStoreService
{
    private static bool? _eventIsAtFirstOK { get; set; }

    public static EventManagerInfoAggregate EventInfos { get; private set; }

    public static bool EventIsOK
    {
        get
        {
            if (_eventIsAtFirstOK == null)
                readTheEvent();
            return (_eventIsAtFirstOK.Value && eventDateIsOK());
        }
    }

    private static void MakeEventNotOK()
    {
        _eventIsAtFirstOK = false;
        EventInfos = null;

        tryRemoveEventFile();
    }

    public static bool MaxEventsReached(int activeProcesses)
    {
        if (EventInfos is null)
            return true;
        if (!EventIsOK)
            return true;
        if (EventInfos.OptionsInfo.MaxActive is null)
            return false;
        return (EventInfos.OptionsInfo.MaxActive <= activeProcesses);
    }

    public static void IsEventNotOKFromCenter()
    {
        if (!EventIsOK)
            return;

        Guid eventUniqueId = EventInfos.BaseInfo.UniqueId;


        try
        {
            bool isNotOK = true;

            string supportUrl = SupportAddressManager.GetSupportCenterUrl();

            using (var httpClient = new HttpClient())
            {
                string requestUrl = $"{supportUrl}/api/v1/Events/CheckEvent?TheUniqueId={eventUniqueId}";
                string content = httpClient.GetStringAsync(requestUrl).Result;
                EventIsNotOKOutputDTO outputDTO = JsonConvert.DeserializeObject<EventIsNotOKOutputDTO>(content);
                isNotOK = outputDTO.EventIsNotOK;
            }

            if (isNotOK)
            {
                MakeEventNotOK();
            }
        }
        catch
        {

        }
    }

    private static void readTheEvent()
    {
        try
        {
            string eventDirPath = Path.Combine("~/License");

            var eventReaderService = new EventReaderService();
            var eventInfo = eventReaderService.ReadEventFile(out bool eventIsOk, eventDirPath);
            _eventIsAtFirstOK = eventIsOk;
            EventInfos = eventInfo;
        }
        catch { }
    }

    private static bool eventDateIsOK()
    {
        if (EventInfos == null)
            return false;

        return EventInfos.SupInfo.IsOK();
    }

    private static void tryRemoveEventFile()
    {
        try
        {
            string eventDirectoryPath = Path.Combine("~/License");
            string eventPath = $"{eventDirectoryPath}\\BPMS.License";
            System.IO.File.Delete(eventPath);
        }
        catch { }
    }
}