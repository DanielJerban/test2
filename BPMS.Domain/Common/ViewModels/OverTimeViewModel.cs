namespace BPMS.Domain.Common.ViewModels;

public class OverTimeViewModel
{
    public Guid uniqeid { get; set; }
    public string Ezafekari { get; set; }
    public int RequestDate { get; set; }
    public string RequestTime { get; set; }

    public string Dsr { get; set; }
    public bool IsValid { get; set; }
    public double Pric { get; set; }
    public float Pric2 { get; set; }
    public long LongNumber { get; set; }
    public short ShortNumber { get; set; }
    public byte ByteNumber { get; set; }
    public decimal Pric3 { get; set; }

    //public Guid RequestTypeId { get; set; }
    public bool HasItem(string a)
    {
        // todo: business should not be in the domain 
        // if (int.Parse(a) == RequestDate)
        // {
        // var d = UnitOfWork.LookUps.GetAllLookUpsByTypeTitle(null, "StaffType").ToList();
        // return d.Any(lookUpViewModel => lookUpViewModel.Code == 1);
        // }
        return false;
    }
}