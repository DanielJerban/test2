namespace BPMS.BuildingBlocks.Domain;

public interface IEntityNotFoundException
{
}

public class EntityNotFoundException<TEntity> : Exception, IEntityNotFoundException where TEntity : IAggregate
{
    public EntityNotFoundException() : this(string.Empty)
    {
    }

    public EntityNotFoundException(string message)
        : base($"Entity with type {typeof(TEntity).FullName} could not found{(!string.IsNullOrEmpty(message) ? $" with message: {message}" : string.Empty)}")
    {
    }
}