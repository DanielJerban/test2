using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class UsefulLinksRepository : Repository<UsefulLinks>, IUsefulLinksRepository
{
    public BpmsDbContext DbContext => Context;
    public UsefulLinksRepository(BpmsDbContext context) : base(context)
    {
    }
    public void AddUsefulLink(UsefulLinks model)
    {
        DbContext.UsefulLinks.Add(model);
        DbContext.SaveChanges();
    }

    public void UpdateUsefulLink(UsefulLinks model)
    {
        var usefulLink = DbContext.UsefulLinks.FirstOrDefault(d => d.Id == model.Id);
        if (usefulLink != null)
        {
            usefulLink.Name = model.Name;
            usefulLink.Url = model.Url;
            usefulLink.Description = model.Description;
            usefulLink.IsExternalLink = model.IsExternalLink;
        }
        DbContext.SaveChanges();
    }

    public bool RemoveUsefulLink(Guid id)
    {
        var linkId = DbContext.UsefulLinks.FirstOrDefault(d => d.Id == id);
        if (linkId != null)
        {
            DbContext.UsefulLinks.Remove(linkId);
            DbContext.SaveChanges();
            return true;
        }
        return false;
    }

    public IQueryable<UsefulLinks> GetAllUsefulLinks()
    {
        return DbContext.UsefulLinks;
    }

}