namespace BPMS.Domain.Common.ViewModels;

public class OrderViewModel
{
    public OrderViewModel()
    {
        Items = new List<Item>();
    }

    public Guid Uniqeid { get; set; }
    public int OrderId { get; set; }
    public Customer Customer { get; set; }
    public Guid Id { get; set; }
    public List<Item> Items { get; set; }
    public string PersonalCode { get; set; }
    public bool IsValid { get; set; }
    public IList<int> Numbers { get; set; }
    public bool HasItem(string itemCode)
    {
        return Items.Any(x => x.ItemCode == itemCode);
    }
}
public class Item
{
    public decimal Cost { get; set; }
    public string ItemCode { get; set; }
}

public class Customer
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Country Country { get; set; }
}
public class Country
{
    public string CountryCode { get; set; }
}