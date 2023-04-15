namespace BPMS.Infrastructure.Services;

public interface IProducer<T>
{
    void ProduceEvent(T @event);
}