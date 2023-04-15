namespace BPMS.Application.Services.EventManager;

public sealed class EventManagerInfoAggregate
{
    public BasicInfo BaseInfo { get; set; } = new();
    public EventSupInfo SupInfo { get; set; } = new();
    public IdenInfo IdensInfo { get; set; } = new();
    public EventOwnerInfo EventOwnerInformation { get; set; } = new();
    public EventOptionsInfo OptionsInfo { get; set; } = new();

    public bool IsOK() => SupInfo.IsOK() && IdensInfo.IsOK();

    public sealed class BasicInfo
    {
        public Guid UniqueId { get; set; }
        public DateTime CreateDate { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
    }

    public sealed class EventSupInfo
    {
        public DateTime EventEx { get; set; }

        public bool IsOK() => DateTime.Now <= EventEx;
    }

    public sealed class IdenInfo
    {
        public string IdenCode { get; set; }

        public bool IsOK()
        {
            bool answer = false;
            try
            {
                if (string.IsNullOrEmpty(IdenCode))
                    return answer;

                var systemInfoService = new EventInfoHardService();
                string currentSystemIdentity = systemInfoService.GetUniqueInfo();

                answer = (IdenCode == currentSystemIdentity);
            }
            catch
            {

            }
            return answer;
        }
    }

    public sealed class EventOwnerInfo
    {
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public sealed class EventOptionsInfo
    {
        public int? MaxActive { get; set; }
    }
}