using BPMS.BuildingBlocks.Application;
using BPMS.BuildingBlocks.Application.Configuration.Commands;
using BPMS.BuildingBlocks.Domain;
using BPMS.BuildingBlocks.Domain.Events;
using Marten;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ICommand = System.Windows.Input.ICommand;

namespace BPMS.BuildingBlocks.Infrastructure.Configuration.Processing;

public class CommandHandlerEntityEventPostProssesor<TRequest, TResult> : IRequestPostProcessor<TRequest, TResult> where TRequest : IRequestMessage<TResult>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public CommandHandlerEntityEventPostProssesor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task Process(TRequest request, TResult response, CancellationToken cancellationToken)
    {
        if (request is ICommand)
        {
            if (response is EntityResult entityResult)
            {
                var entity = entityResult.GetEntity<Entity>();
                if (entity != null)
                {
                    var domainEvents = entity.DomainEvents.ToList();
                    if (domainEvents.Any())
                    {
                        foreach (var domainEvent in domainEvents)
                        {

                            Task.Run(async () =>
                            {
                                await SaveToEvents(domainEvent, entity);
                            });
                            Task.Run(async () =>
                            {
                                using var scope = _serviceScopeFactory.CreateScope();

                                try
                                {
                                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                                    await mediator.Publish(domainEvent);
                                }
                                catch (Exception exception)
                                {
                                    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                                    var logger = loggerFactory.CreateLogger("DomainEventsProcessor");

                                    logger.LogError(exception, $"Error has accrued while execution domain event {domainEvent.GetType()}. the message is: {exception.Message}");
                                }
                            });
                        }

                        entity.ClearDomainEvents();
                    }
                }
            }

        }

        return Task.CompletedTask;
    }

    private async Task SaveToEvents(IDomainEvent domainEvent, Entity entity)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
        await using (var session = store.LightweightSession())
        {
            var events = await session.Events.FetchStreamAsync(entity.StreamId);
            if (!events.Any())
                session.Events.StartStream(entity.GetType(), entity.StreamId, domainEvent);
            else
                session.Events.Append(entity.StreamId, domainEvent);
            await session.SaveChangesAsync();
        }
    }
}