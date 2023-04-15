using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class Contract
{
    public Contract()
    {
        Id = Guid.NewGuid();
    }
    [Key]
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    [Required(ErrorMessage = "{0} را وارد نمایید")]
    [Display(Name = "عنوان قرارداد")]
    public string Title { get; set; }
    public string RequestorUnit { get; set; }
    public string RequestorPerson { get; set; }
    public int RequestDate { get; set; }
    public string PersonFullName { get; set; }
    public string PersonFatherName { get; set; }
    public string PersonBirthDate { get; set; }
    public string  PersonIdentityNumber { get; set; }
    public string PersonPhoneNumber { get; set; }
    public string PersonMobileNumber { get; set; }
    public string PersonEmailAddress { get; set; }
    public string PersonPostalCode { get; set; }
    public string  PersonPostalAddress { get; set; }
    public string CompanyName { get; set; }
    public string CompanyType { get; set; }
    public string CompanyRegistrationNumber { get; set; }
    public string CompanyEconomicalCode { get; set; }
    public string  EstablishDate { get; set; }


    //Navigation Property
    public virtual Request Requests { get; set; }
}