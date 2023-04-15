using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class FormClassificationRelationRepository : Repository<FormClassificationRelation>, IFormClassificationRelationRepository
{
    public FormClassificationRelationRepository(BpmsDbContext context) : base(context)
    {
    }

    public List<FormClassification> GetRelatedFormClassifications(Guid formClassificationId)
    {
        var related = Context.FormClassificationRelations.Where(c => c.MainId == formClassificationId).Select(c => new GuidModel()
        {
            Id = c.SecondaryId
        });

        var x = (from fc in Context.FormClassificationRelations
            where related.Contains(new GuidModel()
            {
                Id = fc.Id
            })
            select fc).ToList();

        return Context.FormClassifications.Where(c => related.Contains(new GuidModel()
        {
            Id = c.Id
        })).OrderByDescending(c => c.RegisterDate).ThenByDescending(c => c.EditNo).Distinct().ToList();
    }
}

public class GuidModel
{
    public Guid Id { get; set; }
}