using BPMS.BuildingBlocks.Domain;

namespace BPMS.BuildingBlocks.Application.Configuration.Commands;

public class EntityResult
{
    private Entity? entity;

    public void SetEntity(Entity entity)
    {
        this.entity = entity;
    }

    public TEntity? GetEntity<TEntity>() where TEntity : Entity
    {
        return entity as TEntity;
    }
}