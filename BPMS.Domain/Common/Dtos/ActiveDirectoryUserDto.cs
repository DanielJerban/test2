namespace BPMS.Domain.Common.Dtos;

public class ActiveDirectoryUserDto
{
    public string CN { get; set; }
    public string LdapUserName { get; set; }
    public string PersonalCode { get; set; }
    public string Email { get; set; }
    public string FName { get; set; }
    public string LName { get; set; }
    public string PhoneNumber { get; set; }
    public string InsuranceNumber { get; set; }
    public string IdNumber { get; set; }
    public string LocalPhone { get; set; }
    public string ImagePath { get; set; }
    public string Password { get; set; }
    // Middle Initials
    public string Initials { get; set; }
    // Address
    public string homePostalAddress { get; set; }
    // title
    public string title { get; set; }
    // company
    public string company { get; set; }
    //state
    public string st { get; set; }
    //city
    public string l { get; set; }
    //country
    public string co { get; set; }
    //postal code
    public string postalCode { get; set; }
    //extention
    public string otherTelephone { get; set; }
    //fax
    public string facsimileTelephoneNumber { get; set; }
    // Challenge Question
    public string extensionAttribute1 { get; set; }
    // Challenge Response
    public string extensionAttribute2 { get; set; }
    //Member Company
    public string extensionAttribute3 { get; set; }
    // Company Relation ship Exits
    public string extensionAttribute4 { get; set; }
    //status
    public string extensionAttribute5 { get; set; }
    // Assigned Sales Person
    public string extensionAttribute6 { get; set; }
    // Accept T and C
    public string extensionAttribute7 { get; set; }
    // jobs
    public string extensionAttribute8 { get; set; }
    //public string extensionAttribute9 { get; set; }
    // email daily emerging market
    public string extensionAttribute10 { get; set; }
    // email daily corporate market
    public string extensionAttribute11 { get; set; }
    // AssetMgt Range
    public string extensionAttribute12 { get; set; }
    // date of account created
    public string whenCreated { get; set; }
    // date of account changed
    public string whenChanged { get; set; }
    public Guid StaffId { get; set; }
}