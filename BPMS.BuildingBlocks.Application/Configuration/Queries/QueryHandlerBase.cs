using Microsoft.Extensions.Logging;

namespace BPMS.BuildingBlocks.Application.Configuration.Queries;

public abstract class QueryHandlerBase<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    protected ILogger Logger { get; }
    public QueryHandlerBase(ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger(GetType());
    }
    public async Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var rtn = await Batch(query, cancellationToken);
            return rtn;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, $"QueryHandler({GetType()}): Unexpected error: {exception.Message}");
            throw;
        }

    }
    protected abstract Task<TResult> Batch(TQuery query, CancellationToken cancellationToken = default);
}