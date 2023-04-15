using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class ThirdPartyRepository : Repository<ThirdParty>, IThirdPartyRepository
{
    public BpmsDbContext DbContext => Context;
    public ThirdPartyRepository(BpmsDbContext context) : base(context)
    {
    }


    public void AddOrUpdateThirdParty(ThirdPartyViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.ExpireDateString))
        {
            model.ExpireDate = Convert.ToInt32(model.ExpireDateString.Replace("/", string.Empty));
        }

        if (model.Id != null)
        {
            var thirdParty = DbContext.ThirdParties.Single(c => c.Id == model.Id);
            thirdParty.ExpireDate = model.ExpireDate;
            thirdParty.IsActive = model.IsActive;
            thirdParty.Password = model.Password;
            thirdParty.Name = model.Name;
            thirdParty.PasswordExpires = model.PasswordExpires;
            thirdParty.UserName = model.UserName;
            thirdParty.Description = model.Description;
            thirdParty.IPAddresses = model.IPAddresses;

            DbContext.ThirdParties.Update(thirdParty);
        }
        else
        {
            DbContext.ThirdParties.Add(new ThirdParty()
            {
                ExpireDate = model.ExpireDate,
                IsActive = model.IsActive,
                Password = model.Password,
                PasswordExpires = model.PasswordExpires,
                Name = model.Name,
                Description = model.Description,
                UserName = model.UserName,
                IPAddresses = model.IPAddresses
            });
        }

        DbContext.SaveChanges();
    }

    public void RemoveThirdParty(Guid id)
    {
        var thirdParty = DbContext.ThirdParties.Single(c => c.Id == id);
        DbContext.ThirdParties.Remove(thirdParty);
        DbContext.SaveChanges();
    }

    public List<ThirdPartyViewModel> GetThirdParties()
    {
        return DbContext.ThirdParties.Select(c => new ThirdPartyViewModel()
        {
            Id = c.Id,
            IsActive = c.IsActive,
            ExpireDate = c.ExpireDate,
            ExpireDateString = c.ExpireDate.ToString(),
            Password = c.Password,
            PasswordExpires = c.PasswordExpires,
            Name = c.Name,
            UserName = c.UserName,
            Description = c.Description,
            IPAddresses = c.IPAddresses
        }).ToList();
    }
}