using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IFormClassificationRelationRepository : IRepository<FormClassificationRelation>
{
    List<FormClassification> GetRelatedFormClassifications(Guid formClassificationId);
}